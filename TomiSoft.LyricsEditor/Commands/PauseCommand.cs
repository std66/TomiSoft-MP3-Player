using System;
using System.Windows.Input;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.Commands {
    class PauseCommand : ICommand {
        private readonly PlayerClient Client;

        public event EventHandler CanExecuteChanged {
            add { }
            remove { }
        }

        public PauseCommand(PlayerClient Client) {
            this.Client = Client;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            this.Client.Playback.Pause();
        }
    }
}
