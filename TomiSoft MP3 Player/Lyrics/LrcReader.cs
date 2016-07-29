using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Provides functionality to read LRC-format lyrics files.
	/// </summary>
	public class LrcReader : ILyricsReader {
		private Dictionary<double, string> Lyrics;

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
		/// Gets the creator of the lyrics.
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
		/// Gets the current lyrics line for the given timestamp.
		/// </summary>
		/// <param name="Seconds">The timestamp in seconds.</param>
		/// <returns>The lyrics line at the given timestamp.</returns>
		public string GetLyricsLine(double Seconds) {
			int i;
			for (i = 0; i < this.Lyrics.Count; i++) {
				if (this.Lyrics.ElementAt(i).Key > Seconds)
					break;
			}

			return (i == 0) ? String.Empty : this.Lyrics.ElementAt(i - 1).Value;
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
			string Pattern = @"\[(?<timestamp>\d+:\d+\.\d+)\](?<text>.+?)\r?\n";
			var Entries = FileContents.GetKeyValueMatches(Pattern, "timestamp", "text");

			var Result = from c in Entries
						 orderby c.Key ascending
						 select c;

			this.Lyrics = Result.ToDictionary(x => this.TimestampToSeconds(x.Key), y => y.Value);
		}

		/// <summary>
		/// Converts an LRC timestamp to seconds.
		/// </summary>
		/// <param name="Time">The timestamp to parse</param>
		/// <returns>The timestamp in seconds</returns>
		private double TimestampToSeconds(string Time) {
			string Pattern = @"(?<mins>\d{2}):(?<secs>\d{2}).(?<msecs>\d{2})";
			var Matches = Regex.Matches(Time, Pattern);

			int Mins = Convert.ToInt32(Matches[0].Groups["mins"].Value);
			int Secs = Convert.ToInt32(Matches[0].Groups["secs"].Value);
			int MSecs = Convert.ToInt32(Matches[0].Groups["msecs"].Value);

			return Mins * 60 + Secs + (MSecs / 100.0);
		}
	}
}
