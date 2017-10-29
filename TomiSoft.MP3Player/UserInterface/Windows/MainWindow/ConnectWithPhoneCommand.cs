using System;
using System.Windows.Input;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.UserInterface.Windows.MainWindow {
	class ConnectWithPhoneCommand : ICommand {
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
		/// Initializes a new instance of the <see cref="ConnectWithPhoneCommand"/> class.
		/// </summary>
		/// <param name="ViewModel">
		///		The view model for the main window that is used for hiding the menu when this command is executed.
		/// </param>
		public ConnectWithPhoneCommand(MainWindowViewModel ViewModel) {
			this.ViewModel = ViewModel;
		}

		public bool CanExecute(object parameter) {
			return true;
		}

		public void Execute(object parameter) {
			this.ViewModel.MenuVisible = false;

			ApiAccessQrDialog.ApiAccessQrDialog dlg = new ApiAccessQrDialog.ApiAccessQrDialog();
			dlg.Show();
		}
	}
}
