using System.IO;

namespace TomiSoft.Music.LyricsTests.Xml {
	class SampleXmlLyrics {
		public static Stream GetIncorrectXmlLyricsFile() {
			return new StringMemoryStream(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
				<Lyrics Title='Arrow' Artist='Namine Ritsu' Album=''>
			");
		}

		public static Stream GetLyricsFileWithMetadataMissing() {
			return new StringMemoryStream(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
				<Lyrics>
					<Translations Default='none'>
						<Translation ID='none' Name='Not existing translation' />
					</Translations>
					<Lines />
				</Lyrics>
			");
		}

		public static Stream GetLyricsFileWithTranslationsTagMissing() {
			return new StringMemoryStream(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
				<Lyrics>
					<Lines />
				</Lyrics>
			");
		}

		public static Stream GetLyricsFileWithLinesTagMissing() {
			return new StringMemoryStream(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
				<Lyrics>
					<Translations Default='none'>
						<Translation ID='none' Name='Not existing translation' />
					</Translations>
				</Lyrics>
			");
		}

		public static Stream GetCorrectXmlLyricsFile() {
			return new StringMemoryStream(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
				<Lyrics Title='Arrow' Artist='Namine Ritsu' Album='Vocaloid songs'>
					<Translations Default='jp'>
						<Translation ID='jp' Name='Japanese' />
						<Translation ID='jp_romaji' Name='Japanese (romaji)' />
					</Translations>
	
					<Lines>
						<Line Start='00:22.19' End='00:26.23'>
							<Translation ID='jp'>完璧な空気に酔って秘める小悪魔の</Translation>
							<Translation ID='jp_romaji'>Kanpeki na kuuki ni yotte himeru koakuma no</Translation>
						</Line>

						<Line Start='00:26.24' End='00:29.29'>
							<Translation ID='jp'>隙間を狙ってく</Translation>
							<Translation ID='jp_romaji'>Sukima o neratteku</Translation>
						</Line>

						<Line Start='00:29.30' End='00:29.51'>
							<Translation ID='jp'></Translation>
							<Translation ID='jp_romaji'></Translation>
						</Line>

						<Line Start='00:29.52' End='00:33.57'>
							<Translation ID='jp'>夢に見た未来の凄い装置で</Translation>
							<Translation ID='jp_romaji'>Yume ni mita mirai no sugoi souchi de</Translation>
						</Line>

						<Line Start='00:33.57' End='00:37.83'>
							<Translation ID='jp'>どんな世界も作り出せる</Translation>
							<Translation ID='jp_romaji'>Donna sekai mo tsukuridaseru</Translation>
						</Line>

						<!-- Empty line Start='00:37.84' End='00:38.36' -->

						<Line Start='00:38.37' End='00:44.38'>
							<Translation ID='jp'>この星は弾けるソーダのような</Translation>
							<Translation ID='jp_romaji'>Kono hoshi wa hajikeru sou da no you na</Translation>
						</Line>
					</Lines>
				</Lyrics>
			");
		}
	}
}
