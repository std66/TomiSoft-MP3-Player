using System.Collections.Generic;

namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// Represents a lyrics reader.
	/// </summary>
	public interface ILyricsReader {
		/// <summary>
		/// Gets if the used file format supports multiple translations.
		/// </summary>
		bool SupportsMultipleTranslations { get; }
        
        /// <summary>
        /// Gets the default translation ID.
        /// </summary>
        string DefaultTranslationID { get; }
        
		/// <summary>
		/// Gets or sets the currently used translation ID.
		/// </summary>
		string TranslationID { get; set; }

		/// <summary>
		/// Gets the translations supported by the lyrics file.
		/// </summary>
		IReadOnlyDictionary<string, string> Translations { get; }

		/// <summary>
		/// Gets a lyrics line that is displayed at the given playback
		/// position. Note that this method may return multiple lines
		/// if there are multiple singers at the given playback position.
		/// </summary>
		/// <param name="Seconds">The playback position is seconds</param>
		/// <returns>The lyrics to display.</returns>
		IEnumerable<string> GetLyricsLine(double Seconds);

		/// <summary>
		/// Gets all the lyrics lines of the selected translation.
		/// </summary>
		/// <returns>A sequence of lyrics lines.</returns>
		IEnumerable<ILyricsLine> GetAllLines();
	}
}