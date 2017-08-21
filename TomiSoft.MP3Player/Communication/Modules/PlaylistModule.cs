using System;
using System.Collections.Generic;
using TomiSoft.MP3Player.Common.MediaInformation;

namespace TomiSoft.MP3Player.Communication.Modules {
	public class PlaylistModule : IServerModule {
		private readonly Playlist.Playlist playlist;

		public string ModuleName {
			get {
				return "Playlist";
			}
		}

		public PlaylistModule(Playlist.Playlist Playlist) {
			#region Error checking
			if (Playlist == null)
				throw new ArgumentNullException(nameof(Playlist));
			#endregion

			this.playlist = Playlist;
		}

		[ServerCommand]
		public IEnumerable<ISongMetadata> GetPlaylist() {
			return this.playlist;
		}

		[ServerCommand]
		public string Count() {
			return this.playlist.Count.ToString();
		}

		[ServerCommand]
		public ISongMetadata GetNowPlaying() {
			return this.playlist.CurrentSongInfo;
		}
	}
}
