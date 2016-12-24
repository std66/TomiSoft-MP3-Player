using System.IO;

namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// This interface represents a file format checker.
	/// </summary>
	public interface IFileFormatChecker {
		/// <summary>
		/// Gets if the file is a valid lyrics file.
		/// </summary>
		bool ValidFile { get; }

		/// <summary>
		/// Gets an instance of the corresponding lyrics reader class.
		/// </summary>
		/// <returns>An ILyricsReader instance that parses the validated lyrics file</returns>
		ILyricsReader GetLyricsReader();
	}
}
