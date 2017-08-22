using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using TomiSoft.ExternalApis.YoutubeDl;
using TomiSoft.ExternalApis.YoutubeDl.MediaInformation;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Playback.BASS;
using TomiSoft.MP3Player.Utils;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Playback.YouTube {
	/// <summary>
	/// Provides a playback method to support media on YouTube with
	/// youtube-dl.
	/// </summary>
	class YoutubePlayback : LocalAudioFilePlayback {
		/// <summary>
		/// Stores the path of the downloaded file.
		/// </summary>
		private readonly string DownloadedFile;

		/// <summary>
		/// Initializes a new instance of the <see cref="YoutubePlayback"/> class.
		/// </summary>
		/// <param name="DownloadedFile">The path of the media file downloaded and converted by youtube-dl.</param>
		private YoutubePlayback(ISongInfo SongInfo, string DownloadedFile) : base(DownloadedFile) {
			this.songInfo = new SongInfo(SongInfo);

			this.DownloadedFile = DownloadedFile;
		}

		/// <summary>
		/// Creates a copy of the downloaded file to the given <see cref="Stream"/>. If the
		/// <paramref name="TargetStream"/> is a <see cref="FileStream"/>, additional metatags will
		/// be added.
		/// </summary>
		/// <param name="TargetStream">The <see cref="Stream"/> where the file is copied to</param>
		/// <param param name="Progress">An <see cref="IProgress{T}"/> instance that will be used to report the save progress. Can be null.</param>
		/// <returns>True if saving was successful, false if not</returns>
		public override async Task<bool> SaveToAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress) {
			bool SavedSuccessfully = await base.SaveToAsync(TargetStream, Progress);

			if (!SavedSuccessfully)
				return false;

			FileStream fs = TargetStream as FileStream;
			if (fs == null)
				return true;

			TargetStream.Dispose();

			TagLib.File f = TagLib.File.Create(fs.Name);
			f.Tag.Title = this.songInfo.Title;
			f.Tag.AlbumArtists = new string[] { this.songInfo.Artist };

			if (this.songInfo.AlbumImage != null) {
				using (MemoryStream ms = new MemoryStream()) {
					this.songInfo.AlbumImage.Save(ms, ImageFormat.Png);
					ms.Position = 0;
					f.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(TagLib.ByteVector.FromStream(ms)) };
				}
			}

			f.Save();

			return true;
		}

		/// <summary>
		/// Closes the BASS channel and deletes the downloaded file.
		/// </summary>
		public override void Dispose() {
			base.Dispose();
			File.Delete(this.DownloadedFile);
		}

		/// <summary>
		/// Gets if all tools are available to play media from YouTube. If false,
		/// call <see cref="DownloadSoftwareAsync"/>.
		/// </summary>
		public static bool ToolsAvailable {
			get {
				string[] Files = new string[] {
					"ffmpeg.exe", "ffprobe.exe", "youtube-dl.exe"
				};

				foreach (var Filename in Files) {
					if (!File.Exists(App.Path + Filename))
						return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Downloads a video asynchronously with youtube-dl.
		/// </summary>
		/// <param name="SongInfo">A <see cref="ISongInfo"/> instance that holds the URI of the media to download</param>
		/// <returns>
		///		A <see cref="Task"/> for an <see cref="IPlaybackManager"/> instance
		///		that can play the YouTube media. Null is returned when the download
		///		fails.
		/// </returns>
		/// <exception cref="ArgumentNullException">when <paramref name="SongInfo"/> is null</exception> 
		/// <exception cref="ArgumentException">when <see cref="ISongInfo.Source"/> is not a valid YouTube URI</exception>
		public static async Task<IPlaybackManager> DownloadVideoAsync(ISongInfo SongInfo, IProgress<YoutubeDownloadProgress> Progress) {
			#region Error checking
			if (SongInfo == null)
				throw new ArgumentNullException(nameof(SongInfo));

			if (!YoutubeUri.IsValidYoutubeUri(SongInfo.Source))
				throw new ArgumentException("Not a valid YouTube URI");
			#endregion

			string MediaFilename = Path.ChangeExtension(Path.GetTempFileName(), "mp3");

			#region Cleanup
			if (File.Exists(MediaFilename))
				File.Delete(MediaFilename);
			#endregion

			MediaFormat Format = (SongInfo as YoutubeSongInfo)?.GetBestAudioFormat();

			try {
				YoutubeDl Downloader = new YoutubeDl("youtube-dl.exe", App.Path) {
					AudioFileFormat = YoutubeDlAudioFormat.mp3,
					Filename = MediaFilename,
					VideoID = YoutubeUri.GetVideoID(SongInfo.Source)
				};

				//When download fails and youtube-dl reports that an update is required, update it and retry.
				Downloader.UpdateRequired += async (o, e) => {
					Progress.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Updating, 0));
					await Downloader.UpdateAsync();

					await Downloader.DownloadAudioAsync(Progress);
				};

				await Downloader.DownloadAudioAsync(Progress, Format);
			}
			catch (Exception e) {
				Trace.WriteLine(e.Message);
				return null;
			}

			if (File.Exists(MediaFilename))
				return new YoutubePlayback(SongInfo, MediaFilename);
			else
				return null;
		}

		/// <summary>
		/// Downloads the required additional softwares asynchronously for downloading and converting
		/// YouTube videos.
		/// </summary>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		public static async Task DownloadSoftwareAsync() {
			string CpuArchitecture = Environment.Is64BitOperatingSystem ? "64" : "32";

			//Define paths and URIs
			string FFMpeg = $"http://ffmpeg.zeranoe.com/builds/win{CpuArchitecture}/static/ffmpeg-latest-win{CpuArchitecture}-static.zip";
			string FFMpegLocation = Path.ChangeExtension(Path.GetTempFileName(), "zip");

			string YoutubeDl = "https://yt-dl.org/downloads/latest/youtube-dl.exe";
			string YoutubeDlLocation = App.Path + "youtube-dl.exe";

			//Download softwares
			WebClient cl = new WebClient();
			await cl.DownloadFileTaskAsync(FFMpeg, FFMpegLocation);
			await cl.DownloadFileTaskAsync(YoutubeDl, YoutubeDlLocation);

			//Extract ffmpeg
			using (ZipArchive Archive = ZipFile.OpenRead(FFMpegLocation)) {
				string Dir = $"ffmpeg-latest-win{CpuArchitecture}-static";
				await Task.Run(() => {
					Archive.GetEntry($"{Dir}/bin/ffmpeg.exe").ExtractToFile(App.Path + "ffmpeg.exe");
					Archive.GetEntry($"{Dir}/bin/ffprobe.exe").ExtractToFile(App.Path + "ffprobe.exe");
				});
			}

			//Delete unnecessary ffmpeg archive
			File.Delete(FFMpegLocation);
		}
	}
}
