using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TomiSoft.MP3Player.Communication;
using Un4seen.Bass;

namespace TomiSoft.MP3Player.Playback {
	internal class NetworkAudioFilePlayback : BassPlaybackAbstract {
		private RemoteResourceInfo Info;

		public NetworkAudioFilePlayback(string Uri, RemoteResourceInfo Info) 
			: base(Bass.BASS_StreamCreateURL(Uri, 0, BASSFlag.BASS_DEFAULT, null, IntPtr.Zero)) {
			this.Info = Info;
		}
	}
}
