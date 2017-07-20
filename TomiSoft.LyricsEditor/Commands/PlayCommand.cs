using System;
using System.Windows.Input;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor.Commands {
    class PlayCommand : ICommand {
        private readonly PlayerClient Client;

        public event EventHandler CanExecuteChanged;

        public PlayCommand(PlayerClient Client) {
            this.Client = Client;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            this.Client.Playback.Play();
        }
    }
}
