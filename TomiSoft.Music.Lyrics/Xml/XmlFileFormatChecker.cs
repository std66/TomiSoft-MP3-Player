using System;
using System.Xml.Linq;

namespace TomiSoft.Music.Lyrics.Xml {
	/// <summary>
	/// This class can be used to determine whether a file is an
	/// XML lyrics file.
	/// </summary>
	public class XmlFileFormatChecker : IFileFormatChecker {
		/// <summary>
		/// Stores the file's path that is validated.
		/// </summary>
		private string Filename;

		/// <summary>
		/// Stores that the given file is a valid XML lyrics file.
		/// </summary>
		private bool IsValid;

		/// <summary>
		/// Gets if the file is a valid XML lyrics file.
		/// </summary>
		public bool ValidFile {
			get {
				return this.IsValid;
			}
		}

		/// <summary>
		/// Gets an instance of the XmlLyricsReader class.
		/// </summary>
		/// <returns>An XmlLyricsReader instance that parses the validated lyrics file</returns>
		/// <exception cref="InvalidOperationException">when the file is not a valid lyrics file</exception>
		public ILyricsReader GetLyricsReader() {
			#region Error checking
			if (!this.IsValid)
				throw new InvalidOperationException($"The file is not a valid XML lyrics file: {this.Filename}");
			#endregion

			return new XmlLyricsReader(this.Filename);
		}

		/// <summary>
		/// Initializes a new instance of the XmlFileFormatChecker class.
		/// </summary>
		/// <param name="Filename">The file's full path which is checked</param>
		public XmlFileFormatChecker(string Filename) {
			this.Filename = Filename;

			try {
				XDocument doc = XDocument.Load(Filename);
				this.IsValid = true;
			}
			catch (Exception e) {
				this.IsValid = false;
			}
		}
	}
}
