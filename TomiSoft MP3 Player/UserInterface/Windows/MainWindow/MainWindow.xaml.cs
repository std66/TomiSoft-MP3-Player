using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using Un4seen.Bass;
using TomiSoft.Music.Lyrics.Lrc;
using TomiSoft.MP3Player.Communication;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Playlist;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Windows;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using TomiSoft.MP3Player.Utils.Extensions;
using System.Reflection;
using TomiSoft.MP3Player.UserInterface.Windows.AboutWindow;

namespace TomiSoft_MP3_Player {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private IPlaybackManager Player;
        private Playlist Playlist = new Playlist();
        private PlayerServer Server;
        private PlaybackHotkeys Hotkeys;
        public MainWindowViewModel viewModel;
        private bool MenuShowing = false;

        public MainWindow() {
            this.StartServer();
            
            Trace.TraceInformation("[Player startup] Preparing main window to display...");

            InitializeComponent();
            this.viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            this.PreparePlaybackController();
            this.Icon = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt.ToImageSource();

            //Attaching a null-playback instance to display default informations.
            Trace.TraceInformation("[Player startup] Attaching a NULL-playback manager");
            this.AttachPlayer(
                PlaybackFactory.NullPlayback(100)
            );

            //Load BASS with its all plugins.
            Trace.TraceInformation("[Player startup] Initializing BASS library...");
            if (!BassManager.Load()) {
                Trace.TraceError("[Player startup] Fatal error occured. Terminating application...");
                PlayerUtils.ErrorMessageBox(App.Name, "Nem sikerült betölteni a BASS-t.");
                Environment.Exit(1);
            }

            //Initialize BASS output device.
            Trace.TraceInformation("[Player startup] Initializing audio output device");
            if (!BassManager.InitializeOutputDevice()) {
                Trace.TraceError("[Player startup] Fatal error occured. Terminating application...");
                PlayerUtils.ErrorMessageBox(App.Name, "Nem sikerült beállítani a hangkimenetet.");
                Environment.Exit(1);
            }

            this.Loaded += RegisterHotKeys;

            this.Closed += (o, e) => {
                Trace.TraceInformation("[Player] Closing application...");
                BassManager.Free();
                this.Hotkeys.Dispose();
            };

            Trace.TraceInformation("[Player startup] Startup successful.");
        }

        /// <summary>
        /// This event handler method registers the hotkeys.
        /// </summary>
        /// <param name="sender">Always null</param>
        /// <param name="ev">Empty EventArgs instance</param>
        private void RegisterHotKeys(object sender, EventArgs ev) {
            //Register hotkeys
            this.Hotkeys = new PlaybackHotkeys(this);
            if (!Hotkeys.Registered) {
                if (App.Config.ToastOnMediaKeysFault) {
                    Toast t = new Toast(App.Name) {
                        Title = "Hoppá!",
                        Content = "Úgy tűnik, hogy a billentyűzet médiabillentyűit már más program használja.",
                        Image = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt
                    };

                    t.Show();
                }
            }
            else {
                this.Hotkeys.Stop += (o, e) => this.Stop();
                this.Hotkeys.NextTrack += (o, e) => this.PlayNext();
                this.Hotkeys.PreviousTrack += (o, e) => this.PlayPrevious();
                this.Hotkeys.PlayPause += (o, e) => {
                    if (this.Player.IsPlaying)
                        this.Pause();
                    else
                        this.Play();
                };
            }
        }
        
        /// <summary>
        /// Starts a TCP server to listen to specific commands.
        /// </summary>
        private void StartServer() {
            this.Server = new PlayerServer();
            this.Closed += (o, e) => {
                this.Server.Dispose();
            };

            Server.CommandReceived += (ClientStream, Command, Parameters) => {
                Dispatcher.Invoke((Action<Stream, string, string[]>)delegate {
                    StreamWriter wrt = new StreamWriter(ClientStream) {
                        AutoFlush = true
                    };

                    Trace.TraceInformation($"[Server] Command={Command}");

                    switch (Command) {
                        case "Play":
                            this.OpenFiles(Parameters);
                            break;

                        case "PlayNext":
                            this.PlayNext();
                            break;

                        case "PlayPrevious":
                            this.PlayPrevious();
                            break;

                        case "ShowPlaylist":
                            int Index = 0;
                            foreach (SongInfo Song in this.Playlist) {
                                wrt.WriteLine($"{Index};{Song.Artist};{Song.Title}");
                                Index++;
                            }
                            break;

                        default:
                            Trace.TraceWarning("[Server] Unrecognized command");
                            break;
                    }
                }, ClientStream, Command, Parameters);
            };
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            //When CTRL+O is pressed, a file open dialog appears that lets the user to load a file.
            if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                if (this.OpenFile()) {
                    this.PlayerOperaion(() => this.Player.Play());
                }
            }
        }

        /// <summary>
        /// Moves to the previous song and plays it.
        /// </summary>
        private void PlayPrevious() {
            if (this.Playlist.MoveToPrevious()) {
                this.PlayerOperaion(() => this.Player.Stop());

                this.AttachPlayer(
                    PlaybackFactory.LoadFile(this.Playlist.CurrentSongInfo.Source)
                );

                this.Play();
            }
        }

        /// <summary>
        /// Moves to the next song and plays it.
        /// </summary>
        private void PlayNext() {
            if (this.Playlist.MoveToNext()) {
                this.PlayerOperaion(() => this.Player.Stop());

                this.AttachPlayer(
                    PlaybackFactory.LoadFile(this.Playlist.CurrentSongInfo.Source)
                );

                this.Play();
            }
        }

        /// <summary>
        /// Asks the user to open a file using a file open dialog.
        /// </summary>
        /// <returns>True if the file is opened, false if not.</returns>
        private bool OpenFile() {
            OpenFileDialog dlg = new OpenFileDialog();
            bool? DialogResult = dlg.ShowDialog();
            if (DialogResult.HasValue && DialogResult.Value == true) {
                return this.OpenFile(dlg.FileName);
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Clears the current playlist, then adds the given file to it. Also,
        /// tries to load the lyrics file if exists.
        /// </summary>
        /// <param name="Filename">The file's name to load.</param>
        /// <returns>True if the file is loaded, false if not.</returns>
        private bool OpenFile(string Filename) {
            try {
                this.Playlist.Clear();
                this.Playlist.Add(new SongInfo(Filename));

                this.AttachPlayer(
                    PlaybackFactory.LoadFile(Filename)
                );

                //Load lyrics
                string LyricsFile = Path.ChangeExtension(Filename, "lrc");
                this.OpenLyrics(LyricsFile);

                return true;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private bool OpenFiles(string[] Filenames) {
            try {
                this.Playlist.Clear();
                foreach (string Filename in Filenames)
                    this.Playlist.Add(new SongInfo(Filename));

                this.Playlist.MoveTo(0);

                this.AttachPlayer(
                    PlaybackFactory.LoadFile(this.Playlist.CurrentSongInfo.Source)
                );

                this.Play();
            }
            catch (Exception e) {
                PlayerUtils.ErrorMessageBox(App.Name, e.Message);
                return false;
            }

            return true;
        }

        private void OpenLyrics(string Filename) {
            if (File.Exists(Filename)) {
                this.viewModel.LyricsReader = new LrcReader(Filename);
            }
            else {
                this.viewModel.LyricsReader = null;
            }
        }

        /// <summary>
        /// Registers the events of the playback controller.
        /// </summary>
        private void PreparePlaybackController() {
            PlaybackController.Play += this.Play;
            PlaybackController.Pause += this.Pause;
            PlaybackController.Stop += this.Stop;

            PlaybackController.PositionChanged += (value) => {
                this.PlayerOperaion(() => this.Player.Position = value);
            };
        }

        /// <summary>
        /// Attaches and configures a new playback handler.
        /// </summary>
        /// <param name="Player">The playback handler to attach.</param>
        private void AttachPlayer(IPlaybackManager Player) {
            if (Player == null)
                throw new ArgumentNullException("Player");

            int PreviousVolume = 100;

            if (this.Player != null) {
                PreviousVolume = this.Player.Volume;
                this.Player.Dispose();
            }

            this.Player = Player;
            this.viewModel.PlaybackManager = Player;
            this.Player.Volume = PreviousVolume;
            PlaybackController.Player = this.Player;

            this.Player.SongEnded += this.PlayNext;

            //Send toast notification
            ShowToast(Player);
        }

        /// <summary>
        /// Shows toast notification.
        /// </summary>
        /// <param name="Player">The IPlaybackManager instance which holds the song's information.</param>
        private void ShowToast(IPlaybackManager Player) {
            //We don't need to display toast when it is disabled in the config
            if (!App.Config.ToastOnSongOpen)
                return;

            System.Drawing.Image AlbumImage = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt;
            if (Player.SongInfo != null) {
                AlbumImage = this.Player.SongInfo.AlbumImage;
                if (AlbumImage == null)
                    AlbumImage = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt;

                Toast t = new Toast(App.Name) {
                    Title = this.Player.SongInfo.Title,
                    Content = this.Player.SongInfo.Artist,
                    Image = AlbumImage
                };
                t.Show();
            }
        }

        /// <summary>
        /// Provides a safe way to control the playback handler.
        /// </summary>
        /// <param name="Operaion">The function to execute.</param>
        private void PlayerOperaion(Action Operaion) {
            if (this.Player != null && Operaion != null) {
                Operaion();
            }
        }

        /// <summary>
        /// Plays the current song.
        /// </summary>
        public void Play() {
            this.PlayerOperaion(() => this.Player.Play());
        }

        /// <summary>
        /// Stops the current song.
        /// </summary>
        public void Stop() {
            this.PlayerOperaion(() => this.Player.Stop());
        }

        /// <summary>
        /// Pauses the current song.
        /// </summary>
        public void Pause() {
            this.PlayerOperaion(() => this.Player.Pause());
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (this.OpenFile(Files[0])) {
                    this.Play();
                }
            }
        }

        private void FileOpenButton_Click(object sender, RoutedEventArgs e) {
            this.ToggleMenu(Show: false);

            if (this.OpenFile()) {
                this.PlayerOperaion(() => this.Player.Play());
            }
        }

        private void ExitClicked(object sender, MouseButtonEventArgs e) {
            Environment.Exit(0);
        }

        private void ToggleMenuVisibility(object sender, MouseButtonEventArgs e) {
            bool ShowMenu = !this.MenuShowing;
            this.ToggleMenu(ShowMenu);
        }

        /// <summary>
        /// Changes the visibility of the menu.
        /// </summary>
        /// <param name="Show">Set to true if you want to show the menu, set to false to hide it</param>
        private void ToggleMenu(bool Show) {
            if (Show) {
                (this.Resources["FadeInAnimation"] as Storyboard)?.Begin();
            }
            else {
                (this.Resources["FadeOutAnimation"] as Storyboard)?.Begin();
            }

            this.MenuShowing = Show;
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(this.Hotkeys.Hook);
        }

        /// <summary>
        /// This method is executed when the user clicks the "About"
        /// in the menu. Closes the menu and shows the about window.
        /// </summary>
        /// <param name="sender">The "About" label's instance</param>
        /// <param name="e">Event parameters</param>
        private void AboutClicked(object sender, MouseButtonEventArgs e) {
            this.ToggleMenu(Show: false);

            AboutWindow wnd = new AboutWindow() {
                Owner = this
            };
            wnd.ShowDialog();
        }
    }
}
