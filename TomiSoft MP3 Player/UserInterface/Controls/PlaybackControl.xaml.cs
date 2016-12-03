using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TomiSoft.MP3Player.Playback;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for PlaybackControl.xaml
	/// </summary>
	public partial class PlaybackControl : UserControl {
		private IPlaybackManager player;

		public event Action Stop;
		public event Action Play;
		public event Action Pause;
		public event Action<long> PositionChanged;

		public IPlaybackManager Player {
			get {
				return this.player;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");

				this.player = value;
				this.DataContext = this.player;

				this.Player.PropertyChanged += (o, e) => {
					if (e.PropertyName == "IsPlaying") {
						this.TogglePlaybackButtons(this.player.IsPlaying);
					}
				};
			}
		}
		
		public PlaybackControl() {
			InitializeComponent();
		}

		private void TogglePlaybackButtons(bool IsPlaying) {
			UI_PlayButton.IsEnabled = !IsPlaying;
			UI_PauseButton.IsEnabled = IsPlaying;
			UI_StopButton.IsEnabled = IsPlaying;
		}

		private void UI_StopButton_Click(object sender, RoutedEventArgs e) {
			this.Stop?.Invoke();
		}

		private void UI_PauseButton_Click(object sender, RoutedEventArgs e) {
			this.Pause?.Invoke();

			UI_StopButton.IsEnabled = true;
		}

		private void UI_PlayButton_Click(object sender, RoutedEventArgs e) {
			this.Play?.Invoke();
		}

		private void UI_PlaybackPosition_MouseUp(object sender, MouseButtonEventArgs e) {
			this.PositionChanged?.Invoke((long)UI_PlaybackPosition.Value);
		}
	}
}
