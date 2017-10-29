using System;
using System.Windows.Input;

namespace TomiSoft.MP3Player.UserInterface.Windows.MainWindow {
	class ConnectWithPhoneCommand : ICommand {
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) {
			return true;
		}

		public void Execute(object parameter) {
			ApiAccessQrDialog.ApiAccessQrDialog dlg = new ApiAccessQrDialog.ApiAccessQrDialog();
			dlg.Show();
		}
	}
}
