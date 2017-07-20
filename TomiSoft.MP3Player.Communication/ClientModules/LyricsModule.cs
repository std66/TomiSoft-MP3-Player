using System;
using System.Collections.Generic;
using System.Linq;
using TomiSoft.MP3Player.Common.Lyrics;

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
                return this.Connection.ReadBoolean();
            }
        }

        /// <summary>
        /// Gets whether the currently loaded lyrics supports multiple
        /// translations.
        /// </summary>
        public bool SupportsMultipleTranslations {
            get {
                this.Connection.Send("Lyrics.SupportsMultipleTranslations");
                return this.Connection.ReadBoolean();
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
        public Translation CurrentTranslationID {
            get {
                this.Connection.Send("Lyrics.GetCurrentTranslationID");
                string TranslationID = this.Connection.Read();

                return this.Translations.FirstOrDefault(x => x.TranslationID == TranslationID);
            }
            set {
                if (value != null && this.SupportsMultipleTranslations)
                    this.Connection.Send($"Lyrics.UseTranslation;" + value.TranslationID);
            }
        }
        
        /// <summary>
        /// Gets all the translations. The key is the Translation ID and the value is
        /// its user-friendly display name.
        /// </summary>
        public IEnumerable<Translation> Translations {
            get {
                for (int i = 0; i < NumberOfTranslations; i++) {
                    string[] Parts = this.Connection.Read().Split(';');

                    yield return new Translation(Parts[0], Parts[1]);
                }
            }
        }

        internal LyricsModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
