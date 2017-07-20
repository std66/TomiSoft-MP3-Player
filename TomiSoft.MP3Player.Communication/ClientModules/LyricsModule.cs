using System;
using System.Collections.Generic;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the lyrics.
    /// </summary>
    public class LyricsModule {
        private readonly ServerConnection Connection;

        /// <summary>
        /// Gets whether there is a lyrics loaded.
        /// </summary>
        public bool HasLyricsLoaded {
            get {
                this.Connection.Send("Lyrics.HasLyricsLoaded");
                return Convert.ToBoolean(this.Connection.Read());
            }
        }

        /// <summary>
        /// Gets the number of translations supported by the currently
        /// loaded lyrics.
        /// </summary>
        public int NumberOfTranslations {
            get {
                this.Connection.Send("Lyrics.GetNumberOfTranslations");
                return Convert.ToInt32(this.Connection.Read());
            }
        }

        /// <summary>
        /// Gets or sets the currently displayed translation.
        /// </summary>
        public string CurrentTranslationID {
            get {
                this.Connection.Send("Lyrics.GetCurrentTranslationID");
                return this.Connection.Read();
            }
            set {
                this.Connection.Send($"Lyrics.UseTranslation;" + value);
            }
        }
        
        /// <summary>
        /// Gets all the translations. The key is the Translation ID and the value is
        /// its user-friendly display name.
        /// </summary>
        public IDictionary<string, string> Translations {
            get {
                Dictionary<string, string> Result = new Dictionary<string, string>();

                for (int i = 0; i < NumberOfTranslations; i++) {
                    string[] Parts = this.Connection.Read().Split(';');

                    Result.Add(Parts[0], Parts[1]);
                }

                return Result;
            }
        }

        internal LyricsModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
