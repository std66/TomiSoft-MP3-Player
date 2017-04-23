using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Playback.YouTube;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Communication.Modules {
	public class SoftwareModule : IServerModule {
		public string ModuleName {
			get {
				return "Software";
			}
		}

		[ServerCommand]
		public string Name() {
			return App.Name;
		}

		[ServerCommand]
		public string Version() {
			return App.Version.ToString();
		}

		[ServerCommand]
		public string Author() {
			return App.Author;
		}

		[ServerCommand]
		public string Website() {
			return App.Website.ToString();
		}

		[ServerCommand]
		public string Path() {
			return App.Path;
		}

		[ServerCommand]
		public string IsRunning() {
			return "true";
		}
	}
}
