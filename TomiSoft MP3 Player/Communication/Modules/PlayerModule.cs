using System;
using TomiSoft.MP3Player.Playback;

namespace TomiSoft.MP3Player.Communication.Modules {
	public class PlayerModule : IServerModule {
		private IPlaybackManager Playback;
		private IAudioPeakMeter PeakMeter;

		public event EventHandler<string[]> OpenFiles;
		public event EventHandler NextSong;
		public event EventHandler PreviousSong;
		
		public string ModuleName {
			get {
				return "Player";
			}
		}

		public IPlaybackManager PlaybackManager {
			get {
				return this.Playback;
			}
			set {
				this.Playback = value;
				this.PeakMeter = value as IAudioPeakMeter;
			}
		}

		[ServerCommand]
		public string PeakLevel() {
			return $"{PeakMeter?.LeftPeak ?? 0}/{PeakMeter?.RightPeak ?? 0}";
		}

		[ServerCommand]
		public int MaximumPeakLevel() {
			return 32768;
		}

		[ServerCommand]
		public string PlaybackPosition() {
			return $"{this.Playback?.Position ?? 0}/{this.Playback?.Length ?? 0}";
		}

		[ServerCommand]
		public void Play(params string[] Files) {
			this.OpenFiles?.Invoke(this, Files);
		}

		[ServerCommand]
		public void PlayNext() {
			this.NextSong?.Invoke(this, EventArgs.Empty);
		}

		[ServerCommand]
		public void PlayPrevious() {
			this.PreviousSong?.Invoke(this, EventArgs.Empty);
		}
	}
}
