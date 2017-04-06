using System;
using System.Windows.Input;
using Un4seen.Bass;

namespace TomiSoft.MP3Player.UserInterface.Windows.AboutWindow {
    /// <summary>
    /// Represents the action for clicking "About BASS" button.
    /// </summary>
    internal class AboutBassCommand : ICommand {
        /// <summary>
        /// This event is fired when the CanExecute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged {
			add { }
			remove { }
		}

        /// <summary>
        /// Returns that the button should be enabled.
        /// </summary>
        /// <param name="parameter">A parameter.</param>
        /// <returns>Always returns true</returns>
        public bool CanExecute(object parameter) {
            return true;
        }

        /// <summary>
        /// This method is executed when the button is clicked.
        /// </summary>
        /// <param name="parameter">I don't know what it is...</param>
        public void Execute(object parameter) {
            BassNet.ShowAbout(null);
        }
    }
}
