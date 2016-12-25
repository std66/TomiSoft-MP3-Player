using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// This class can be used to load a lyrics file by determining
	/// its type.
	/// </summary>
	public class LyricsLoader {
		/// <summary>
		/// Determines the type of a lyrics file and parses its contents.
		/// </summary>
		/// <param name="Filename">The file's path to load.</param>
		/// <returns>An ILyricsReader instance if the file is a valid lyrics file or null if the file is not supported or not exists.</returns>
		public static ILyricsReader LoadFile(string Filename) {
			#region Error checking
			if (!File.Exists(Filename))
				return null;
			#endregion

			foreach (IFileFormatChecker Validator in GetValidators(Filename)) {
				if (Validator.ValidFile)
					return Validator.GetLyricsReader();
			}

			return null;
		}

		/// <summary>
		/// Gets all possible validators for the file.
		/// </summary>
		/// <param name="Filename">The file's path to be validated.</param>
		/// <returns>A sequence that contains all the possible lyrics file validators</returns>
		private static IEnumerable<IFileFormatChecker> GetValidators(string Filename) {
			return new IFileFormatChecker[] {
				new Lrc.LrcFileFormatChecker(Filename),
				new Xml.XmlFileFormatChecker(Filename)
			};
		}
	}
}
