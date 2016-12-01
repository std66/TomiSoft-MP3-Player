namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// Represents a line of a lyrics.
	/// </summary>
	public interface ILyricsLine {
		/// <summary>
		/// The start time of the line in seconds.
		/// </summary>
		double StartTime { get; }

		/// <summary>
		/// The end time of the line in seconds.
		/// </summary>
		double EndTime { get; }

		/// <summary>
		/// The line text of the lyrics.
		/// </summary>
		string Text { get; }
	}
}
