using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using TomiSoft.Music.Lyrics;
using TomiSoft.Music.Lyrics.Xml;
using System.Linq;

namespace TomiSoft.Music.LyricsTests.Xml {
	[TestClass]
	public class XmlLyricsWriterTests {
		private readonly XNamespace ns = XmlLyrics.XmlNamespace;

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void XmlLyricsWriter_BuildAsString_WithoutTranslationAdded() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			Writer.Build();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void XmlLyricsWriter_BuildAsString_WithoutSettingDefaultTranslation() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			Writer.AddTranslation("Hungarian");
			Writer.Build();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void XmlLyricsWriter_BuildAsStream_WithoutTranslationAdded() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			using (Stream s = new MemoryStream())
				Writer.Build(s);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void XmlLyricsWriter_BuildAsStream_WithoutSettingDefaultTranslation() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			Writer.AddTranslation("Hungarian");
			using (Stream s = new MemoryStream())
				Writer.Build(s);
		}

		[TestMethod]
		public void XmlLyricsWriter_SetDefaultTranslationID() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			string TranslationName = "Hungarian";
			string TranslationID = Writer.AddTranslation(TranslationName);
			Writer.DefaultTranslationID = TranslationID;
			string ActualDefaultTranslationID = Writer.DefaultTranslationID;

			Assert.AreEqual(TranslationID, ActualDefaultTranslationID);

			using (Stream Xml = new MemoryStream()) {
				Writer.Build(Xml);
				Assert.IsTrue(this.IsValidXmlBySchema(Xml));

				XDocument doc = XDocument.Load(Xml);

				Assert.AreEqual(
					expected: TranslationID,
					actual: doc.Root.Element(ns + "Translations").Attribute("Default").Value
				);

				Assert.IsTrue(
					doc.Root.Element(ns + "Translations").Elements(ns + "Translation").Where(
						x => x.Attribute("Name").Value == TranslationName &&
							 x.Attribute("ID").Value == TranslationID
					).Any()
				);
			}
		}

		[TestMethod]
		public void XmlLyricsWriter_GetDefaultTranslationID_WhenNotSetPreviously() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			Assert.IsNull(Writer.DefaultTranslationID);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void XmlLyricsWriter_SetDefaultTranslationID_ToANonExistingTranslation() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			Writer.DefaultTranslationID = "A non existing translation ID.";
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_GetTitle_WhenNotSetPreviously() {
			ILyricsMetadata Metadata = new XmlLyricsWriter();
			Assert.IsNull(Metadata.Title);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_GetArtist_WhenNotSetPreviously() {
			ILyricsMetadata Metadata = new XmlLyricsWriter();
			Assert.IsNull(Metadata.Artist);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_GetAlbum_WhenNotSetPreviously() {
			ILyricsMetadata Metadata = new XmlLyricsWriter();
			Assert.IsNull(Metadata.Album);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetAlbum() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			const string ExpectedAlbum = "Sample album";
			wrt.Album = ExpectedAlbum;
			Assert.AreEqual(ExpectedAlbum, wrt.Album);

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				GeneratedXml.Contains($"Album=\"{ExpectedAlbum}\"")
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetTitle_RemovesDeclarationWhenSetToNull() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			wrt.Title = "Sample";
			wrt.Title = null;

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				!GeneratedXml.Contains("Title=\"\"")
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetAlbum_RemovesDeclarationWhenSetToNull() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			wrt.Album = "Sample";
			wrt.Album = null;

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				!GeneratedXml.Contains("Album=\"\"")
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetArtist_RemovesDeclarationWhenSetToNull() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			wrt.Artist = "Sample";
			wrt.Artist = null;

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				!GeneratedXml.Contains("Artist=\"\"")
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetTitle() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			const string ExpectedTitle = "Sample title";
			wrt.Title = ExpectedTitle;
			Assert.AreEqual(ExpectedTitle, wrt.Title);

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				GeneratedXml.Contains($"Title=\"{ExpectedTitle}\"")
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_Metadata_SetArtist() {
			XmlLyricsWriter wrt = new XmlLyricsWriter();
			wrt.DefaultTranslationID = wrt.AddTranslation("Hungarian");

			const string ExpectedArtist = "Sample artist";
			wrt.Artist = ExpectedArtist;
			Assert.AreEqual(ExpectedArtist, wrt.Artist);

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				GeneratedXml.Contains($"Artist=\"{ExpectedArtist}\"")
			);
		}

		public bool IsValidXmlBySchema(Stream Xml) {
			bool ValidationResult = true;

			long PreviousPosition = Xml.Position;

			// Set the validation settings.
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			settings.ValidationEventHandler += (o, e) => {
				if (e.Severity == XmlSeverityType.Error || e.Severity == XmlSeverityType.Warning)
					ValidationResult = false;

				if (e.Exception != null)
					ValidationResult = false;
			};

			// Create the XmlReader object.
			XmlReader reader = XmlReader.Create(Xml, settings);

			// Parse the file. 
			while (reader.Read()) ;

			Xml.Position = PreviousPosition;

			return ValidationResult;
		}
	}
}
