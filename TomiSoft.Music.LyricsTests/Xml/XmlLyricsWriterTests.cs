using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TomiSoft.Music.Lyrics;
using TomiSoft.Music.Lyrics.Xml;

namespace TomiSoft.Music.LyricsTests.Xml {
	[TestClass]
	public class XmlLyricsWriterTests {
		[TestMethod]
		public void XmlLyricsWriter_CreateEmptyFile() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			string Actual = Writer.Build();

			Assert.AreEqual(
				"<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><Lyrics><Translations /><Lines /></Lyrics>",
				Actual
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_AddTranslation() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			string TranslationName = "Hungarian";
			string TranslationID = Writer.AddTranslation(TranslationName);
			string Actual = Writer.Build();

			Assert.AreEqual(
				$"<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><Lyrics><Translations><Translation ID=\"{TranslationID}\" Name=\"{TranslationName}\" /></Translations><Lines /></Lyrics>",
				Actual
			);
		}

		[TestMethod]
		public void XmlLyricsWriter_SetDefaultTranslationID() {
			ILyricsWriter Writer = new XmlLyricsWriter();
			string TranslationName = "Hungarian";
			string TranslationID = Writer.AddTranslation(TranslationName);
			Writer.DefaultTranslationID = TranslationID;
			string ActualDefaultTranslationID = Writer.DefaultTranslationID;
			string Actual = Writer.Build();

			Assert.AreEqual(
				$"<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?><Lyrics><Translations Default=\"{TranslationID}\"><Translation ID=\"{TranslationID}\" Name=\"{TranslationName}\" /></Translations><Lines /></Lyrics>",
				Actual
			);

			Assert.AreEqual(TranslationID, ActualDefaultTranslationID);
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

			const string ExpectedArtist = "Sample artist";
			wrt.Artist = ExpectedArtist;
			Assert.AreEqual(ExpectedArtist, wrt.Artist);

			string GeneratedXml = wrt.Build();
			Assert.IsTrue(
				GeneratedXml.Contains($"Artist=\"{ExpectedArtist}\"")
			);
		}
	}
}
