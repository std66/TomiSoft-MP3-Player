using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Playback {
	class YoutubePlayback : LocalAudioFilePlayback {
		private YoutubePlayback(string DownloadedFile) : base(DownloadedFile) {
		}

		public static async Task<IPlaybackManager> DownloadVideo(string Uri) {
			string Filename = Path.GetTempFileName();
			string MediaFilename = Path.ChangeExtension(Filename, "mp3");

			if (File.Exists(Filename))
				File.Delete(Filename);

			try {
				string YoutubeDownloader = "C:\\Program Files (x86)\\youtube-dl.exe";
				string Arguments = $"--extract-audio --audio-format mp3 -o \"{Filename}\" {Uri}";

				Process DownloaderProcess = new Process() {
					StartInfo = new ProcessStartInfo(
						YoutubeDownloader,
						Arguments
					) {
						UseShellExecute = true
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
	}
}
