using System.Drawing;
using TomiSoft.MP3Player.Common.MediaInformation;

namespace TomiSoft.MP3Player.MediaInformation {
	/// <summary>
	/// Provides informations about a song.
	/// </summary>
	public interface ISongInfo : ISongMetadata {
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
