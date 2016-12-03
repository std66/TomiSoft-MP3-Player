using System.Drawing;

namespace TomiSoft.MP3Player.Playback {
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
		/// Gets the image of the album.
		/// </summary>
		Image AlbumImage { get; }
	}
}
