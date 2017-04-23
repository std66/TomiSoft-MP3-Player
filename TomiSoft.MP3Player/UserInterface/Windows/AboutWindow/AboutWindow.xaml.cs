using System.Windows;

namespace TomiSoft.MP3Player.UserInterface.Windows.AboutWindow {
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        public AboutWindow() {
            InitializeComponent();
            this.DataContext = new AboutWindowViewModel();
        }
    }
}
