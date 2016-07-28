using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Provides functionality to read LRC-format lyrics files.
	/// </summary>
	class LrcReader {
		/// <summary>
		/// Gets the song's title that the lyrics is written for.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		public string Artist { get; private set; }

		/// <summary>
		/// Gets the album of the song.
		/// </summary>
		public string Album { get; private set; }

		/// <summary>
		/// Gets the creator of the song.
		/// </summary>
		public string Creator { get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the LrcReader class. Loads
		/// the lyrics from the given file.
		/// </summary>
		/// <param name="Filename">The path of the file to process.</param>
		public LrcReader(string Filename)
			:this(File.OpenRead(Filename)) {
			
		}

		/// <summary>
		/// Initializes a new instance of the LrcReader class. Loads
		/// the lyrics from the given stream.
		/// </summary>
		/// <param name="FileStream">The stream to process.</param>
		public LrcReader(Stream FileStream) {
			string Contents;

			using (StreamReader sr = new StreamReader(FileStream)) {
				Contents = sr.ReadToEnd();
			}

			this.ExtractMetadata(Contents);
			this.ExtractLyrics(Contents);
		}

		/// <summary>
		/// Extracts the metadata from the file.
		/// </summary>
		/// <param name="FileContents">The contents of the lyrics file.</param>
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

		/// <summary>
		/// Extracts the lyrics and timings from the file.
		/// </summary>
		/// <param name="FileContents">The contents of the file.</param>
		private void ExtractLyrics(string FileContents) {
			var Entries = FileContents.GetKeyValueMatches(@"\[(?<timestamp>\d+:\d+\.\d+)\](?<text>.*)\r?", "timestamp", "text");

			var Result = from c in Entries
						 orderby c.Key ascending
						 select c;
		}
	}
}
