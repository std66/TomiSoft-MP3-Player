using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TomiSoft.Music.LyricsTests.Xml;

namespace TomiSoft.Music.Lyrics.Xml.Tests {
	[TestClass()]
	public class XmlLyricsReaderTests {
		[TestMethod()]
		public void XmlLyricsReader_CanLoadCorrectFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				try {
					new XmlLyricsReader(LyricsStream);
				}
				catch (Exception e) {
					Assert.Fail(e.Message, e);
				}
			}
		}

		[TestMethod()]
		[ExpectedException(typeof(XmlException))]
		public void XmlLyricsReader_CannotLoadInvalidFile_NonWellFormattedXml() {
			using (Stream LyricsStream = SampleXmlLyrics.GetIncorrectXmlLyricsFile()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod()]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_CannotLoadInvalidFile_TranslationsTagMissing() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithTranslationsTagMissing()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod()]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_CannotLoadInvalidFile_LinesTagMissing() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithLinesTagMissing()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_SupportsMultipleTranslations() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(true, Reader.SupportsMultipleTranslations);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetTranslations() {
			Dictionary<string, string> Expected = new Dictionary<string, string>() {
				{ "jp_romaji", "Japanese (romaji)" },
				{ "jp", "Japanese" }
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				
				Assert.IsTrue(
					Expected.Count == Reader.Translations.Count && !Expected.Except(Reader.Translations).Any()
				);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_DefaultTranslationID() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("jp", Reader.DefaultTranslationID);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetTranslationID() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("jp", Reader.TranslationID);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_SetTranslationID_ToAnExistingTranslation() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Reader.TranslationID = "jp_romaji";
				Assert.AreEqual("jp_romaji", Reader.TranslationID);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetTitle() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Arrow", Reader.Title);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetTitle_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Title);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetArtist() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Namine Ritsu", Reader.Artist);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetArtist_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Artist);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetAlbum() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Vocaloid songs", Reader.Album);
			}
		}

		[TestMethod]
		public void XmlLyricsReader_LyricsMetadata_GetAlbum_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Album);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_SetTranslationID_ToANonExistingTranslation() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Reader.TranslationID = "NonExistingTranslationID";
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetLyricsLine_WithDefaultTranslation() {
			Dictionary<double, List<string>> Tests = new Dictionary<double, List<string>>() {
				{
					22.19,
					new List<string> { "完璧な空気に酔って秘める小悪魔の" }
				},
				{
					23.15,
					new List<string> { "完璧な空気に酔って秘める小悪魔の" }
				},
				{
					26.23,
					new List<string> { "完璧な空気に酔って秘める小悪魔の" }
				}
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				
				foreach (var Expected in Tests) {
					var Actual = Reader.GetLyricsLine(Expected.Key);
					Assert.IsTrue(
						Enumerable.SequenceEqual(Expected.Value, Actual)
					);
				}
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetLyricsLine_WithNonDefaultTranslation() {
			Dictionary<double, List<string>> Tests = new Dictionary<double, List<string>>() {
				{
					22.19,
					new List<string> { "Kanpeki na kuuki ni yotte himeru koakuma no" }
				},
				{
					23.15,
					new List<string> { "Kanpeki na kuuki ni yotte himeru koakuma no" }
				},
				{
					26.23,
					new List<string> { "Kanpeki na kuuki ni yotte himeru koakuma no" }
				}
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream) {
					TranslationID = "jp_romaji"
				};

				foreach (var Expected in Tests) {
					var Actual = Reader.GetLyricsLine(Expected.Key);
					Assert.IsTrue(
						Enumerable.SequenceEqual(Expected.Value, Actual)
					);
				}
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetLyricsLine_MultipleLinesWithDefaultTranslation() {
			Dictionary<double, List<string>> Tests = new Dictionary<double, List<string>>() {
				{
					33.57,
					new List<string> {
						"夢に見た未来の凄い装置で",
						"どんな世界も作り出せる"
					}
				},
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);

				foreach (var Expected in Tests) {
					var Actual = Reader.GetLyricsLine(Expected.Key);
					Assert.IsTrue(
						Enumerable.SequenceEqual(Expected.Value, Actual)
					);
				}
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetLyricsLine_UndefinedLines() {
			double[] TimestampsForEmptyLines = {
				37.84, 37.90, 38.36
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);

				foreach (var Timestamp in TimestampsForEmptyLines) {
					var Actual = Reader.GetLyricsLine(Timestamp);
					Assert.AreEqual(0, Actual.Count());
				}
			}
		}

		[TestMethod]
		public void XmlLyricsReader_GetLyricsLine_DefinedEmptyLines() {
			double[] TimestampsForEmptyLines = {
				29.30, 29.30, 29.51
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);

				foreach (var Timestamp in TimestampsForEmptyLines) {
					var Actual = Reader.GetLyricsLine(Timestamp);
					Assert.IsTrue(
						Enumerable.SequenceEqual(
							new List<string> { String.Empty },
							Actual
						)
					);
				}
			}
		}
	}
}