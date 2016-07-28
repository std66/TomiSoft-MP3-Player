using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Provides a file playback method using BASS.
	/// </summary>
	class LocalAudioFilePlayback : BassPlaybackAbstract {
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
