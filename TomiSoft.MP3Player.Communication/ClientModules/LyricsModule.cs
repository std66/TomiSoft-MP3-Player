using System.Collections.Generic;
using TomiSoft.MP3Player.Common.Lyrics;

namespace TomiSoft.MP3Player.Communication.ClientModules {
	/// <summary>
	/// Provides commands for controlling the lyrics.
	/// </summary>
	public class LyricsModule {
		private readonly string ServerModule = "Lyrics";
		private readonly ServerConnection Connection;

        /// <summary>
        /// Gets whether there is a lyrics loaded.
        /// </summary>
        public bool HasLyricsLoaded {
            get {
				var Response = this.Connection.Send<bool>(
					new ServerRequest(ServerModule, "HasLyricsLoaded")
				);

				Response.Check();

				return Response.Result;
			}
        }

        /// <summary>
        /// Gets whether the currently loaded lyrics supports multiple
        /// translations.
        /// </summary>
        public bool SupportsMultipleTranslations {
            get {
				var Response = this.Connection.Send<bool>(
					new ServerRequest(ServerModule, "SupportsMultipleTranslations")
				);

				Response.Check();

				return Response.Result;
			}
        }

        /// <summary>
        /// Gets the number of translations supported by the currently
        /// loaded lyrics.
        /// </summary>
        public int NumberOfTranslations {
            get {
				var Response = this.Connection.Send<int>(
					new ServerRequest(ServerModule, "GetNumberOfTranslations")
				);

				Response.Check();

				return Response.Result;
			}
        }

        /// <summary>
        /// Gets or sets the currently displayed translation.
        /// </summary>
        public Translation CurrentTranslation {
            get {
				var Response = this.Connection.Send<Translation>(
					new ServerRequest(ServerModule, "GetCurrentTranslation")
				);

				Response.Check();

				return Response.Result;
			}

            set {
				if (value != null && this.SupportsMultipleTranslations) {
					var Response = this.Connection.Send(
						new ServerRequest(ServerModule, "SetCurrentTranslation", value.TranslationID)
					);

					Response.Check();
				}
            }
        }
        
        /// <summary>
        /// Gets all the translations. The key is the Translation ID and the value is
        /// its user-friendly display name.
        /// </summary>
        public IEnumerable<Translation> Translations {
            get {
				var Response = this.Connection.Send<IEnumerable<Translation>>(
					new ServerRequest(ServerModule, "GetTranslations")
				);

				if (!Response.RequestSucceeded)
					return null;

				return Response.Result;
            }
        }

        internal LyricsModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
