using System;
using System.Windows;
using System.Windows.Input;

namespace TomiSoft.LyricsEditor.Commands {
    class NewLyricsCommand : ICommand {
        public event EventHandler CanExecuteChanged {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            MessageBox.Show("New lyrics");
        }
    }
}
