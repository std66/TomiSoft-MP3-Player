using System.Collections.Generic;

namespace TomiSoft.MP3Player.Playlist {
	/// <summary>
	/// Represents a playlist provider.
	/// </summary>
	interface IPlaylistProvider {
		/// <summary>
		/// Gets the path of the media files.
		/// </summary>
		IEnumerable<string> Files {
			get;
		}
	}
}
