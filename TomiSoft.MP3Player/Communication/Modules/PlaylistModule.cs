using System;
using System.Text;
using TomiSoft.MP3Player.MediaInformation;

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
		public string List() {
			StringBuilder sb = new StringBuilder();

			int Index = 0;
			
			foreach (ISongInfo Song in this.playlist) {
				sb.AppendLine($"<index>{Index}</index><artist>{Song.Artist}</artist><title>{Song.Title}</title><source>{Song.Source}</source>");
				Index++;
			}

			return sb.ToString();
		}

		[ServerCommand]
		public string Count() {
			return this.playlist.Count.ToString();
		}
	}
}
