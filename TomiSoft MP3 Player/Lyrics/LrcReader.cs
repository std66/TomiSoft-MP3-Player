using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TomiSoft_MP3_Player {
	class LrcReader {
		public string Title { get; private set; }
		public string Artist { get; private set; }
		public string Album { get; private set; }
		public string Creator { get; private set; }
		
		public LrcReader(string Filename)
			:this(File.OpenRead(Filename)) {
			
		}

		public LrcReader(Stream FileStream) {
			string Contents;

			using (StreamReader sr = new StreamReader(FileStream)) {
				Contents = sr.ReadToEnd();
			}

			this.ExtractMetadata(Contents);
			this.ExtractLyrics(Contents);
		}

		private void ExtractMetadata(string FileContents) {
			string BasePattern = @"\[{0}:\s*(.*)\]";

			Dictionary<string, string> Properties = new Dictionary<string, string>() {
														{"Title", "ti"},
														{"Artist", "ar"},
														{"Album", "al"},
														{"Creator", "by"}
													};

			foreach (var Property in Properties) {
				this.GetType().GetProperty(Property.Key).SetValue(
					this,
					FileContents.GetFirstMatch(String.Format(BasePattern, Property.Value))
				);
			}
		}

		private void ExtractLyrics(string FileContents) {
			var Entries = FileContents.GetKeyValueMatches(@"\[(?<timestamp>\d+:\d+\.\d+)\](?<text>.*)\r?", "timestamp", "text");

			var Result = from c in Entries
						 orderby c.Key ascending
						 select c;
		}
	}
}
