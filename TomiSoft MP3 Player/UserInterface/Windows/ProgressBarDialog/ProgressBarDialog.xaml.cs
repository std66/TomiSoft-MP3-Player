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

namespace TomiSoft.MP3Player.UserInterface.Windows.ProgressBarDialog {
	/// <summary>
	/// Interaction logic for ProgressBarDialog.xaml
	/// </summary>
	public partial class ProgressBarDialog : Window {
		public ProgressBarDialog() {
			InitializeComponent();
		}

		public ProgressBarDialog(string Title, string Text)
			: this(){
			this.Title = Title;
			this.UI_Text.Content = Text;
		}
	}
}
