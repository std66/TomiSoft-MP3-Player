using System;
using System.ComponentModel;
using System.Windows.Threading;
using TomiSoft.LyricsEditor.Commands;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.ViewModels {
    class MainWindowViewModel : INotifyPropertyChanged {
        private readonly ExitApplicationCommand exitApplicationCommand = new ExitApplicationCommand();
        private readonly NewLyricsCommand newLyricsCommand = new NewLyricsCommand();
        private readonly OpenLyricsCommand openLyricsCommand = new OpenLyricsCommand();
        private readonly SaveLyricsCommand saveLyricsCommand = new SaveLyricsCommand();
        private readonly PlayCommand playCommand;
        private readonly PauseCommand pauseCommand;
        private readonly StopCommand stopCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public ExitApplicationCommand ExitApplicationCommand {
            get {
                return exitApplicationCommand;
            }
        }

        public NewLyricsCommand NewLyricsCommand {
            get {
                return newLyricsCommand;
            }
        }

        public OpenLyricsCommand OpenLyricsCommand {
            get {
                return openLyricsCommand;
            }
        }

        public SaveLyricsCommand SaveLyricsCommand {
            get {
                return saveLyricsCommand;
            }
        }

        public PlayCommand PlayCommand {
            get {
                return playCommand;
            }
        }

        public PauseCommand PauseCommand {
            get {
                return pauseCommand;
            }
        }

        public StopCommand StopCommand {
            get {
                return stopCommand;
            }
        }

        public PlaybackStatus Playback {
            get;
            private set;
        }
        
        public MainWindowViewModel() {
            
        }

        public MainWindowViewModel(PlayerClient Client) {
            this.Playback = new PlaybackStatus(Client);

            this.playCommand = new PlayCommand(Client);
            this.pauseCommand = new PauseCommand(Client);
            this.stopCommand = new StopCommand(Client);
        }

        private void Update(object sender, EventArgs e) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Playback)));
        }
    }
}
