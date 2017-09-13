using System.IO;

namespace TomiSoft.Music.LyricsTests {
	class StringMemoryStream : MemoryStream {
		public StringMemoryStream(string Contents) {
			StreamWriter Writer = new StreamWriter(this) {
				AutoFlush = true
			};

			Writer.Write("\xfeff");
			Writer.Write(Contents);
			this.Seek(0, SeekOrigin.Begin);
		}
	}
}
