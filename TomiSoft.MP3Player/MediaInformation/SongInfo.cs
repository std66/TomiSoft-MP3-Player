using System;
using System.Drawing;

namespace TomiSoft.MP3Player.MediaInformation {
	internal class SongInfo : ISongInfo {
		public string Album {
			get;
			set;
		}

		public Image AlbumImage {
			get;
			set;
		}

		public string Artist {
			get;
			set;
		}

		public bool IsLengthInSeconds {
			get;
			private set;
		}

		public double Length {
			get;
			private set;
		}

		public string Source {
			get;
			set;
		}

		public string Title {
			get;
			set;
		}

		public SongInfo(string Source, double Length, bool IsLengthInSeconds) {
			#region Error checking
			if (String.IsNullOrWhiteSpace(Source))
				throw new ArgumentException($"{nameof(Source)} is null, empty {nameof(String)} or a {nameof(String)} that contains only white-spaces.");
			#endregion

			this.Source = Source;
			this.Length = Length;
			this.IsLengthInSeconds = IsLengthInSeconds;
		}

		public SongInfo(ISongInfo SongInfo) {
			#region Error checking
			if (SongInfo == null)
				throw new ArgumentNullException(nameof(SongInfo));
			#endregion

			this.Album = SongInfo.Album;
			this.AlbumImage = SongInfo.AlbumImage;
			this.Artist = SongInfo.Artist;
			this.IsLengthInSeconds = SongInfo.IsLengthInSeconds;
			this.Length = SongInfo.Length;
			this.Source = SongInfo.Source;
			this.Title = SongInfo.Title;
		}
	}
}
