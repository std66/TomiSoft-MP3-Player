using System;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// This interface provides a skeleton for reading lyrics files.
	/// </summary>
	public interface ILyricsReader {
		/// <summary>
		/// Gets the album of the song.
		/// </summary>
		string Album { get; }

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		string Artist { get; }

		/// <summary>
		/// Gets the creator of the lyrics.
		/// </summary>
		string Creator { get; }

		/// <summary>
		/// Gets the song's title that the lyrics is written for.
		/// </summary>
		string Title { get; }

		/// <summary>
		/// Gets the current lyrics line for the given timestamp.
		/// </summary>
		/// <param name="Seconds">The timestamp in seconds.</param>
		/// <returns>The lyrics line at the given timestamp.</returns>
		string GetLyricsLine(double Seconds);
	}
}
