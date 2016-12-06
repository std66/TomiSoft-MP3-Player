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
			//If an instance is already running, send the file list to it.
			//Otherwise, this is the only running instance, so we start a server to
			//listen to the other instances.
			Trace.TraceInformation("[Player startup] Checking if an instance is already running...");
			if (PlayerClient.IsServerRunning()) {
				Trace.TraceInformation("[Player startup] Found a running instance");
				this.SendFileListToServer();
			}
			else {
				Trace.TraceInformation("[Player startup] No other instances are running.");
				this.StartServer();
			}

			Trace.TraceInformation("[Player startup] Preparing main window to display...");

			InitializeComponent();
			this.viewModel = new MainWindowViewModel();
			this.DataContext = viewModel;
			this.PreparePlaybackController();

			//Attaching a null-playback instance to display default informations.
			Trace.TraceInformation("[Player startup] Attaching a NULL-playback manager");
			this.AttachPlayer(	
				PlaybackFactory.NullPlayback(100)
			);

			//Load BASS with its all plugins.
			Trace.TraceInformation("[Player startup] Initializing BASS library...");
			if (!BassManager.Load()) {
				Trace.TraceError("[Player startup] Fatal error occured. Terminating application...");
				PlayerUtils.ErrorMessageBox("TomiSoft MP3 Player", "Nem sikerült betölteni a BASS-t.");
				Environment.Exit(1);
			}

            this.Loaded += RegisterHotKeys;

			this.Closed += (o, e) => {
				Trace.TraceInformation("[Player] Closing application...");
				Trace.TraceInformation("[BASS] Free");
				Bass.BASS_Free();
                this.Hotkeys.Dispose();
			};
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
                Toast t = new Toast("TomiSoft MP3 Player") {
                    Title = "Hoppá!",
                    Content = "Úgy tűnik, hogy a billentyűzet médiabillentyűit már más program használja."
                };
            }

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

        /// <summary>
        /// Sends the file list to the server then closes the application.
        /// Currently only the first file is sent.
        /// </summary>
        private void SendFileListToServer() {
			try {
				using (PlayerClient Client = new PlayerClient()) {
					string[] args = Environment.GetCommandLineArgs();

					if (args.Length > 1)
						Client.Play(args[1]);
				}
			}
			catch (Exception e) {
				PlayerUtils.ErrorMessageBox("TomiSoft MP3 Player", e.Message);
			}

			Environment.Exit(0);
		}

		/// <summary>
		/// Starts a TCP server to listen to specific commands.
		/// </summary>
		private void StartServer() {
			this.Server = new PlayerServer();
			this.Closed += (o, e) => {
				this.Server.Dispose();
			};

			Server.CommandReceived += (ClientStream, Command, Parameter) => {
				Dispatcher.Invoke((Action<Stream, string, string>)delegate {
					StreamWriter wrt = new StreamWriter(ClientStream);

					Trace.TraceInformation($"[Server] Command={Command}; Parameter={Parameter}");

					switch (Command) {
						case "Play":
							this.OpenFile(Parameter);
							this.PlayerOperaion(() => this.Player.Play());
							break;

						case "IsRunning":
							wrt.WriteLine("true");
							wrt.Flush();
							break;

						default:
							Trace.TraceWarning("[Server] Unrecognized command");
							break;
					}
				}, ClientStream, Command, Parameter);
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

				Toast t = new Toast("TomiSoft MP3 Player") {
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
    }
}
