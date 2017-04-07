using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;

namespace TomiSoft.MP3Player.Playback {
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
		double Position { get; set; }

		/// <summary>
		/// Gets the length of the song.
		/// </summary>
		double Length { get; }

		/// <summary>
		/// Gets whether the song is currently playing.
		/// </summary>
		bool IsPlaying { get; }

		/// <summary>
		/// Gets whether the song is currently paused.
		/// </summary>
		bool IsPaused { get; }
		
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
