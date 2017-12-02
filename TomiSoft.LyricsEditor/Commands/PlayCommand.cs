using System;
using System.Windows.Input;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.Commands {
    class PlayCommand : ICommand {
        private readonly PlayerClient Client;

        public event EventHandler CanExecuteChanged {
            add { }
            remove { }
        }

        public PlayCommand(PlayerClient Client) {
            this.Client = Client;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public async void Execute(object parameter) {
            await this.Client.Playback.PlayAsync();
        }
    }
}
