using System;
using System.ComponentModel;
using System.Windows.Threading;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.ViewModels {
    class PlaybackStatus : INotifyPropertyChanged {
        private readonly DispatcherTimer Timer;
        private readonly PlayerClient Client;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Position {
            get {
                return this.Client.Playback.PlaybackPosition;
            }
            set {
                this.Client.Playback.PlaybackPosition = value;
            }
        }

        public double Length {
            get {
                return this.Client.Playback.SongLength;
            }
        }

        public PlaybackStatus(PlayerClient Client) {
            this.Client = Client;

            this.Timer = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(50),
                IsEnabled = true
            };

            this.Timer.Tick += Update;
        }

        private void Update(object sender, EventArgs e) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
        }
    }
}
