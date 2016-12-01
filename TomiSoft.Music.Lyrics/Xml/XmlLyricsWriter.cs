using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TomiSoft.Music.Lyrics.Xml {
	public class XmlLyricsWriter : ILyricsWriter {
		private XDocument doc;
		private Dictionary<string, string> translations = new Dictionary<string, string>();
		private Dictionary<string, List<ILyricsLine>> lines = new Dictionary<string, List<ILyricsLine>>();

		public string Title {
			get {
				return doc.Root.GetAttributeValue("Title");
			}
			set {
				doc.Root.SetAttributeValue("Title", value);
			}
		}

		public string Artist {
			get {
				return doc.Root.GetAttributeValue("Artist");
			}
			set {
				doc.Root.SetAttributeValue("Artist", value);
			}
		}

		public string Album {
			get {
				return doc.Root.GetAttributeValue("Album");
			}
			set {
				doc.Root.SetAttributeValue("Album", value);
			}
		}

		public string DefaultTranslationID {
			get {
				return doc.Root.Element("Translations").GetAttributeValue("Default");
			}
			set {
				if (!this.translations.ContainsKey(value))
					throw new Exception($"{value} azonosítójú fordítás nem lett még hozzáadva.");

				doc.Root.Element("Translations").SetAttributeValue("Default", value);
			}
		}

		public IReadOnlyDictionary<string, string> Translations {
			get {
				return this.translations;
			}
		}

		public bool SupportsMultipleTranslations {
			get {
				return true;
			}
		}

		public XmlLyricsWriter() {
			this.doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Lyrics"));
			this.doc.Root.Add(new XElement("Translations"));
			this.doc.Root.Add(new XElement("Lines"));
		}

		public string Build() {
			this.BuildLines();

			StringBuilder sb = new StringBuilder();
			using (XmlWriter wrt = XmlWriter.Create(sb, new XmlWriterSettings() { Indent=true })) {
				this.doc.WriteTo(wrt);
			}
			return sb.ToString();
		}

		private void BuildLines() {
			XElement Lines = this.doc.Root.Element("Lines");
			Lines.Elements().Remove();

			foreach (var Translation in this.lines) {
				foreach (var CurrentLine in Translation.Value) {
					string Start = this.SecondsToTimestamp(CurrentLine.StartTime);
					string End = this.SecondsToTimestamp(CurrentLine.EndTime);

					var Line = from e in Lines.Descendants()
							   where e.HasAttributeWithValue("Start", Start) && e.HasAttributeWithValue("End", End)
							   select e;

					XElement TargetLine = null;

					if (Line.Count() == 0) {
						TargetLine = new XElement("Line",
							new XAttribute("Start", Start),
							new XAttribute("End", End)
						);

						Lines.Add(TargetLine);
					}
					else
						TargetLine = Line.First();

					TargetLine.Add(new XElement("Translation",
						new XAttribute("ID", Translation.Key),
						CurrentLine.Text
					));
				}
			}
		}

		public void AddTranslation(string Language) {
			string TranslationID = this.GenerateTranslationID(Language);
			if (!this.translations.ContainsKey(TranslationID)) {
				this.translations.Add(TranslationID, Language);
				this.lines.Add(TranslationID, new List<ILyricsLine>());
				this.UpdateTranslations();
			}
		}

		public void AddLine(string TranslationID, double StartTime, double EndTime, string Text) {
			this.AddLine(
				TranslationID,
				new LyricsLine(StartTime, EndTime, Text)
			);
		}

		public void AddLine(string TranslationID, ILyricsLine Line) {
			this.lines[TranslationID].Add(Line);
		}

		private void UpdateTranslations() {
			this.doc.Root.Element("Translations").Elements().Remove();

			foreach (var item in this.translations) {
				XElement e = new XElement(
					"Translation",
					new XAttribute("ID", item.Key),
					new XAttribute("Name", item.Value)
				);

				this.doc.Root.Element("Translations").Add(e);
			}
		}

		private string GenerateTranslationID(string Language) {
			StringBuilder sb = new StringBuilder();
			foreach (var c in Language) {
				if (Char.IsLetter(c))
					sb.Append(Char.ToLower(c));
				else if (c == ' ')
					sb.Append('_');
			}

			return sb.ToString();
		}

		private string SecondsToTimestamp(double Seconds) {
			if (Double.IsPositiveInfinity(Seconds)) {
				return "999:59.99";
			}
			else {
				TimeSpan Time = TimeSpan.FromSeconds(Seconds);
				return Time.ToString(@"mm\:ss\.ff");

			}
		}

		public static ILyricsWriter CreateFromReader(ILyricsReader Reader) {
			ILyricsWriter wrt = new XmlLyricsWriter() {
				Title = Reader.Title,
				Artist = Reader.Artist,
				Album = Reader.Album
			};

			foreach (var i in Reader.Translations) {
				wrt.AddTranslation(i.Value);

				Reader.TranslationID = i.Key;
				foreach (var line in Reader.GetAllLines()) {
					wrt.AddLine(i.Key, line);
				}
			}

			wrt.DefaultTranslationID = Reader.DefaultTranslationID;

			return wrt;
		}
	}

	static class XElementExtensions {
		public static string GetAttributeValue(this XElement e, XName AttributeName) {
			if (!e.HasAttribute(AttributeName))
				return String.Empty;
			else
				return e.Attribute(AttributeName).Value;
		}

		public static void SetAttributeValue(this XElement e, XName AttributeName, object Value) {
			if (e.HasAttribute(AttributeName))
				e.Add(new XAttribute(AttributeName, Value));
			else
				e.Attribute(AttributeName).SetValue(Value);
		}

		public static bool HasAttribute(this XElement e, XName AttributeName) {
			return e.Attribute(AttributeName) != null;
		}

		public static bool HasAttributeWithValue(this XElement e, XName AttributeName, string Value) {
			return e.HasAttribute(AttributeName) && e.GetAttributeValue(AttributeName) == Value;
		}
	}
}
