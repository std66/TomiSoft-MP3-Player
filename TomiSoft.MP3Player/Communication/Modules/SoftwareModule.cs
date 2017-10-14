using System;
using System.Linq;
using TomiSoft.MP3Player.Playback;
using TomiSoft.MP3Player.Playback.BASS;
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
		public Version Version() {
			return App.Version;
		}

		[ServerCommand]
		public string Author() {
			return App.Author;
		}

		[ServerCommand]
		public Uri Website() {
			return App.Website;
		}

		[ServerCommand]
		public string Path() {
			return App.Path;
		}

		[ServerCommand]
		public bool IsRunning() {
			return true;
		}

		[ServerCommand]
		public Version ApiVersion() {
			return new Version(2, 0, 0, 0);
		}

		[ServerCommand]
		public string TraceLog() {
			return App.TraceLog;
		}

		[ServerCommand]
		public string[] GetSupportedFileExtensions() {
			return BassManager.GetSupportedExtensions().ToArray();
		}

		[ServerCommand]
		public bool IsSupportedMedia(string Source) {
			return PlaybackFactory.IsSupportedMedia(Source);
		}
	}
}
