using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace TomiSoft.Music.Lyrics.Xml {
	/// <summary>
	/// Reads an XML-format lyrics file.
	/// </summary>
	public class XmlLyricsReader : ILyricsReader {
		private XDocument doc;
		private string translationID;

		/// <summary>
		/// Gets the available translations. The key is the ID of the translation and the value
		/// is the user-friendly translation name.
		/// </summary>
		public IReadOnlyDictionary<string, string> Translations {
			get {
				var Result = from Element in doc.Root.Element("Translations").Descendants()
							 select new {
								 TranslationID = Element.Attribute("ID").Value,
								 TranslationName = Element.Attribute("Name").Value
							 };

				return Result.ToDictionary(x => x.TranslationID, x => x.TranslationName);
			}
		}

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title {
			get {
				return doc.Root.Attribute("Title").Value;
			}
		}

		/// <summary>
		/// Gets the default translation's ID.
		/// </summary>
		public string DefaultTranslationID {
			get {
				return doc.Root.Element("Translations").Attribute("Default").Value;
			}
		}

		/// <summary>
		/// Gets or sets the ID of the translation.
		/// </summary>
		public string TranslationID {
			get {
				return this.translationID;
			}

			set {
				if (!this.Translations.ContainsKey(value))
					throw new Exception("A megadott fordítás nem létezik.");

				this.translationID = value;
			}
		}

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		public string Artist {
			get {
				return doc.Root.Attribute("Artist").Value;
			}
		}

		/// <summary>
		/// Gets the album's name that contains the song.
		/// </summary>
		public string Album {
			get {
				return doc.Root.Attribute("Album").Value;
			}
		}

		/// <summary>
		/// Gets the length of the song in seconds.
		/// </summary>
		public double Length {
			get {
				return this.TimestampToSeconds(doc.Root.Attribute("Length").Value);
			}
		}

		/// <summary>
		/// Returns true representing this format supports multiple translations.
		/// </summary>
		public bool SupportsMultipleTranslations {
			get {
				return true;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlLyricsReader"/> class. Loads the given lyrics file.
		/// </summary>
		/// <param name="Filename">The file's URI to load.</param>
		public XmlLyricsReader(string Filename) {
			this.doc = XDocument.Load(Filename);
			this.TranslationID = this.DefaultTranslationID;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlLyricsReader"/> class. Loads the given lyrics file.
		/// </summary>
		/// <param name="Stream">A <see cref="Stream"/> that represents the file.</param>
		public XmlLyricsReader(System.IO.Stream Stream) {
			this.doc = XDocument.Load(Stream);
			this.TranslationID = this.DefaultTranslationID;
		}

		/// <summary>
		/// Gets the lyrics line(s) at the given playback position. Multiple lines may be returned eg. when
		/// there are more than one singers. The translation defined in the <see cref="TranslationID"/> property will be used.
		/// </summary>
		/// <param name="Seconds">The playback position in seconds.</param>
		/// <returns>The lyrics line(s)</returns>
		public IEnumerable<string> GetLyricsLine(double Seconds) {
			return from LineNode in this.doc.Descendants("Line")
				   let Text = LineNode.Elements().Where(x => x.Attribute("ID").Value == this.translationID).First().Value
				   let Start = this.TimestampToSeconds(LineNode.Attribute("Start").Value)
				   let End = this.TimestampToSeconds(LineNode.Attribute("End").Value)
				   where Start <= Seconds && Seconds <= End
				   select Text;
		}

		/// <summary>
		/// Converts a timestamp to seconds.
		/// </summary>
		/// <param name="Time">The timestamp to parse</param>
		/// <returns>The timestamp in seconds</returns>
		private double TimestampToSeconds(string Time) {
			string Pattern = @"(?<mins>\d{2}):(?<secs>\d{2}).(?<msecs>\d{2})";
			var Matches = Regex.Matches(Time, Pattern);

			int Mins = Convert.ToInt32(Matches[0].Groups["mins"].Value);
			int Secs = Convert.ToInt32(Matches[0].Groups["secs"].Value);
			int MSecs = Convert.ToInt32(Matches[0].Groups["msecs"].Value);

			return Mins * 60 + Secs + (MSecs / 100.0);
		}

		/// <summary>
		/// Gets all the lines of the given translation (see <see cref="TranslationID"/>).
		/// </summary>
		/// <returns>A sequence of the lyrics' lines.</returns>
		public IEnumerable<ILyricsLine> GetAllLines() {
			return from LineNode in this.doc.Descendants("Line")
				   let Text = LineNode.Elements().Where(x => x.Attribute("ID").Value == this.translationID).First().Value
				   let Start = this.TimestampToSeconds(LineNode.Attribute("Start").Value)
				   let End = this.TimestampToSeconds(LineNode.Attribute("End").Value)
				   select new LyricsLine(Start, End, Text);
		}
	}
}
