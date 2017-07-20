using System;
using System.Windows.Input;

namespace TomiSoft.LyricsEditor.Commands {
    /// <summary>
    /// This command closes the application.
    /// </summary>
    class ExitApplicationCommand : ICommand {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            Environment.Exit(0);
        }
    }
}
