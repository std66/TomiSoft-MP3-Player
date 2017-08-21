using System.ComponentModel;

namespace TomiSoft.MP3Player.Common.Playback {
	public class AudioPeakMeter : IAudioPeakMeter {
        public int LeftPeak {
            get;
            set;
        }

		public int Maximum {
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

		public override string ToString() {
			return $"Left={this.LeftPeak} Right={this.RightPeak} Maximum={this.Maximum}";
		}
	}
}
