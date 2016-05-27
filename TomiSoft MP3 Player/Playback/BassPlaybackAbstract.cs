using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	abstract class BassPlaybackAbstract : IPlaybackManager {
		private bool playing;
		private int channelID;
		public event PropertyChangedEventHandler PropertyChanged;
		public event Action SongEnded;

		private DispatcherTimer PlaybackTimer;

		public bool IsPlaying {
			get {
				return this.playing;
			}
			private set {
				this.playing = value;
				this.NotifyPropertyChanged("IsPlaying");

				if (this.playing) {
					this.PlaybackTimer.Start();
				}
				else {
					this.PlaybackTimer.Stop();
				}
			}
		}

		public int Volume {
			get {
				float vol = 1;
				Bass.BASS_ChannelGetAttribute(this.channelID, BASSAttribute.BASS_ATTRIB_VOL, ref vol);

				return (int)(vol * 100);
			}
			set {
				if (value < 0 || value > 100)
					throw new Exception("A hangerő 0 és 100 közti érték lehet.");

				Bass.BASS_ChannelSetAttribute(this.channelID, BASSAttribute.BASS_ATTRIB_VOL, (float)value / 100);
			}
		}

		public int LeftPeak {
			get {
				if (!IsPlaying)
					return 0;

				return Utils.LowWord32(Bass.BASS_ChannelGetLevel(this.channelID));
			}
		}

		public int RightPeak {
			get {
				if (!IsPlaying)
					return 0;

				return Utils.HighWord32(Bass.BASS_ChannelGetLevel(this.channelID));
			}
		}

		public int ChannelID {
			get {
				return this.channelID;
			}
		}

		public long Position {
			get {
				return Bass.BASS_ChannelGetPosition(this.ChannelID);
			}
			set {
				if (value < 0 || value > this.Length)
					throw new ArgumentOutOfRangeException(String.Format(
						"A lejátszási pozíciónak {0} és {1} közt kell lennie.",
						0,
						this.Length
					));

				Bass.BASS_ChannelSetPosition(this.ChannelID, value);
				this.NotifyPropertyChanged("Position");
			}
		}

		public long Length {
			get {
				return Bass.BASS_ChannelGetLength(this.ChannelID);
			}
		}

		public BassPlaybackAbstract(int ChannelID) {
			if (ChannelID == 0) {
				throw new Exception("Nem sikerült megnyitni a fájlt: " + Bass.BASS_ErrorGetCode());
			}

			this.channelID = ChannelID;

			this.PlaybackTimer = new DispatcherTimer() {
				Interval = TimeSpan.FromSeconds(0.02)
			};
			this.PlaybackTimer.Tick += (o, e) => {
				if (Position >= Length) {
					this.Stop();

					if (this.SongEnded != null)
						this.SongEnded();
				}

				this.NotifyAll();
			};

			this.IsPlaying = false;
		}

		public void Play() {
			if (Bass.BASS_ChannelPlay(this.ChannelID, false)) {
				this.IsPlaying = true;
			}
		}

		public void Stop() {
			if (Bass.BASS_ChannelStop(this.ChannelID)) {
				this.IsPlaying = false;
				this.Position = 0;
				this.NotifyAll();
			}
		}

		public void Pause() {
			if (Bass.BASS_ChannelPause(this.ChannelID)) {
				this.IsPlaying = false;
				this.NotifyAll();
			}
		}

		public void Dispose() {
			if (this.IsPlaying) {
				this.Stop();
			}

			Bass.BASS_StreamFree(this.ChannelID);
		}

		private void NotifyPropertyChanged(String info) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private void NotifyAll() {
			foreach (var Property in this.GetType().GetProperties()) {
				this.NotifyPropertyChanged(Property.Name);
			}
		}
	}
}
