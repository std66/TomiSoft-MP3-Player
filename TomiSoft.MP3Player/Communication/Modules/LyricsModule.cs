using System;
using System.Text;
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
		public bool HasLoadedLyrics() {
			return this.LyricsReader != null;
		}

		[ServerCommand]
		public string Translations() {
			#region Error checking
			if (this.LyricsReader == null)
				return String.Empty;
			#endregion

			StringBuilder sb = new StringBuilder();

			foreach (var Translation in LyricsReader.Translations) {
				sb.AppendLine($"{Translation.Key};{Translation.Value}");
			}

			return sb.ToString();
		}

		[ServerCommand]
		public void UseTranslation(string TranslationID) {
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
        public string GetCurrentTranslationID() {
            #region Error checking
            if (this.LyricsReader == null)
                return "";
            #endregion

            return this.LyricsReader.TranslationID;
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
