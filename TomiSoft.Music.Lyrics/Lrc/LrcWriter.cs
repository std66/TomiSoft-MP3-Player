using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Music.Lyrics.Lrc {
	public class LrcWriter : ILyricsWriter, ILyricsMetadata {
		public string Album {
			get;
			set;
		}

		public string Artist {
			get;
			set;
		}

		public string DefaultTranslationID {
			get {
				return "default_translation";
			}
			set {

			}
		}

		public bool SupportsMultipleTranslations {
			get {
				return false;
			}
		}

		public string Title {
			get;
			set;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="LrcWriter"/> class.
		/// Invoking this will cause <see cref="NotImplementedException"/>
		/// </summary>
		public LrcWriter() {
			throw new NotImplementedException();
		}

		public IReadOnlyDictionary<string, string> Translations {
			get {
				return new Dictionary<string, string>() {
					{ "default_translation", "Default translation" }
				};
			}
		}

        public double Length
        {
            get {
                throw new NotImplementedException();
            }
        }

        public void AddLine(string TranslationID, ILyricsLine Line) {
			throw new NotImplementedException();
		}

		public void AddLine(string TranslationID, double StartTime, double EndTime, string Text) {
			throw new NotImplementedException();
		}

		public void AddTranslation(string Language) {
			throw new NotImplementedException();
		}

		public string Build() {
			throw new NotImplementedException();
		}
	}
}
