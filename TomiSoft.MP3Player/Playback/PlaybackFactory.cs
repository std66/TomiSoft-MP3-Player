using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomiSoft.ExternalApis.YoutubeDl;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Playback.BASS;
using TomiSoft.MP3Player.Playback.YouTube;
using TomiSoft.MP3Player.Utils;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Provides a simple way of loading media to play.
	/// </summary>
	public class PlaybackFactory {
		/// <summary>
		/// Stores the last IPlaybackManager instance.
		/// </summary>
		private static IPlaybackManager lastInstance;

		public static event Action<double, string> MediaOpenProgressChanged;

		/// <summary>
		/// Loads the given file.
		/// </summary>
		/// <param name="Source">The file's path or an uri to load.</param>
		/// <returns>An <see cref="IPlaybackManager"/> instance that can handle the given file.</returns>
		/// <exception cref="ArgumentNullException">when <paramref name="SongInfo"/> is null</exception> 
		/// <exception cref="NotSupportedException">when the media represented by <paramref name="SongInfo"/> is not supported by any playback methods</exception>
		public async static Task<IPlaybackManager> LoadMedia(ISongInfo SongInfo) {
			#region Error checking
			if (SongInfo == null)
				throw new ArgumentNullException(nameof(SongInfo));
			#endregion

			//If the Source is file:
			if (File.Exists(SongInfo.Source)) {
				string Extension = PlayerUtils.GetFileExtension(SongInfo.Source);

				//In case of any file supported by BASS:
				if (BassManager.GetSupportedExtensions().Contains(Extension)) {
					lastInstance = new LocalAudioFilePlayback(SongInfo.Source);
					return lastInstance;
				}
			}

			//If the Source is an Uri:
			else if (Uri.IsWellFormedUriString(SongInfo.Source, UriKind.Absolute)) {
				//Youtube link:
				if (SongInfo is YoutubeSongInfo) {
					Progress<YoutubeDownloadProgress> Progress = new Progress<YoutubeDownloadProgress>();
					Progress.ProgressChanged += YoutubeDownloadProgressChanged;

					lastInstance = await YoutubePlayback.DownloadVideoAsync(SongInfo, Progress);

					Progress.ProgressChanged -= YoutubeDownloadProgressChanged;
					return lastInstance;
				}
			}

			Trace.TraceWarning($"[Playback] Unsupported media: {SongInfo.Source}");
			throw new NotSupportedException("Nem támogatott média");
		}

		private static void YoutubeDownloadProgressChanged(object sender, YoutubeDownloadProgress e) {
			#region Error checking
			if (e == null)
				return;
			#endregion

			MediaOpenProgressChanged?.Invoke(e.Percentage, e.ToString());
		}
		
		/// <summary>
		/// Gets a Null-Playback manager instance.
		/// </summary>
		/// <param name="Volume">The volume to initialize with.</param>
		/// <returns>An <see cref="IPlaybackManager"/> that represents the Null-Playback manager instance</returns>
		public static IPlaybackManager NullPlayback(int Volume) {
			lastInstance = new NullPlayback() {
				Volume = Volume
			};

			return lastInstance;
		}

		/// <summary>
		/// Determines whether the given media is supported.
		/// </summary>
		/// <param name="Source">The source (a file's path or an URI) to check</param>
		/// <returns>True if the media is supported, false if not</returns>
		public static bool IsSupportedMedia(string Source) {
			if (File.Exists(Source)) {
				if (BassManager.IsSupportedFile(Source))
					return true;
			}

			else if (Uri.IsWellFormedUriString(Source, UriKind.Absolute)) {
				if (Source.Contains("youtube.com/watch?v="))
					return true;
			}

			return false;
		}
	}
}
