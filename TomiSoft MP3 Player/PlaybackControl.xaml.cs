using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
			if (this.Stop != null)
				this.Stop();
		}

		private void UI_PauseButton_Click(object sender, RoutedEventArgs e) {
			if (this.Pause != null)
				this.Pause();

			UI_StopButton.IsEnabled = true;
		}

		private void UI_PlayButton_Click(object sender, RoutedEventArgs e) {
			if (this.Play != null)
				this.Play();
		}

		private void UI_PlaybackPosition_MouseUp(object sender, MouseButtonEventArgs e) {
			if (this.PositionChanged != null)
				this.PositionChanged((long)UI_PlaybackPosition.Value);
		}
	}
}
