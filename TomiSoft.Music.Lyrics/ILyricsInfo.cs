using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// Represents a lyrics handler.
	/// </summary>
	public interface ILyricsHandler {
		/// <summary>
		/// Gets if the used file format supports multiple translations.
		/// </summary>
		bool SupportsMultipleTranslations { get; }

		/// <summary>
		/// Gets the album of the song.
		/// </summary>
		string Album { get; }

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		string Artist { get; }

		/// <summary>
		/// Gets the default translation ID.
		/// </summary>
		string DefaultTranslationID { get; }

		/// <summary>
		/// Gets the length of the song that this lyrics fits for.
		/// </summary>
		double Length { get; }

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Gets or sets the currently used translation ID.
		/// </summary>
		string TranslationID { get; set; }

		/// <summary>
		/// Gets the translations supported by the lyrics file.
		/// </summary>
		IReadOnlyDictionary<string, string> Translations { get; }
	}
}
