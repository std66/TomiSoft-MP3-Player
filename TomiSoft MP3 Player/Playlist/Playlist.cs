using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TomiSoft.MP3Player.Utils;

namespace TomiSoft.MP3Player.Playlist {
	/// <summary>
	/// Represents a playlist.
	/// </summary>
	public class Playlist : ObservableCollection<SongInfo> {
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
		public SongInfo CurrentSongInfo {
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
			bool Result = false;
			try {
				this.CurrentlyPlaying = SongIndex;
				Result = true;
			}
			catch (ArgumentOutOfRangeException) { }

			return Result;
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

		private void NotifyChanges() {
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void NotifyPropertyChange(string PropertyName) {
			this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
		}
	}
}
