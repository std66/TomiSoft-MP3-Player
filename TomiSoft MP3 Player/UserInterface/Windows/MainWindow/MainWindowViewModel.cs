using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using TomiSoft.Music.Lyrics;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Utils.Extensions;
using System.Windows;

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

				string t = this.playbackManager.SongInfo.Title;
				return String.IsNullOrWhiteSpace(t) ? "Nincs cím" : t;
			}
		}

		/// <summary>
		/// Gets or sets the current line of lyrics.
		/// </summary>
		public string Lyrics {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the IPlaybackManager instance where the song's informations are
		/// taken from.
		/// </summary>
		public IPlaybackManager PlaybackManager {
			get {
				return this.playbackManager;
			}
			set {
				if (this.playbackManager != null)
					this.playbackManager.PropertyChanged -= this.PlaybackStateChanged;

				if (value == null)
					throw new ArgumentNullException("PlaybackManager");

				this.playbackManager = value;
				this.playbackManager.PropertyChanged += this.PlaybackStateChanged;
				this.NotifyAll();
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

		public MainWindowViewModel() {
			this.NotifyAll();
		}

		/// <summary>
		/// Event handler for IPlaybackManager.PropertyChanged.
		/// </summary>
		/// <param name="PlaybackPropertyName">The property's name that has changed.</param>
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
						this.Lyrics = this.lyricsReader.GetLyricsLine(Playback.Position).First();
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
