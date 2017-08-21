using System.Text;

namespace TomiSoft.MP3Player.Common.MediaInformation {
    /// <summary>
    /// Represents the metadata of a media.
    /// </summary>
    public class SongMetadata : ISongMetadata {
        /// <summary>
        /// Gets the album name of the song.
        /// </summary>
        public string Album {
            get;
            private set;
        }

        /// <summary>
        /// Gets the artist of the song.
        /// </summary>
        public string Artist {
            get;
            private set;
        }

        /// <summary>
		/// Gets the title of the song.
		/// </summary>
        public string Title {
            get;
            private set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SongMetadata"/> class.
        /// </summary>
        /// <param name="Title">The title of the song</param>
        /// <param name="Artist">The artist of the song</param>
        /// <param name="Album">The album name of the song</param>
        public SongMetadata(string Title, string Artist, string Album) {
            this.Title = Title;
            this.Artist = Artist;
            this.Album = Album;
        }

		public override string ToString() {
			StringBuilder sb = new StringBuilder();

			if (this.Artist != null)
				sb.Append($"{this.Artist} - ");

			sb.Append(this.Title);

			return sb.ToString();
		}
	}
}
