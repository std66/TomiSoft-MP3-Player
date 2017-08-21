using System.Collections.Generic;
using TomiSoft.MP3Player.Common.MediaInformation;

namespace TomiSoft.MP3Player.Communication.ClientModules {
	/// <summary>
	/// Provides commands for controlling the playlist.
	/// </summary>
	public class PlaylistModule {
        private readonly ServerConnection Connection;
		
		/// <summary>
		/// Gets informations about all the songs that are currently on the
		/// playlist.
		/// </summary>
		public IEnumerable<ISongMetadata> Playlist {
			get {
				var Response = this.Connection.Send<IEnumerable<SongMetadata>>(
					new ServerRequest("Playlist", "GetPlaylist")
				);

				if (!Response.RequestSucceeded)
					return null;

				return Response.Result;
			}
		}

		/// <summary>
		/// Gets informations about the currently playing song.
		/// </summary>
		public ISongMetadata CurrentlyPlaying {
			get {
				var Response = this.Connection.Send<SongMetadata>(
					new ServerRequest("Playlist", "GetNowPlaying")
				);

				if (!Response.RequestSucceeded)
					return null;

				return Response.Result;
			}
		}

        internal PlaylistModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
