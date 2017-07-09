using System;
using System.Windows;
using TomiSoft.MP3Player.Utils;

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

        public ProgressBarDialog(string Title, string Text, Progress<LongOperationProgress> Progress)
            : this (Title, Text) {
            Progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object sender, LongOperationProgress e) {
            UI_ProgressBar.IsIndeterminate = e.IsIndetermine;
            UI_ProgressBar.Maximum = e.Maximum;
            UI_ProgressBar.Value = e.Position;
        }
    }
}
