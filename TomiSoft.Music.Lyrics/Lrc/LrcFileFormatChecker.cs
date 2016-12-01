using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Music.Lyrics.Lrc {
	/// <summary>
	/// This class can be used to determine whether a file is an
	/// LRC lyrics file.
	/// </summary>
	public class LrcFileFormatChecker : IFileFormatChecker {
		private bool validFile;

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
			this.validFile = this.Validate(File.ReadAllText(Filename));
		}

		private bool Validate(string FileContents) {
			return FileContents.Matches(@"^(^\[[a-zA-Z]{2}:(.+)?\]|\[\d{2}:\d{2}.\d{2}\](.+)?|\r?\n)$");
		}
	}
}
