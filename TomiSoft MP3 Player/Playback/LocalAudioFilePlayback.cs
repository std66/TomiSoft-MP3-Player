using TomiSoft.MP3Player.MediaInformation;
using Un4seen.Bass;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Provides a file playback method using BASS.
	/// </summary>
	internal class LocalAudioFilePlayback : BassPlaybackAbstract {
		/// <summary>
		/// Initializes a new instance of LocalAudioFilePlayback using
		/// the given filename.
		/// </summary>
		/// <param name="Filename">The file to play.</param>
		public LocalAudioFilePlayback(string Filename)
			:base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)){
			this.songInfo = new BassSongInfo(Filename);
		}

	}
}
