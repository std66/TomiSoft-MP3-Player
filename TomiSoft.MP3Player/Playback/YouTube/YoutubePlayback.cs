using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Playback.BASS;
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
			this.songInfo = new SongInfo(this.songInfo) {
				Title = SongInfo.Title
			};

			this.DownloadedFile = DownloadedFile;
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

			if (!IsValidYoutubeUri(SongInfo.Source))
				throw new ArgumentException("Not a valid YouTube URI");
			#endregion
			
			string MediaFilename = Path.ChangeExtension(Path.GetTempFileName(), "mp3");

			#region Cleanup
			if (File.Exists(MediaFilename))
				File.Delete(MediaFilename);
			#endregion

			try {
				YoutubeDl Downloader = new YoutubeDl("youtube-dl.exe", App.Path) {
					AudioFileFormat = YoutubeDlAudioFormat.mp3,
					Filename = MediaFilename,
					VideoID = GetVideoID(SongInfo.Source)
				};

				//When download fails and youtube-dl reports that an update is required, update it and retry.
				Downloader.UpdateRequired += async (o, e) => {
					Progress.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Updating, 0));
					await Downloader.UpdateAsync();

					await Downloader.DownloadAudioAsync(Progress);
				};

				await Downloader.DownloadAudioAsync(Progress);
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

		/// <summary>
		/// Determines that an URI string is a valid YouTube URI.
		/// </summary>
		/// <param name="Uri">The URI to check</param>
		/// <returns>True if valid, false if not</returns>
		public static bool IsValidYoutubeUri(string Uri) {
			if (!System.Uri.IsWellFormedUriString(Uri, UriKind.Absolute))
				return false;

			Uri u = (new Uri(Uri));

			if (!u.Host.Contains("youtube.com"))
				return false;

			if (!HttpUtility.ParseQueryString(u.Query).AllKeys.Contains("v"))
				return false;

			return true;
		}

		/// <summary>
		/// Creates a new YouTube URI that contains only the video ID.
		/// </summary>
		/// <param name="Uri">A YouTube URI to clean up.</param>
		/// <returns>A YouTube URI that contains only the video ID</returns>
		private static string GetVideoID(string Uri) {
			Uri u = (new Uri(Uri));
			return HttpUtility.ParseQueryString(u.Query)["v"];
		}

		
	}
}
