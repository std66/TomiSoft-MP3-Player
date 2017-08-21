using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomiSoft.MP3Player.Common.Lyrics;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Communication.Modules {
	public class LyricsModule : IServerModule {
		public ILyricsReader LyricsReader {
			get;
			set;
		}
		
		public string ModuleName {
			get {
				return "Lyrics";
			}
		}

		[ServerCommand]
		public bool HasLyricsLoaded() {
			return this.LyricsReader != null;
		}

		[ServerCommand]
		public IEnumerable<Translation> GetTranslations() {
			#region Error checking
			if (this.LyricsReader == null)
				return null;
			#endregion

			return this.LyricsReader.Translations.Select(
				x => new Translation(x.Key, x.Value)
			);
		}

		[ServerCommand]
		public void SetCurrentTranslation(string TranslationID) {
			#region Error checking
			if (this.LyricsReader == null)
				return;
			#endregion

			if (LyricsReader.Translations.ContainsKey(TranslationID)) {
				LyricsReader.TranslationID = TranslationID;
			}
		}

        [ServerCommand]
        public int GetNumberOfTranslations() {
            #region Error checking
            if (this.LyricsReader == null)
                return 0;
            #endregion

            return this.LyricsReader.Translations.Count;
        }

        [ServerCommand]
        public Translation GetCurrentTranslation() {
			#region Error checking
			if (this.LyricsReader == null)
				return null;
			#endregion

			var DisplayName = this.LyricsReader.Translations[this.LyricsReader.TranslationID];
			return new Translation(this.LyricsReader.TranslationID, DisplayName);

		}

        [ServerCommand]
        public bool SupportsMultipleTranslations() {
            #region Error checking
            if (this.LyricsReader == null)
                return false;
            #endregion

            return this.LyricsReader.SupportsMultipleTranslations;
        }
    }
}
