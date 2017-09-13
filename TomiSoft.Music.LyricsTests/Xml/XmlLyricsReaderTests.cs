using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TomiSoft.Music.LyricsTests.Xml;

namespace TomiSoft.Music.Lyrics.Xml.Tests {
	[TestClass]
	public class XmlLyricsReaderTests {
		[TestMethod]
		[TestCategory("XML Lyrics Processing - loading and parsing")]
		public void XmlLyricsReader_CanLoadCorrectFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - loading and parsing")]
		public void XmlLyricsReader_CanLoadCorrectFile_WithMissingSongMetadata() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - loading and parsing")]
		[ExpectedException(typeof(XmlException))]
		public void XmlLyricsReader_CannotLoadInvalidFile_NonWellFormattedXml() {
			using (Stream LyricsStream = SampleXmlLyrics.GetIncorrectXmlLyricsFile()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - loading and parsing")]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_CannotLoadInvalidFile_TranslationsTagMissing() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithTranslationsTagMissing()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - loading and parsing")]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_CannotLoadInvalidFile_LinesTagMissing() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithLinesTagMissing()) {
				new XmlLyricsReader(LyricsStream);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - translation support")]
		public void XmlLyricsReader_SupportsMultipleTranslations() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(true, Reader.SupportsMultipleTranslations);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - translation support")]
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
		[TestCategory("XML Lyrics Processing - translation support")]
		public void XmlLyricsReader_GetDefaultTranslationID() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("jp", Reader.DefaultTranslationID);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - translation support")]
		public void XmlLyricsReader_GetTranslationID() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("jp", Reader.TranslationID);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - translation support")]
		public void XmlLyricsReader_SetTranslationID_ToAnExistingTranslation() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Reader.TranslationID = "jp_romaji";
				Assert.AreEqual("jp_romaji", Reader.TranslationID);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetTitle() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Arrow", Reader.Title);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetTitle_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Title);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetArtist() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Namine Ritsu", Reader.Artist);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetArtist_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Artist);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetAlbum() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual("Vocaloid songs", Reader.Album);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - song metadata support")]
		public void XmlLyricsReader_LyricsMetadata_GetAlbum_MissingMetadataInFile() {
			using (Stream LyricsStream = SampleXmlLyrics.GetLyricsFileWithMetadataMissing()) {
				ILyricsMetadata Reader = new XmlLyricsReader(LyricsStream);
				Assert.AreEqual(null, Reader.Album);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - translation support")]
		[ExpectedException(typeof(Exception))]
		public void XmlLyricsReader_SetTranslationID_ToANonExistingTranslation() {
			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);
				Reader.TranslationID = "NonExistingTranslationID";
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
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
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
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
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
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
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
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
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
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

		[TestMethod]
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
		public void XmlLyricsReader_GetAllLines_WithDefaultTranslationID() {
			ILyricsLine[] Expected = new LyricsLine[] {
				new LyricsLine(22.19, 26.23, "完璧な空気に酔って秘める小悪魔の"),
				new LyricsLine(26.24, 29.29, "隙間を狙ってく"),
				new LyricsLine(29.30, 29.51, ""),
				new LyricsLine(29.52, 33.57, "夢に見た未来の凄い装置で"),
				new LyricsLine(33.57, 37.83, "どんな世界も作り出せる"),
				new LyricsLine(38.37, 44.38, "この星は弾けるソーダのような")
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream);

				Assert.IsTrue(
					Enumerable.SequenceEqual(
						Expected,
						Reader.GetAllLines()
					)
				);
			}
		}

		[TestMethod]
		[TestCategory("XML Lyrics Processing - lyrics text reading")]
		public void XmlLyricsReader_GetAllLines_WithNonDefaultTranslationID() {
			ILyricsLine[] Expected = new LyricsLine[] {
				new LyricsLine(22.19, 26.23, "Kanpeki na kuuki ni yotte himeru koakuma no"),
				new LyricsLine(26.24, 29.29, "Sukima o neratteku"),
				new LyricsLine(29.30, 29.51, ""),
				new LyricsLine(29.52, 33.57, "Yume ni mita mirai no sugoi souchi de"),
				new LyricsLine(33.57, 37.83, "Donna sekai mo tsukuridaseru"),
				new LyricsLine(38.37, 44.38, "Kono hoshi wa hajikeru sou da no you na")
			};

			using (Stream LyricsStream = SampleXmlLyrics.GetCorrectXmlLyricsFile()) {
				ILyricsReader Reader = new XmlLyricsReader(LyricsStream) {
					TranslationID = "jp_romaji"
				};

				Assert.IsTrue(
					Enumerable.SequenceEqual(
						Expected,
						Reader.GetAllLines()
					)
				);
			}
		}
	}
}