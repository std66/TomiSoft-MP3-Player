using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private IPlaybackManager Player;
		private Playlist Playlist = new Playlist();
		private PlayerServer Server;
		private LrcReader LyricsReader;
		public MainWindowViewModel viewModel = new MainWindowViewModel();

		public MainWindow() {
			//If an instance is already running, send the file list to it.
			//Otherwise, this is the only running instance, so we start a server to
			//listen to the other instances.
			if (User32.FindWindow(null, "TomiSoft MP3 Player") != IntPtr.Zero) {
				this.SendFileListToServer();
			}
			else {
				this.StartServer();
			}

			InitializeComponent();
			this.DataContext = viewModel;
			this.PreparePlaybackController();

			//Attaching a null-playback instance to display default informations.
			this.AttachPlayer(
				PlaybackFactory.NullPlayback(100)
			);

			//Load BASS with its all plugins.
			if (!BassManager.Load()) {
				PlayerUtils.ErrorMessageBox("TomiSoft MP3 Player", "Nem sikerült betölteni a BASS-t.");
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// Sends the file list to the server then closes the application.
		/// Currently only the first file is sent.
		/// </summary>
		private void SendFileListToServer() {
			try {
				using (PlayerClient Client = new PlayerClient()) {
					string[] args = Environment.GetCommandLineArgs();

					if (args.Length > 0)
						Client.Play(args[0]);
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
				Bass.BASS_Free();
			};

			Server.CommandReceived += (Command, Parameter) => {
				Dispatcher.Invoke((Action<string, string>)delegate {
					switch (Command) {
						case "Play":
							this.OpenFile(Parameter);
							this.PlayerOperaion(() => this.Player.Play());
							break;
					}
				}, new object[] { Command, Parameter });
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

				//Load lyrics
				string LyricsFile = Path.ChangeExtension(Filename, "lrc");
				this.OpenLyrics(LyricsFile);

				this.AttachPlayer(
					PlaybackFactory.LoadFile(Filename)
				);
				
				return true;
			}
			catch (Exception e) {
				MessageBox.Show(e.Message);
				return false;
			}
		}

		private void OpenLyrics(string Filename) {
			if (File.Exists(Filename)) {
				this.LyricsReader = new LrcReader(Filename);
			}
			else {
				this.LyricsReader = null;
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
			this.Player.Volume = PreviousVolume;
			PlaybackController.Player = this.Player;

			this.Player.SongEnded += this.PlayNext;

			//Attach lyrics events
			if (this.LyricsReader != null) {
				this.Player.SongEnded += () => this.viewModel.Lyrics = "";
				this.Player.PropertyChanged += (o, ev) => {
					if (ev.PropertyName == "Position") {
						this.viewModel.Lyrics = this.LyricsReader.GetLyricsLine(this.Player.Position);
					}
				};
			}

			//Display album art and send toast notification
			System.Drawing.Image AlbumImage = Properties.Resources.AbstractAlbumArt;
			if (Player.SongInfo != null) {
				AlbumImage = this.Player.SongInfo.AlbumImage;
				if (AlbumImage == null)
					AlbumImage = Properties.Resources.AbstractAlbumArt;

				Toast t = new Toast(System.Reflection.Assembly.GetExecutingAssembly().FullName) {
					Title = this.Player.SongInfo.Title,
					Content = this.Player.SongInfo.Artist,
					Image = AlbumImage
				};
				t.Show();
				viewModel.Title = this.Player.SongInfo.Title;
			}

			viewModel.Lyrics = "Nem találtunk dalszöveget.";
			viewModel.AlbumImage = AlbumImage.ToImageSource();
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
			if (this.OpenFile()) {
				this.PlayerOperaion(() => this.Player.Play());
			}
		}
	}
}
