using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Represents a playlist.
	/// </summary>
	class Playlist : ObservableCollection<SongInfo> {
		private int currentlyPlaying;

		/// <summary>
		/// Gets the total length of all songs in the playlist.
		/// </summary>
		public double TotalLength {
			get {
				return this.Sum((x) => x.Length);
			}
		}

		/// <summary>
		/// Gets the index of the currently playing song.
		/// </summary>
		public int CurrentlyPlaying { 
			get {
				return this.currentlyPlaying;
			}
			private set {
				if (value < 0 || (value >= this.Count && this.Count > 0)) {
					throw new ArgumentOutOfRangeException("value");
				}

				this.currentlyPlaying = value;
				this.NotifyPropertyChange("CurrentlyPlaying");
				this.NotifyPropertyChange("CurrentSongInfo");
			}
		}

		/// <summary>
		/// Gets the SongInfo instance of the currently playing song.
		/// </summary>
		public SongInfo CurrentSongInfo {
			get {
				return this[currentlyPlaying];
			}
		}

		/// <summary>
		/// Creates an empty playlist.
		/// </summary>
		public Playlist() : base() {
			this.CurrentlyPlaying = 0;
			this.CollectionChanged += PlaylistChanged;
		}

		private void PlaylistChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (this.Count < this.CurrentlyPlaying) {
				this.CurrentlyPlaying = this.Count - 1;
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
			catch (ArgumentOutOfRangeException e) { }

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
