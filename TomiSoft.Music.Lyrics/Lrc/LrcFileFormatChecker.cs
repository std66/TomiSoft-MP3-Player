using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TomiSoft.Music.Lyrics.Lrc {
	/// <summary>
	/// This class can be used to determine whether a file is an
	/// LRC lyrics file.
	/// </summary>
	public class LrcFileFormatChecker : IFileFormatChecker {
		/// <summary>
		/// Stores that the given file is a valid LRC file.
		/// </summary>
		private bool validFile;

		/// <summary>
		/// Stores the file's path that is validated.
		/// </summary>
		private string filename;

		/// <summary>
		/// Gets if the file is a valid LRC file.
		/// </summary>
		public bool ValidFile {
			get {
				return this.validFile;
			}
		}

		/// <summary>
		/// Initializes a new instance of the LrcFileFormatChecker class.
		/// </summary>
		/// <param name="Filename">The file's full path which is checked</param>
		public LrcFileFormatChecker(string Filename) {
			this.validFile = this.Validate(File.ReadAllLines(Filename));
			this.filename = Filename;
		}

		/// <summary>
		/// Gets an instance of the LrcReader class.
		/// </summary>
		/// <returns>An LrcReader instance that parses the validated lyrics file</returns>
		/// <exception cref="InvalidOperationException">when the file is not a valid lyrics file</exception>
		public ILyricsReader GetLyricsReader() {
			#region Error checking
			if (!this.validFile)
				throw new InvalidOperationException($"The file is not a valid LRC file: {this.filename}");
			#endregion

			return new LrcReader(this.filename);
		}
		
		/// <summary>
		/// Determines that the file's content is a valid LRC file.
		/// </summary>
		/// <param name="FileContents">The contents of the file</param>
		/// <returns>True if the file is a valid LRC file, false if not</returns>
		private bool Validate(string[] FileContents) {
			Regex Metadata = new Regex(@"\[\w{2,}:\s*(.*)\]");
			Regex Lyrics = new Regex(@"\[(\d+:\d+\.\d+)\](.*?)\r?\n");

			foreach (string CurrentLine in FileContents) {
				bool IsEmptyLine = String.IsNullOrWhiteSpace(CurrentLine);
				bool IsMetadata = Metadata.IsMatch(CurrentLine);
				bool IsLyrics = Lyrics.IsMatch(CurrentLine);

				if (!IsEmptyLine && !IsMetadata && !IsLyrics)
					return false;
			}

			return true;
		}
	}
}
