namespace TomiSoft.MP3Player.Common.MediaInformation {
    /// <summary>
    /// Represents the metadata of a media.
    /// </summary>
    public interface ISongMetadata {
        /// <summary>
		/// Gets the title of the song.
		/// </summary>
		string Title { get; }

        /// <summary>
        /// Gets the artist of the song.
        /// </summary>
        string Artist { get; }

        /// <summary>
        /// Gets the album name of the song.
        /// </summary>
        string Album { get; }
    }
}
