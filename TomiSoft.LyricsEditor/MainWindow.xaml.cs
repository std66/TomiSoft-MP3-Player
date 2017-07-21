using System.Windows;
using TomiSoft.LyricsEditor.ViewModels;
using TomiSoft.MP3Player.Communication;

namespace TomiSoft.LyricsEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MainWindowViewModel ViewModel;
        private readonly PlayerClient Client;

        public MainWindow() {
            InitializeComponent();

            this.Closed += (o, e) => this.Client.Dispose();

            this.Client = new PlayerClient();
            this.Client.KeepAlive();

            this.ViewModel = new MainWindowViewModel(this.Client);
            this.DataContext = this.ViewModel;
        }
    }
}
