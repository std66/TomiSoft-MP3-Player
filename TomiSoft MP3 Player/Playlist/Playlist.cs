using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using TomiSoft.MP3Player.MediaInformation;

namespace TomiSoft.MP3Player.Playlist {
	/// <summary>
	/// Represents a playlist.
	/// </summary>
	public class Playlist : ObservableCollection<ISongInfo> {
		private int currentlyPlaying;
        public event EventHandler SelectedSongChanged;

		/// <summary>
		/// Gets the total length of all songs in the playlist.
		/// </summary>
		public double TotalLength {
			get {
				return this.Sum((x) => x.Length);
			}
		}

		/// <summary>
		/// Gets the index of the currently playing song. Returns 0 also if
        /// the playlist is empty.
		/// </summary>
		public int CurrentlyPlaying { 
			get {
				return this.currentlyPlaying;
			}
			private set {
				if (value < 0 || (value >= this.Count && this.Count > 0)) {
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				this.currentlyPlaying = value;
				this.NotifyPropertyChange("CurrentlyPlaying");
				this.NotifyPropertyChange("CurrentSongInfo");

                this.SelectedSongChanged?.Invoke(this, EventArgs.Empty);
            }
		}

		/// <summary>
		/// Gets the SongInfo instance of the currently playing song.
        /// Returns null if the playlist is empty.
		/// </summary>
		public ISongInfo CurrentSongInfo {
			get {
                if (this.currentlyPlaying < this.Count)
                    return this[currentlyPlaying];

                return null;
			}
		}

		/// <summary>
		/// Creates an empty playlist.
		/// </summary>
		public Playlist() : base() {
			this.CurrentlyPlaying = 0;
			this.CollectionChanged += PlaylistChanged;
		}

        /// <summary>
        /// This method is executed after the collection changed.
        /// </summary>
        /// <param name="sender">The Playlist instance</param>
        /// <param name="e">Event parameters</param>
		private void PlaylistChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (this.Count < this.CurrentlyPlaying) {
                int NewValue = this.Count - 1;

                if (NewValue < 0)
                    NewValue = 0;

                this.CurrentlyPlaying = NewValue;
            }

            this.NotifyPropertyChange(nameof(this.Count));
            this.NotifyPropertyChange(nameof(this.TotalLength));
		}

		/// <summary>
		/// Sets the next song as the current song.
		/// </summary>
		/// <returns>True if the song is set, false if not.</returns>
		public bool MoveToNext() {
			if (this.HasNext()) {
				this.CurrentlyPlaying++;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Sets the previous song as the current song.
		/// </summary>
		/// <returns>True if the song is set, false if not.</returns>
		public bool MoveToPrevious() {
			if (this.HasPrevious()) {
				this.CurrentlyPlaying--;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Sets a specific song by index as the current song.
		/// </summary>
		/// <param name="SongIndex">The index of the song.</param>
		/// <returns>True if the song is set, false if not.</returns>
		public bool MoveTo(int SongIndex) {
            if (SongIndex >= 0 && SongIndex < this.Count) {
                this.CurrentlyPlaying = SongIndex;
                return true;
            }
            else {
                return false;
            }
		}

		/// <summary>
		/// Determines that is there a next song in the list.
		/// </summary>
		/// <returns>True if the playlist has a next song, false if not.</returns>
		public bool HasNext() {
			return CurrentlyPlaying < this.Count - 1;
		}

		/// <summary>
		/// Determines that is there a previous song in the list.
		/// </summary>
		/// <returns>True if the playlist has a previous song, false if not.</returns>
		public bool HasPrevious() {
			return CurrentlyPlaying > 0;
		}

        /// <summary>
        /// Sends notification about a property change.
        /// </summary>
        /// <param name="PropertyName">The name of the property that changed</param>
		private void NotifyPropertyChange(string PropertyName) {
            this.OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));
		}
    }
}
