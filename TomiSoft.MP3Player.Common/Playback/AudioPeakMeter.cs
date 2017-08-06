using System.ComponentModel;

namespace TomiSoft.MP3Player.Common.Playback {
    public class AudioPeakMeter : IAudioPeakMeter {
        public int LeftPeak {
            get;
            set;
        }

        public int RightPeak {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { }
            remove { }
        }
    }
}
