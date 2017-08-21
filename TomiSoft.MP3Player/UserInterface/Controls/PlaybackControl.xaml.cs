using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.UserInterface.Controls;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for PlaybackControl.xaml
	/// </summary>
	public partial class PlaybackControl : UserControl {
		private IPlaybackManager player;
		
        public event Action PreviousSong;
        public event Action NextSong;

		private int VolumeBeforeMute = 100;

		public IPlaybackManager Player {
			get {
				return this.player;
			}
			set {
				#region Error checking
				if (value == null)
					throw new ArgumentNullException("value");
				#endregion

				#region Cleanup
				if (this.DataContext != null) {
					(this.DataContext as IDisposable)?.Dispose();
				}
				#endregion

				this.player = value;
				this.DataContext = new PlaybackControlViewModel(value);

				this.Player.PropertyChanged += (o, e) => {
                    this.Dispatcher.Invoke(() => UpdatePlaybackButtons(e.PropertyName));
                };
			}
		}

        private void UpdatePlaybackButtons(string PropertyName)
        {
            switch (PropertyName)
            {
                case "IsPlaying":
                    UI_PlayButton.IsEnabled = !this.player.IsPlaying;
                    UI_PauseButton.IsEnabled = this.player.IsPlaying;
                    UI_StopButton.IsEnabled = this.player.IsPlaying;
                    break;

                case "IsPaused":
                    this.UI_StopButton.IsEnabled = this.player.IsPaused || this.player.IsPlaying;
                    break;
            }
        }
		
		public PlaybackControl() {
			InitializeComponent();
		}

		private void UI_StopButton_Click(object sender, RoutedEventArgs e) {
			this.player?.Stop();
		}

		private void UI_PauseButton_Click(object sender, RoutedEventArgs e) {
			this.player?.Pause();

			UI_StopButton.IsEnabled = true;
		}

		private void UI_PlayButton_Click(object sender, RoutedEventArgs e) {
			this.Player?.Play();
		}

		private void UI_PlaybackPosition_MouseUp(object sender, MouseButtonEventArgs e) {
			if (this.player != null)
				this.player.Position = (long)UI_PlaybackPosition.Value;
		}

		private void UI_MuteButton_Click(object sender, RoutedEventArgs e) {
			if (this.player.Volume > 0) {
				this.VolumeBeforeMute = this.player.Volume;
				this.player.Volume = 0;
			}
			else {
				this.player.Volume = this.VolumeBeforeMute;
			}
		}

        private void UI_PreviousSong_Click(object sender, RoutedEventArgs e) {
            this.PreviousSong?.Invoke();
        }

        private void UI_NextSong_Click(object sender, RoutedEventArgs e) {
            this.NextSong?.Invoke();
        }
    }
}
