using System;
using System.Windows;
using System.Windows.Input;

namespace TomiSoft.LyricsEditor.Commands {
    class OpenLyricsCommand : ICommand {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            MessageBox.Show("Open lyrics");
        }
    }
}
