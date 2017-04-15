using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Provides a playback method to support media on YouTube with
	/// youtube-dl.
	/// </summary>
	class YoutubePlayback : LocalAudioFilePlayback {
		/// <summary>
		/// Initializes a new instance of the <see cref="YoutubePlayback"/> class.
		/// </summary>
		/// <param name="DownloadedFile">The path of the media file downloaded and converted by youtube-dl.</param>
		private YoutubePlayback(string DownloadedFile) : base(DownloadedFile) {}

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
		/// <param name="Uri">The URI of the media to download</param>
		/// <returns>
		///		A <see cref="Task"/> for an <see cref="IPlaybackManager"/> instance
		///		that can play the YouTube media. Null is returned when the download
		///		fails.
		/// </returns>
		public static async Task<IPlaybackManager> DownloadVideoAsync(string Uri) {
			#region Error checking
			if (!IsValudYoutubeUri(Uri))
				throw new ArgumentException("Not a valid YouTube URI");
			#endregion

			Uri = GetVideoOnlyUri(Uri);

			string Filename = Path.GetTempFileName();
			string MediaFilename = Path.ChangeExtension(Filename, "mp3");

			#region Cleanup
			if (File.Exists(Filename))
				File.Delete(Filename);

			if (File.Exists(MediaFilename))
				File.Delete(MediaFilename);
			#endregion

			try {
				await UpdateYoutubeDlAsync();

				string YoutubeDownloader = "youtube-dl.exe";
				string Arguments = $"--extract-audio --audio-format mp3 -o \"{Filename}\" {Uri}";

				Process DownloaderProcess = new Process() {
					StartInfo = new ProcessStartInfo {
						FileName = YoutubeDownloader,
						Arguments = Arguments,
						UseShellExecute = true,
						WorkingDirectory = App.Path
					}
				};

				DownloaderProcess.Start();
				await Task.Run(() => DownloaderProcess.WaitForExit());
			}
			catch (Exception e) {
				Trace.WriteLine(e.Message);
				return null;
			}

			return new YoutubePlayback(MediaFilename);
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
		public static bool IsValudYoutubeUri(string Uri) {
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
		private static string GetVideoOnlyUri(string Uri) {
			Uri u = (new Uri(Uri));
			string VideoID = HttpUtility.ParseQueryString(u.Query)["v"];

			return $"http://youtube.com/watch?v={VideoID}";
		}

		/// <summary>
		/// Updates youtube-dl.
		/// </summary>
		/// <returns>A Task for a process that updates youtube-dl.</returns>
		private static async Task UpdateYoutubeDlAsync() {
			string YoutubeDownloader = "youtube-dl.exe";
			string Arguments = "-U";

			Process DownloaderProcess = new Process() {
				StartInfo = new ProcessStartInfo {
					FileName = YoutubeDownloader,
					Arguments = Arguments,
					UseShellExecute = true,
					WorkingDirectory = App.Path
				}
			};

			DownloaderProcess.Start();
			await Task.Run(() => DownloaderProcess.WaitForExit());
		}
	}
}
