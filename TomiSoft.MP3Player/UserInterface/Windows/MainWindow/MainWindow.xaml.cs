using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using TomiSoft.MP3Player.Communication;
using TomiSoft.MP3Player.Communication.Modules;
using TomiSoft.MP3Player.Lyrics;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Playback.BASS;
using TomiSoft.MP3Player.Playback.YouTube;
using TomiSoft.MP3Player.Playlist;
using TomiSoft.MP3Player.UserInterface.Windows.AboutWindow;
using TomiSoft.MP3Player.UserInterface.Windows.ProgressBarDialog;
using TomiSoft.MP3Player.UserInterface.Windows.TextInputDialog;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Extensions;
using TomiSoft.MP3Player.Utils.Windows;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private IPlaybackManager player;
		private readonly Playlist Playlist = new Playlist();
		private PlaybackHotkeys Hotkeys;

		private PlayerServer Server;
		private readonly LyricsModule LyricsServerModule = new LyricsModule();
		private readonly PlayerModule PlayerServerModule = new PlayerModule();

		public MainWindowViewModel viewModel;
		
		/// <summary>
		/// Gets or sets the playback manager that handles the playback procedure.
		/// </summary>
		private IPlaybackManager Player {
			get {
				return this.player;
			}
			set {
				this.player = value;
				this.PlayerServerModule.PlaybackManager = value;
			}
		}
		
		public MainWindow() {
			this.StartServer();

			Trace.TraceInformation("[Player startup] Preparing main window to display...");
			
			InitializeComponent();
			Menu.Opacity = 0;

			this.viewModel = new MainWindowViewModel(this.Resources) {
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
			PlaybackFactory.MediaOpenProgressChanged += (Percentage, StatusString) => {
				this.viewModel.Lyrics = StatusString;
			};

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
		private async void Playlist_SelectedSongChanged(object sender, EventArgs e) {
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
				this.viewModel.LyricsReader = null;
				this.viewModel.Lyrics = "Zene megnyitása...";

				NewPlaybackManager = await PlaybackFactory.LoadMedia(this.Playlist.CurrentSongInfo);
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

			this.Server.AttachModule(
				new SoftwareModule(),
				new PlaylistModule(this.Playlist),
				this.LyricsServerModule,
				this.PlayerServerModule
			);

			this.PlayerServerModule.OpenFiles    += (o, e) => this.Dispatcher.Invoke(() => this.OpenFilesAsync(e));
			this.PlayerServerModule.NextSong     += (o, e) => this.Dispatcher.Invoke(this.PlayNext);
			this.PlayerServerModule.PreviousSong += (o, e) => this.Dispatcher.Invoke(this.PlayPrevious);
		}

		private async void Window_KeyUpAsync(object sender, KeyEventArgs e) {
			//When CTRL+O is pressed, a file open dialog appears that lets the user to load a file.
			if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
				if (await this.OpenFileAsync()) {
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
		private async Task<bool> OpenFileAsync() {
			OpenFileDialog dlg = new OpenFileDialog() {
				Filter = $"Minden támogatott fájl|{String.Join(";", BassManager.GetSupportedExtensions().Select(x => "*." + x))}",
				Multiselect = true
			};

			bool? DialogResult = dlg.ShowDialog();

			if (DialogResult.HasValue && DialogResult.Value == true) {
				return await this.OpenFilesAsync(dlg.FileNames);
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
			if (!PlaybackFactory.IsSupportedMedia(Filename))
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
		private async Task<bool> OpenFilesAsync(string[] Filenames) {
			var SupportedFiles = Filenames.Where(x => PlaybackFactory.IsSupportedMedia(x));

			#region Error checking
			if (SupportedFiles.Count() == 0)
				return false;
			#endregion

			try {
				this.Playlist.Clear();
				foreach (string Filename in SupportedFiles) {
					if (YoutubeUri.IsValidYoutubeUri(Filename)) {
						this.viewModel.LyricsReader = null;
						this.viewModel.Lyrics = "Előkészülünk a letöltéshez...";
						this.Playlist.Add(await YoutubeSongInfo.GetVideoInfoAsync(Filename));
					}
					else
						this.Playlist.Add(new BassSongInfo(Filename));
				}

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
			this.viewModel.LyricsReader = null;
			this.viewModel.Lyrics = "Dalszöveget keresünk...";
			this.viewModel.LyricsReader = await LyricsProvider.FindLyricsAsync(SongInfo);

			this.LyricsServerModule.LyricsReader = this.viewModel.LyricsReader;
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
					if (PlaybackFactory.IsSupportedMedia(File))
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
		private async void FileOpenButton_Click(object sender, RoutedEventArgs e) {
			this.viewModel.MenuVisible = false;

			await this.OpenFileAsync();
		}

		/// <summary>
		/// This method is executed when the mouse right button is pressed.
		/// Toggles the visibility of the menu.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void ToggleMenuVisibility(object sender, MouseButtonEventArgs e) {
			this.viewModel.MenuVisible = !this.viewModel.MenuVisible;
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
			this.viewModel.MenuVisible = false;

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
			#region Error checking
			if (lvPlaylist.SelectedIndex == -1 || e.ChangedButton != MouseButton.Left) {
				return;
			}
			#endregion

			this.Playlist.MoveTo(lvPlaylist.SelectedIndex);
			this.viewModel.MenuVisible = false;
		}

		/// <summary>
		/// This method can be executed when a <see cref="System.Windows.Controls.Control"/> is clicked.
		/// Hides the menu.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void HideMenu(object sender, MouseButtonEventArgs e) {
			this.viewModel.MenuVisible = false;
		}

		/// <summary>
		/// This method is executed when the user wants to play a media from an URI source.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private async void UriOpen_Click(object sender, MouseButtonEventArgs e) {
			this.viewModel.MenuVisible = false;

			#region Check requirements
			if (!YoutubePlayback.ToolsAvailable) {
				MessageBoxResult DownloadQuestion = MessageBox.Show(
					messageBoxText: "Első alkalommal le kell tölteni az ffmpeg.exe, az ffprobe.exe és a youtube-dl.exe programokat. Szeretnéd most letölteni?",
					caption: "Kellene még néhány dolog...",
					button: MessageBoxButton.YesNo,
					icon: MessageBoxImage.Question
				);

				if (DownloadQuestion == MessageBoxResult.Yes) {
					ProgressBarDialog ProgressBar = new ProgressBarDialog("YouTube eszközök letöltése", "Fél perc és kész vagyunk...");
					ProgressBar.Show();

					await YoutubePlayback.DownloadSoftwareAsync();

					ProgressBar.Close();
				}
				else {
					return;
				}
			}
			#endregion

			TextInputDialog Dialog = new TextInputDialog("YouTube média letöltése", "Írd ide a videó címét, amit meg szeretnél nyitni:");
			Dialog.Owner = this;

			bool? Result = Dialog.ShowDialog();
			if (Result.HasValue && Result.Value == true) {
				if (!YoutubeUri.IsValidYoutubeUri(Dialog.UserInput)) {
					PlayerUtils.ErrorMessageBox(App.Name, "Úgy tűnik, hibás linket adtál meg.");
					return;
				}

				await this.OpenFilesAsync(new string[] { Dialog.UserInput });
			}
		}

        private void lLyrics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (this.viewModel.MenuVisible && !this.viewModel.LyricsSettingsVisible)
				this.viewModel.MenuVisible = false;

			this.viewModel.LyricsSettingsVisible = !this.viewModel.LyricsSettingsVisible;
        }
    }
}
