namespace TomiSoft.Music.Lyrics.Xml {
	/// <summary>
	/// Provides informations about the XML declarations that may appear in
	/// an XML lyrics file.
	/// </summary>
	public static class XmlLyrics {
		/// <summary>
		/// Stores the default XML namespace.
		/// </summary>
		public const string XmlNamespace = "https://github.com/std66/TomiSoft-MP3-Player";

		/// <summary>
		/// Stores the URI of the XML Schema that can be used to validate the XML lyrics files.
		/// </summary>
		public const string XmlSchemaLocation = "https://raw.githubusercontent.com/std66/TomiSoft-MP3-Player/master/TomiSoft.Music.Lyrics/XmlLyricsSchema.xsd";
	}
}
