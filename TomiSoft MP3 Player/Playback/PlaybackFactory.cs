using System;
using System.Linq;
using TomiSoft.MP3Player.Utils;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Playback {
	public class PlaybackFactory {
		private static IPlaybackManager lastInstance;

		public static IPlaybackManager LoadFile(string Filename) {
			string Extension = PlayerUtils.GetFileExtension(Filename);

			if (BassManager.GetSupportedExtensions().Contains(Extension)) {
				lastInstance = new LocalAudioFilePlayback(Filename);
				return lastInstance;
			}

			throw new Exception("Nem támogatott fájlformátum");
		}

		public static IPlaybackManager NullPlayback(int Volume) {
			lastInstance = new NullPlayback() {
				Volume = Volume
			};

			return lastInstance;
		}

		public static IPlaybackManager GetLastInstance() {
			return lastInstance;
		}
	}
}
