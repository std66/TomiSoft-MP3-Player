using System.Drawing;

namespace TomiSoft.MP3Player.MediaInformation {
	/// <summary>
	/// Provides informations about a song.
	/// </summary>
	public interface ISongInfo {
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

		/// <summary>
		/// Gets the file's path or URI.
		/// </summary>
		string Source { get; }

		/// <summary>
		/// Gets the image of the album.
		/// </summary>
		Image AlbumImage { get; }

		/// <summary>
		/// Gets the length of the song (in bytes or seconds, see IsLengthInSeconds).
		/// </summary>
		double Length { get; }

		/// <summary>
		/// Gets if the Length property is in bytes (false) or seconds (true).
		/// </summary>
		bool IsLengthInSeconds { get; }
	}
}
