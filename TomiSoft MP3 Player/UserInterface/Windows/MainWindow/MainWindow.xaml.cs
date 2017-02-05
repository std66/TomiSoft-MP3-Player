using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using TomiSoft.MP3Player.Communication;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Playlist;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Windows;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using TomiSoft.MP3Player.Utils.Extensions;
using TomiSoft.MP3Player.UserInterface.Windows.AboutWindow;
using System.Linq;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;
using TomiSoft.MP3Player.Lyrics;

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
			this.viewModel = new MainWindowViewModel() {
				Playlist = this.Playlist
			};

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
			this.Playlist.SelectedSongChanged += Playlist_SelectedSongChanged;
			this.Closed += WindowClosed;

			Trace.TraceInformation("[Player startup] Startup successful.");

			this.ProcessCommandLine();
		}

		/// <summary>
		/// This method is executed after the window is closed.
		/// </summary>
		/// <param name="sender">An object's instance</param>
		/// <param name="e">Event parameters</param>
		private void WindowClosed(object sender, EventArgs e) {
			Trace.TraceInformation("[Player] Closing application...");

			BassManager.Free();
			this.Playlist.SelectedSongChanged -= this.Playlist_SelectedSongChanged;
			this.Closed -= this.WindowClosed;
			this.Loaded -= RegisterHotKeys;

			this.Hotkeys.Dispose();
			this.Server.Dispose();
		}

		/// <summary>
		/// Adds all files to the playlist that is given as command-line
		/// arguments and starts playing the first one.
		/// </summary>
		private void ProcessCommandLine() {
			string[] args = Environment.GetCommandLineArgs();
			foreach (string Filename in args) {
				if (PlaybackFactory.IsSupportedMedia(Filename))
					this.Playlist.Add(new BassSongInfo(Filename));
			}

			this.Playlist.MoveTo(0);
		}

		/// <summary>
		/// This method is executed when the selected song in playlist is
		/// changed.
		/// </summary>
		/// <param name="sender">The Playlist instance</param>
		/// <param name="e">Event parameters</param>
		private void Playlist_SelectedSongChanged(object sender, EventArgs e) {
			this.Player?.Stop();

			#region Error checking
			//If the playlist is empty, we need to attach a NULL-playback
			//manager and prevent any other actions.
			if (this.Playlist.CurrentSongInfo == null) {
				this.AttachPlayer(PlaybackFactory.NullPlayback(this.Player.Volume));
				return;
			}
			#endregion

			IPlaybackManager NewPlaybackManager = null;
			try {
				NewPlaybackManager = PlaybackFactory.LoadFile(this.Playlist.CurrentSongInfo.Source);
			}
			catch (Exception ex) {
				if (ex is IOException || ex is NotSupportedException) {
					new Toast(App.Name) {
						Title = "Hoppá...",
						Content = "Ezt a fájlt nem tudjuk megnyitni.",
						Image = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt
					}.Show();
				}

				return;
			}

			this.AttachPlayer(NewPlaybackManager);

			this.Player?.Play();

            //Load lyrics
            this.OpenLyricsAsync(this.Playlist.CurrentSongInfo);
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
				this.Hotkeys.Stop += (o, e) => this.Player?.Stop();
				this.Hotkeys.NextTrack += (o, e) => this.PlayNext();
				this.Hotkeys.PreviousTrack += (o, e) => this.PlayPrevious();
				this.Hotkeys.PlayPause += (o, e) => {
					if (this.Player.IsPlaying)
						this.Player?.Pause();
					else
						this.Player?.Play();
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
						case "Player.Play":
							this.OpenFiles(Parameters);
							break;

						case "Player.PlayNext":
							this.PlayNext();
							break;

						case "Player.PlayPrevious":
							this.PlayPrevious();
							break;

						case "Player.PlaybackPosition":
							wrt.WriteLine($"{this.Player.Position}/{this.Player.Length}");
							break;

						case "Playlist.ShowPlaylist":
							int Index = 0;
							foreach (ISongInfo Song in this.Playlist) {
								wrt.WriteLine($"{Index};{Song.Artist};{Song.Title}");
								Index++;
							}
							break;

						case "Lyrics.ShowTranslations":
							if (this.viewModel.LyricsReader != null) {
								foreach (var Translation in this.viewModel.LyricsReader.Translations) {
									wrt.WriteLine($"{Translation.Key};{Translation.Value}");
								}
							}
							break;

						case "Lyrics.UseTranslation":
							if (this.viewModel.LyricsReader != null) {
								if (this.viewModel.LyricsReader.Translations.ContainsKey(Parameters.FirstOrDefault())) {
									this.viewModel.LyricsReader.TranslationID = Parameters.FirstOrDefault();
								}
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
					this.Player?.Play();
				}
			}
		}

		/// <summary>
		/// Moves to the previous song and plays it.
		/// </summary>
		private void PlayPrevious() {
			this.Playlist.MoveToPrevious();
		}

		/// <summary>
		/// Moves to the next song and plays it.
		/// </summary>
		private void PlayNext() {
			this.Playlist.MoveToNext();
		}

		/// <summary>
		/// Asks the user to open one or more files using a file open dialog.
		/// </summary>
		/// <returns>True if the files are opened, false if not.</returns>
		private bool OpenFile() {
			OpenFileDialog dlg = new OpenFileDialog() {
				Filter = $"Minden támogatott fájl|{String.Join(";", BassManager.GetSupportedExtensions().Select(x => "*." + x))}",
				Multiselect = true
			};

			bool? DialogResult = dlg.ShowDialog();

			if (DialogResult.HasValue && DialogResult.Value == true) {
				return this.OpenFiles(dlg.FileNames);
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
			#region Error checking
			if (!BassManager.IsSupportedFile(Filename))
				return false;
			#endregion

			try {
				ISongInfo SongInfo = new BassSongInfo(Filename);

				this.Playlist.Clear();
				this.Playlist.Add(SongInfo);
				this.Playlist.MoveTo(0);

				return true;
			}
			catch (Exception e) {
				Trace.TraceWarning("Could not open file: " + e.Message);

				new Toast(App.Name) {
					Title = "Hoppá...",
					Content = "Valami miatt nem sikerült a fájlt megnyitni.",
					Image = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt
				}.Show();

				return false;
			}
		}

		/// <summary>
		/// Adds multiple files to the playlist and starts
		/// playing the first of them.
		/// </summary>
		/// <param name="Filenames">An array containing the files' path</param>
		/// <returns>True if all files are opened successfully, false if not</returns>
		private bool OpenFiles(string[] Filenames) {
			var SupportedFiles = Filenames.Where(x => BassManager.IsSupportedFile(x));

			#region Error checking
			if (SupportedFiles.Count() == 0)
				return false;
			#endregion

			try {
				this.Playlist.Clear();
				foreach (string Filename in SupportedFiles)
					this.Playlist.Add(new BassSongInfo(Filename));

				this.Playlist.MoveTo(0);
			}
			catch (Exception e) {
				Trace.TraceWarning("Could not open file: " + e.Message);

				new Toast(App.Name) {
					Title = "Hoppá...",
					Content = "Valami miatt nem sikerült a fájlokat megnyitni.",
					Image = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt
				}.Show();

				return false;
			}

			return true;
		}

		/// <summary>
		/// Searches for a matching lyrics file and loads it.
		/// </summary>
		/// <param name="SongInfo">The ISongInfo instance that holds informations about the song.</param>
		private async void OpenLyricsAsync(ISongInfo SongInfo) {
            this.viewModel.LyricsReader = await LyricsProvider.FindLyricsAsync(SongInfo);
		}

		/// <summary>
		/// Registers the events of the playback controller.
		/// </summary>
		private void PreparePlaybackController() {
			PlaybackController.NextSong += this.PlayNext;
			PlaybackController.PreviousSong += this.PlayPrevious;
		}

		/// <summary>
		/// Attaches and configures a new playback handler.
		/// </summary>
		/// <param name="Player">The playback handler to attach.</param>
		private void AttachPlayer(IPlaybackManager Player) {
			#region Error checking
			if (Player == null)
				return;
			#endregion

			int PreviousVolume = 100;

			if (this.Player != null) {
				PreviousVolume = this.Player.Volume;
				this.Player.SongEnded -= this.PlayNext;
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
			#region Error checking
			if (Player == null || Player.SongInfo == null)
				return;
			#endregion

			//We don't need to display toast when it is disabled in the config
			if (!App.Config.ToastOnSongOpen)
				return;

			System.Drawing.Image AlbumImage = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt;
			if (this.Player.SongInfo.AlbumImage != null)
				AlbumImage = this.Player.SongInfo.AlbumImage;

			new Toast(App.Name) {
				Title = this.Player.SongInfo.Title ?? "Ismeretlen szám",
				Content = this.Player.SongInfo.Artist ?? "Ismeretlen előadó",
				Image = AlbumImage
			}.Show();
		}

		/// <summary>
		/// This method is executed when one or more files were
		/// dropped to the window.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void Window_Drop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);
				this.Playlist.Clear();

				foreach (string File in Files) {
					if (BassManager.IsSupportedFile(File))
						this.Playlist.Add(new BassSongInfo(File));
				}

				if (this.Playlist.Count > 0)
					this.Playlist.MoveTo(0);
			}
		}

		/// <summary>
		/// This method is executed when the File menu item was clicked.
		/// Displays an open file dialog that helps the user
		/// to open files.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void FileOpenButton_Click(object sender, RoutedEventArgs e) {
			this.ToggleMenu(Show: false);

			this.OpenFile();
		}

		/// <summary>
		/// This method is executed when the mouse right button is pressed.
		/// Toggles the visibility of the menu.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
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

		/// <summary>
		/// This method is executed when an item in the playlist is double clicked.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void lvPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (lvPlaylist.SelectedIndex != -1) {
				this.Playlist.MoveTo(lvPlaylist.SelectedIndex);
				this.ToggleMenu(Show: false);
			}
		}
	}
}
