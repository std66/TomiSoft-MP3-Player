using System.ComponentModel;
using System.Windows.Input;
using TomiSoft.LyricsEditor.Commands;
using TomiSoft.MP3Player.Common.MediaInformation;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.ViewModels {
    class MainWindowViewModel : INotifyPropertyChanged {
        private readonly PlayerClient Client;
        private ISongMetadata songMetadata = new SongMetadata("Sample title", "Sample artist", "Sample album");

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ExitApplicationCommand { get; } = new ExitApplicationCommand();
        public ICommand NewLyricsCommand       { get; } = new NewLyricsCommand();
        public ICommand OpenLyricsCommand      { get; } = new OpenLyricsCommand();
        public ICommand SaveLyricsCommand      { get; } = new SaveLyricsCommand();
        public ICommand PlayCommand            { get; }
        public ICommand PauseCommand           { get; }
        public ICommand StopCommand            { get; }

        public PlaybackStatus Playback {
            get;
            private set;
        }

        public ISongMetadata SongMetadata {
            get {
                return this.songMetadata;
            }
            set {
                this.songMetadata = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SongMetadata)));
            }
        }
        
        public MainWindowViewModel() {
            
        }

        public MainWindowViewModel(PlayerClient Client) {
            this.Client = Client;

            this.SongMetadata = this.Client.Playlist.CurrentlyPlaying;

            this.Playback = new PlaybackStatus(Client);

            this.PlayCommand  = new PlayCommand(Client);
            this.PauseCommand = new PauseCommand(Client);
            this.StopCommand  = new StopCommand(Client);
        }
    }
}
