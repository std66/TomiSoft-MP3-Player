using System;
using System.ComponentModel;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// This interface is for controlling playback.
	/// </summary>
	public interface IPlaybackManager : IDisposable, INotifyPropertyChanged {
		/// <summary>
		/// Gets informations about the song.
		/// </summary>
		ISongInfo SongInfo { get; }

		/// <summary>
		/// Gets the current playback position.
		/// </summary>
		long Position { get; set; }

		/// <summary>
		/// Gets the length of the song.
		/// </summary>
		long Length { get; }

		/// <summary>
		/// Gets whether the song is currently playing.
		/// </summary>
		bool IsPlaying { get; }
		
		/// <summary>
		/// Gets the current left peak level.
		/// </summary>
		int LeftPeak { get; }

		/// <summary>
		/// Gets the current right peak level.
		/// </summary>
		int RightPeak { get; }
		
		/// <summary>
		/// Gets or sets the playback volume.
		/// </summary>
		int Volume { get; set; }
		
		/// <summary>
		/// Starts or continues the playback.
		/// </summary>
		void Play();

		/// <summary>
		/// Stops the playback and sets the position to the beginning of the song.
		/// </summary>
		void Stop();

		/// <summary>
		/// Stops the playback but keeps the current playback position.
		/// </summary>
		void Pause();

		/// <summary>
		/// This event raises when the song is ended.
		/// </summary>
		event Action SongEnded;
	}
}
