using System;
using System.ComponentModel;
using TomiSoft.MP3Player.Playback;

namespace TomiSoft.MP3Player.UserInterface.Controls {
	class PlaybackControlViewModel : INotifyPropertyChanged, IDisposable {
		public IPlaybackManager PlaybackManager {
			get;
			private set;
		}

		public IAudioPeakMeter PeakMeter {
			get;
			private set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public PlaybackControlViewModel(IPlaybackManager PlaybackManager) {
			#region Error checking
			if (PlaybackManager == null)
				throw new ArgumentNullException(nameof(PlaybackManager));
			#endregion

			this.PlaybackManager = PlaybackManager;
			this.PlaybackManager.PropertyChanged += PlaybackManager_PropertyChanged;

			if (PlaybackManager is IAudioPeakMeter)
				this.PeakMeter = PlaybackManager as IAudioPeakMeter;
		}

		private void PlaybackManager_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			this.NotifyPropertyChanged($"PlaybackManager.{e.PropertyName}");

			if (this.PeakMeter != null) {
				this.NotifyPropertyChanged($"PeakMeter.{e.PropertyName}");
			}
		}

		private void NotifyPropertyChanged(string PropertyName) {
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}

		public void Dispose() {
			this.PlaybackManager.PropertyChanged -= PlaybackManager_PropertyChanged;
		}
	}
}
