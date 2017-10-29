using System;
using System.Windows;
using System.Windows.Input;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.UserInterface.Windows.MainWindow {
	class ShowAboutWindowCommand : ICommand {
		/// <summary>
		/// Stores the view model for the main window that is used for hiding
		/// the menu when this command is executed.
		/// </summary>
		private readonly MainWindowViewModel ViewModel;

		/// <summary>
		/// This event occures when the return value of <see cref="CanExecute"/> is changed.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Stores a reference to the dialog's parent window.
		/// </summary>
		private readonly Window ParentWindow;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShowAboutWindowCommand"/> class.
		/// </summary>
		/// <param name="ViewModel">
		///		The view model for the main window that is used for hiding the menu when this command is executed.
		/// </param>
		/// <param name="Parent">A reference to the dialog's parent window.</param>
		public ShowAboutWindowCommand(MainWindowViewModel ViewModel, Window Parent) {
			this.ViewModel = ViewModel;
			this.ParentWindow = Parent;
		}

		public bool CanExecute(object parameter) {
			return true;
		}

		public void Execute(object parameter) {
			this.ViewModel.MenuVisible = false;

			AboutWindow.AboutWindow wnd = new AboutWindow.AboutWindow() {
				Owner = this.ParentWindow
			};
			wnd.ShowDialog();
		}
	}
}
