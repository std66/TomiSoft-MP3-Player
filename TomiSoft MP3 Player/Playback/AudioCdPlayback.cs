using TomiSoft.MP3Player.MediaInformation;
using Un4seen.Bass.AddOn.Cd;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Provides an Audio CD Playback method using BASS.
	/// </summary>
	internal class AudioCdPlayback : BassPlaybackAbstract {
		/// <summary>
		/// Initializes a new instance of AudioCdPlayback class.
		/// Loads the given file that has CDA extension.
		/// </summary>
		/// <param name="Filename">The CDA file to open</param>
		public AudioCdPlayback(string Filename)
			: base(BassCd.BASS_CD_StreamCreateFile(Filename, Un4seen.Bass.BASSFlag.BASS_DEFAULT)) {
			this.songInfo = new BassSongInfo(ChannelID);
		}
	}
}
