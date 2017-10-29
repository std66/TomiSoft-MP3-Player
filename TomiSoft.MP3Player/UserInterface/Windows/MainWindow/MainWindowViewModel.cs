using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using TomiSoft.Music.Lyrics;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Utils.Extensions;
using System.Windows;
using TomiSoft.MP3Player.Playlist;
using System.Collections.Specialized;
using TomiSoft.MP3Player.UserInterface.Windows.MainWindow;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// View model for MainWindow.
	/// </summary>
	public class MainWindowViewModel : INotifyPropertyChanged {
		/// <summary>
		/// Fires when a property is changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private IPlaybackManager playbackManager;
		private ILyricsReader lyricsReader;
        private Playlist playlist;
        private string lyrics;
		private bool menuVisible = false;
		private readonly ResourceDictionary Resources;

		private readonly SaveMediaCommand saveMediaCommand;
		private readonly ConnectWithPhoneCommand connectWithPhoneCommand;
		private readonly ShowAboutWindowCommand showAboutWindowCommand;

        private bool lyricsSettingsIsVisible = false;

		public bool MenuVisible {
			get {
				return this.menuVisible;
			}
			set {
				this.menuVisible = value;

				if (value) {
					this.LyricsSettingsVisible = false;
					(this.Resources["FadeInAnimation"] as Storyboard)?.Begin();
				}
				else {
					(this.Resources["FadeOutAnimation"] as Storyboard)?.Begin();
				}
			}
		}

        /// <summary>
        /// Gets or sets whether the lyrics settings panel is visible.
        /// </summary>
        public bool LyricsSettingsVisible {
            get {
                return this.lyricsSettingsIsVisible;
            }
            set {
				if (this.MenuVisible && value)
					this.MenuVisible = false;

                this.lyricsSettingsIsVisible = value;
                this.NotifyPropertyChanged(nameof(LyricsSettingsVisible));
            }
        }

		/// <summary>
		/// Gets the <see cref="ICommand"/> for the "Save media" button.
		/// </summary>
		public ICommand SaveMediaCommand {
			get {
				return this.saveMediaCommand;
			}
		}

		/// <summary>
		/// Gets the <see cref="ICommand"/> for the "Connect with phone" button.
		/// </summary>
		public ICommand ConnectWithPhoneCommand {
			get {
				return this.connectWithPhoneCommand;
			}
		}

		/// <summary>
		/// Gets the <see cref="ICommand"/> for the "Show About window" button.
		/// </summary>
		public ICommand ShowAboutWindowCommand {
			get {
				return this.showAboutWindowCommand;
			}
		}

		/// <summary>
		/// Gets the name of the application
		/// </summary>
		public string ApplicationName {
            get {
                return App.Name;
            }
        }

		/// <summary>
		/// Gets the album image.
		/// </summary>
		public ImageSource AlbumImage {
			get {
				ImageSource DefaultImage = TomiSoft.MP3Player.Properties.Resources.AbstractAlbumArt.ToImageSource();

				if (this.playbackManager != null && !(this.playbackManager is NullPlayback)) {
					if (this.playbackManager.SongInfo.AlbumImage != null)
						return this.playbackManager.SongInfo.AlbumImage.ToImageSource();
				}

				return DefaultImage;
			}
		}

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title {
			get {
				if (this.playbackManager == null || this.playbackManager is NullPlayback)
					return "Nincs zene betöltve";
				
				return this.playbackManager.SongInfo.Title ?? "Ismeretlen szám";
			}
		}

		/// <summary>
		/// Gets or sets the current line of lyrics.
		/// </summary>
		public string Lyrics {
			get {
                return this.lyrics;
            }
			set {
                this.lyrics = value;
                this.NotifyPropertyChanged(nameof(Lyrics));
            }
		}

		/// <summary>
		/// Gets or sets the IPlaybackManager instance where the song's informations are
		/// taken from.
		/// </summary>
        /// <exception cref="ArgumentNullException">when the assigned value is null</exception>
		public IPlaybackManager PlaybackManager {
			get {
				return this.playbackManager;
			}
			set {
                #region Error checking
                if (value == null)
                    throw new ArgumentNullException("PlaybackManager");
                #endregion

                if (this.playbackManager != null)
					this.playbackManager.PropertyChanged -= this.PlaybackStateChanged;
                
				this.playbackManager = value;
				this.saveMediaCommand.PlaybackManager = value;
				this.playbackManager.PropertyChanged += this.PlaybackStateChanged;
				this.NotifyAll();
			}
		}

        /// <summary>
        /// Gets or sets the playlist instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">when the assigned value is null</exception>
        public Playlist Playlist {
            get {
                return this.playlist;
            }
            set {
                #region Error checking
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                #endregion

                if (this.playlist != null)
                    this.playlist.CollectionChanged -= this.PlaylistChanged;

                this.playlist = value;
                this.playlist.CollectionChanged += this.PlaylistChanged;
                this.NotifyPropertyChanged(nameof(Playlist));
            }
        }

		/// <summary>
		/// Gets or sets the ILyricsReader instance which is used for displaying lyrics.
		/// </summary>
		public ILyricsReader LyricsReader {
			get {
				return this.lyricsReader;
			}
			set {
				if (value == null) {
					this.Lyrics = "Nem találtunk dalszöveget.";
					this.lyricsReader = null;

					this.NotifyPropertyChanged("Lyrics");
					this.NotifyPropertyChanged("LyricsReader");
					return;
				}

				this.lyricsReader = value;
				this.Lyrics = String.Empty;
				this.NotifyAll();
			}
		}
		
		public MainWindowViewModel() : this(null) {}

		public MainWindowViewModel(Window Window) {
			if (Window != null) {
				this.Resources = Window.Resources;
				this.saveMediaCommand = new SaveMediaCommand(this);
				this.connectWithPhoneCommand = new ConnectWithPhoneCommand(this);
				this.showAboutWindowCommand = new ShowAboutWindowCommand(this, Window);
			}

			this.NotifyAll();
		}

        /// <summary>
        /// Event handler for Playlist.CollectionChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.NotifyPropertyChanged(nameof(this.Playlist));
        }

		/// <summary>
		/// Event handler for IPlaybackManager.PropertyChanged.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
        /// <param name="e">Event parameters</param>
		private void PlaybackStateChanged(object sender, PropertyChangedEventArgs e) {
			IPlaybackManager Playback = (IPlaybackManager)sender;

			switch (e.PropertyName) {
				case "IsPlaying":
					if (!Playback.IsPlaying && this.lyricsReader != null) {
						this.Lyrics = "";
						this.NotifyPropertyChanged("Lyrics");
					}
					break;

				case "Position":
					if (this.lyricsReader != null) {
						this.Lyrics = this.lyricsReader.GetLyricsLine(Playback.Position).FirstOrDefault();
						this.NotifyPropertyChanged("Lyrics");
					}
					break;
			}
		}

		/// <summary>
		/// Fires the PropertyChanged event for the given property.
		/// </summary>
		/// <param name="PropertyName">The property's name that value has changed.</param>
		private void NotifyPropertyChanged(string PropertyName) {
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
		}

		/// <summary>
		/// Fires the PropertyChanged event for all properties.
		/// </summary>
		private void NotifyAll() {
			if (this.PropertyChanged == null)
				return;

			foreach (var Property in this.GetType().GetProperties()) {
				PropertyChanged(this, new PropertyChangedEventArgs(Property.Name));
			}
		}
	}
}
