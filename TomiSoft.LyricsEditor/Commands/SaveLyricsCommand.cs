using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TomiSoft.LyricsEditor.Commands {
    class SaveLyricsCommand : ICommand {
        public event EventHandler CanExecuteChanged {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            MessageBox.Show("Save lyrics");
        }
    }
}
