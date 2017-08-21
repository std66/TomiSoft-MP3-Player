using System;
using TomiSoft.MP3Player.Common.Playback;
using TomiSoft.MP3Player.MediaInformation;

namespace TomiSoft.MP3Player.Playback {
    /// <summary>
    /// Represents a playback method that does nothing.
    /// </summary>
    internal class NullPlayback : IPlaybackManager, IAudioPeakMeter {
		/// <summary>
		/// Gets or sets the playback position. Always returns 0.
		/// Setting this value has no effect at all.
		/// </summary>
		public double Position {
			get {
				return 0;
			}
			set {
				
			}
		}

		/// <summary>
		/// Gets the song's length. Always returns 0.1.
		/// </summary>
		public double Length {
			get { return 0.1; }
		}

		/// <summary>
		/// Gets whether the song is playing. Always returns false.
		/// </summary>
		public bool IsPlaying {
			get { return false; }
		}

		/// <summary>
		/// Gets whether the song is paused. Always returns false.
		/// </summary>
		public bool IsPaused {
			get { return false; }
		}

		/// <summary>
		/// Gets the left peak level. Always returns 0.
		/// </summary>
		public int LeftPeak {
			get { return 0; }
		}

		/// <summary>
		/// Gets the right peak level. Always returns 0.
		/// </summary>
		public int RightPeak {
			get { return 0; }
		}

		/// <summary>
		/// Gets or sets the playback volume.
		/// </summary>
		public int Volume {
			get;
			set;
		}

		/// <summary>
		/// Gets informations about the song. Always returns null.
		/// </summary>
		public ISongInfo SongInfo {
			get {
				return null;
			}
		}

		/// <summary>
		/// Gets the possible maximum value for the peak level. Always returns 0.
		/// </summary>
		public int Maximum {
			get {
				return 0;
			}
		}

		/// <summary>
		/// Dummy method for staring playback. This method does nothing.
		/// </summary>
		public void Play() {
			
		}

		/// <summary>
		/// Dummy method for stopping playback. This method does nothing.
		/// </summary>
		public void Stop() {
			
		}

		/// <summary>
		/// Dummy method for pausing playback. This method does nothing.
		/// </summary>
		public void Pause() {
			
		}

		/// <summary>
		/// Dummy method for disposing any allocated resources.
		/// This method does nothing.
		/// </summary>
		public void Dispose() {
			
		}

		/// <summary>
		/// Occures when a property is changed. Never fired.
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged {
			add { }
			remove { }
		}

		/// <summary>
		/// Occures when the song is ended. Never fired.
		/// </summary>
		public event Action SongEnded {
			add { }
			remove { }
		}
	}
}
