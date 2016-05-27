using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	class LocalAudioFilePlayback : BassPlaybackAbstract {
		
		public LocalAudioFilePlayback(string Filename)
			:base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)){
			
		}

	}
}
