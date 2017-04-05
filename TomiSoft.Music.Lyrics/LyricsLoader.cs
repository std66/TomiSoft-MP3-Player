using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TomiSoft.Music.Lyrics {
    /// <summary>
    /// This class can be used to load a lyrics file by determining
    /// its type.
    /// </summary>
    public class LyricsLoader {
        /// <summary>
        /// Gets all the supported file extensions (without the prepending dot, in lowercase).
        /// </summary>
        public static IEnumerable<string> SupportedExtensions {
            get {
                return new string[] {
                    "lrc",
                    "xml"
                };
            }
        }

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
				try {
					if (Validator.ValidFile)
						return Validator.GetLyricsReader();
				}
				catch {
					continue;
				}
			}

			return null;
		}

        /// <summary>
        /// Gets the path to all the lyrics files that possibly found in the specified directory
        /// </summary>
        /// <param name="Dir">The directory in which the files are searched.</param>
        /// <returns>
        /// A sequence of file paths.
        /// An empty sequence is returned if the specified directory does not exists.
        /// </returns>
        public static IEnumerable<string> FindAllLyricsFiles(string Dir) {
            #region Error checking
            if (!Directory.Exists(Dir))
                return new string[0];
            #endregion

            return Directory.GetFiles(Dir).Where(
                x => LyricsLoader.SupportedExtensions.Contains(
                    new FileInfo(x).Extension.Substring(1)
                )
            );
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
