using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Playback.YouTube {
	enum YoutubeDlAudioFormat {
		aac, flac, mp3, m4a, opus, vorbis
	}

	class YoutubeDl {
		private readonly string ExecutablePath;
		private readonly string WorkingDirectory;

		public string VideoID {
			get;
			set;
		}

		public string Filename {
			get;
			set;
		}

		public YoutubeDlAudioFormat FileFormat {
			get;
			set;
		} = YoutubeDlAudioFormat.mp3;

		public YoutubeDl(string ExecutablePathInWorkingDirectory, string WorkingDirectory) {
			#region Error checking
			if (!Directory.Exists(WorkingDirectory))
				throw new DirectoryNotFoundException($"Directory does not exists: {WorkingDirectory}");

			if (!File.Exists(Path.Combine(WorkingDirectory, ExecutablePathInWorkingDirectory)))
				throw new FileNotFoundException($"youtube-dl was not found at path: {ExecutablePathInWorkingDirectory}");
			#endregion

			this.ExecutablePath = ExecutablePathInWorkingDirectory;
			this.WorkingDirectory = WorkingDirectory;
		}

		public async Task DownloadAudioAsync(IProgress<YoutubeDownloadProgress> Progress) {
			string TempPath = Path.ChangeExtension(this.Filename, ".tmp");
			if (File.Exists(TempPath))
				File.Delete(TempPath);

			Process DownloaderProcess = new Process { StartInfo = this.GetProcessStartInfo(TempPath) };

			DownloaderProcess.OutputDataReceived += (o, e) => ReportProgress(e.Data, Progress);
			DownloaderProcess.ErrorDataReceived += (o, e) => {
				if (!String.IsNullOrWhiteSpace(e.Data)) {
					Progress?.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Error, 0));
					Trace.TraceWarning($"youtube-dl StdErr: {e.Data}");
				}
			};

			Progress?.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Initializing, 0));

			DownloaderProcess.Start();
			DownloaderProcess.BeginOutputReadLine();
			DownloaderProcess.BeginErrorReadLine();

			await Task.Run(() => DownloaderProcess.WaitForExit());

			Progress?.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Completed, 100));
		}

		/// <summary>
		/// Updates youtube-dl.
		/// </summary>
		/// <returns>A Task for a process that updates youtube-dl.</returns>
		public async Task UpdateAsync() {
			Process DownloaderProcess = new Process() {
				StartInfo = new ProcessStartInfo {
					FileName = this.ExecutablePath,
					Arguments = "-U",
					UseShellExecute = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = this.WorkingDirectory
				}
			};

			DownloaderProcess.Start();
			await Task.Run(() => DownloaderProcess.WaitForExit());
		}

		private static void ReportProgress(string Output, IProgress<YoutubeDownloadProgress> Progress) {
			#region Error checking
			if (Progress == null || String.IsNullOrWhiteSpace(Output))
				return;
			#endregion

			Trace.TraceInformation($"youtube-dl StdOut: {Output}");

			if (Output.ToLower().StartsWith("[download]") && !Output.ToLower().Contains("destination")) {
				string Match = Regex.Match(Output, @"\b\d+([\.,]\d+)?").Value;
				
				double Percentage;
				if (double.TryParse(Match, NumberStyles.Any, CultureInfo.InvariantCulture, out Percentage))
					Progress.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Downloading, Percentage));
			}

			else if (Output.ToLower().StartsWith("[ffmpeg]")) {
				Progress.Report(new YoutubeDownloadProgress(YoutubeDownloadStatus.Converting, 0));
			}
		}

		private ProcessStartInfo GetProcessStartInfo(string TempFilename) {
			string Uri = $"https://youtube.com/watch?v={this.VideoID}";

			return new ProcessStartInfo {
				FileName = this.ExecutablePath,
				Arguments = $"--extract-audio --audio-format {this.FileFormat} -o \"{TempFilename}\" {Uri}",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				WorkingDirectory = this.WorkingDirectory
			};
		}
	}
}
