using System.Windows;

namespace TomiSoft.MP3Player.UserInterface.Windows.ApiAccessQrDialog {
	/// <summary>
	/// Interaction logic for ApiAccessQrDialog.xaml
	/// </summary>
	public partial class ApiAccessQrDialog : Window {
		private readonly ApiAccessQrDialogViewModel ViewModel = new ApiAccessQrDialogViewModel();

		public int ConnectionID {
			get {
				return this.ViewModel.ID;
			}
		}

		public ApiAccessQrDialog() {
			InitializeComponent();

			this.DataContext = this.ViewModel;
		}
	}
}
