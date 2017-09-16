using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TomiSoft.Music.Lyrics.Xml {
    /// <summary>
    /// Represents a lyrics writer for XML format.
    /// </summary>
	public class XmlLyricsWriter : ILyricsWriter, ILyricsMetadata {
        /// <summary>
        /// Stores the structure of the XML file.
        /// </summary>
		private XDocument doc;

		/// <summary>
		/// Stores the default XML namespace.
		/// </summary>
		private readonly XNamespace Namespace;

        /// <summary>
        /// Stores the translations. The key is the TranslationID and the value is a user-friendly name.
        /// </summary>
		private Dictionary<string, string> translations = new Dictionary<string, string>();
        
		private Dictionary<string, List<ILyricsLine>> lines = new Dictionary<string, List<ILyricsLine>>();

        /// <summary>
        /// Gets or sets the title of the song.
        /// </summary>
		public string Title {
			get {
				if (!doc.Root.HasAttribute("Title"))
					return null;

				return doc.Root.GetAttributeValue("Title");
			}
			set {
				doc.Root.SetAttributeValue("Title", value);
			}
		}

        /// <summary>
        /// Gets or sets the artist of the song.
        /// </summary>
		public string Artist {
			get {
				if (!doc.Root.HasAttribute("Artist"))
					return null;
				
				return doc.Root.GetAttributeValue("Artist");
			}
			set {
				doc.Root.SetAttributeValue("Artist", value);
			}
		}

        /// <summary>
        /// Gets or sets the album of the song.
        /// </summary>
		public string Album {
			get {
				if (!doc.Root.HasAttribute("Album"))
					return null;

				return doc.Root.GetAttributeValue("Album");
			}
			set {
				doc.Root.SetAttributeValue("Album", value);
			}
		}
        
        /// <summary>
        /// Gets or sets the title of the song.
        /// </summary>
        /// <exception cref="ArgumentException">when a translation with ID <paramref name="value"/> has not added yet</exception>
		public string DefaultTranslationID {
			get {
				if (!doc.Root.Element(this.Namespace + "Translations").HasAttribute("Default"))
					return null;
				
				var Value = doc.Root.Element(this.Namespace + "Translations").GetAttributeValue("Default");

				if (String.IsNullOrEmpty(Value))
					return null;

				return Value;
			}
			set {
				if (!this.translations.ContainsKey(value))
					throw new ArgumentException($"A translation with ID '{value}' has not added yet.");

				doc.Root.Element(this.Namespace + "Translations").SetAttributeValue("Default", value);
			}
		}

        /// <summary>
        /// Gets all the translations. The key is the Translation ID and the value is a
        /// user-friendly display name.
        /// </summary>
		public IReadOnlyDictionary<string, string> Translations {
			get {
				return this.translations;
			}
		}

        /// <summary>
        /// Gets if this format supports multiple translations. Always returns true.
        /// </summary>
		public bool SupportsMultipleTranslations {
			get {
				return true;
			}
		}
        
        public double Length
        {
            get
            {
                return 0;
            }
        }

        public XmlLyricsWriter() {
			this.Namespace = XmlLyrics.XmlNamespace;
			XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

			XElement Root = new XElement(
				Namespace + "Lyrics",

				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XAttribute(xsi + "schemaLocation", $"{XmlLyrics.XmlNamespace} {XmlLyrics.XmlSchemaLocation}"),

				new XElement(this.Namespace + "Translations",
					new XAttribute("Default", "")
				),
				new XElement(this.Namespace + "Lines")
			);

			this.doc = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"), 
				Root
			);
		}

		public string Build() {
			#region Error checking
			this.ChecksBeforeBuild();
			#endregion

			this.BuildLines();

			StringBuilder sb = new StringBuilder();
			using (XmlWriter wrt = XmlWriter.Create(sb, new XmlWriterSettings() { Indent=false, Encoding=new UTF8Encoding(true) })) {
				this.doc.WriteTo(wrt);
			}

			return sb.ToString();
		}

		public void Build(Stream TargetStream) {
			#region Error checking
			if (TargetStream == null)
				throw new ArgumentNullException(nameof(TargetStream));

			if (!TargetStream.CanWrite)
				throw new ArgumentException($"{nameof(TargetStream)} is not a writeable stream.");

			this.ChecksBeforeBuild();
			#endregion

			long PreviousPosition = TargetStream.Position;
			XmlWriter wrt = XmlWriter.Create(TargetStream, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8 });
			this.doc.WriteTo(wrt);
			wrt.Flush();
			TargetStream.Position = PreviousPosition;
		}

		private void BuildLines() {
			XElement Lines = this.doc.Root.Element(this.Namespace + "Lines");
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
						TargetLine = new XElement(this.Namespace + "Line",
							new XAttribute("Start", Start),
							new XAttribute("End", End)
						);

						Lines.Add(TargetLine);
					}
					else
						TargetLine = Line.First();

					TargetLine.Add(new XElement(this.Namespace + "Translation",
						new XAttribute("ID", Translation.Key),
						CurrentLine.Text
					));
				}
			}
		}

		public string AddTranslation(string Language) {
			string TranslationID = this.GenerateTranslationID(Language);

			if (!this.translations.ContainsKey(TranslationID)) {
				this.translations.Add(TranslationID, Language);
				this.lines.Add(TranslationID, new List<ILyricsLine>());
				this.UpdateTranslations();
			}

			return TranslationID;
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
			this.doc.Root.Element(this.Namespace + "Translations").Elements().Remove();

			foreach (var item in this.translations) {
				XElement e = new XElement(
					this.Namespace + "Translation",
					new XAttribute("ID", item.Key),
					new XAttribute("Name", item.Value)
				);

				this.doc.Root.Element(this.Namespace + "Translations").Add(e);
			}
		}

		private string GenerateTranslationID(string Language) {
			var b = Guid.NewGuid().ToByteArray();
			b[3] |= 0xF0;
			return new Guid(b).ToString();
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
            XmlLyricsWriter wrt = new XmlLyricsWriter();

            ILyricsMetadata Metadata = Reader as ILyricsMetadata;
            if (Metadata != null)
            {
                wrt.Title  = Metadata.Title;
                wrt.Artist = Metadata.Artist;
                wrt.Album  = Metadata.Album;
            }

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

		private void ChecksBeforeBuild() {
			if (this.translations.Count == 0)
				throw new InvalidOperationException("You must add at least one translation to build a valid file.");

			if (this.DefaultTranslationID == null)
				throw new InvalidOperationException($"You must set {nameof(DefaultTranslationID)} to build a valid file.");
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
