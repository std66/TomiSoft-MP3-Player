using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TomiSoft.MP3Player.UserInterface.Windows.TextInputDialog {
	/// <summary>
	/// Interaction logic for TextInputDialog.xaml
	/// </summary>
	public partial class TextInputDialog : Window {
		public string UserInput {
			get {
				return this.UI_TextBox.Text;
			}
		}

		public TextInputDialog() {
			InitializeComponent();
			this.Initialized += (o, e) => this.DialogResult = false;
		}

		public TextInputDialog(string Title, string Text)
			: this() {
			this.Title = Title;
			this.UI_Text.Content = Text;
		}

		public TextInputDialog(string Title, string Text, string OkButtonText, string CancelButtonText)
			: this(Title, Text) {
			this.btnOk.Content = OkButtonText;
			this.btnCancel.Content = CancelButtonText;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
			this.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
