using System;

namespace TomiSoft_MP3_Player {
	class NullPlayback : IPlaybackManager {

		public long Position {
			get {
				return 0;
			}
			set {
				
			}
		}

		public long Length {
			get { return 1; }
		}

		public bool IsPlaying {
			get { return false; }
		}

		public int LeftPeak {
			get { return 0; }
		}

		public int RightPeak {
			get { return 0; }
		}

		public int Volume {
			get;
			set;
		}

		public ISongInfo SongInfo {
			get {
				return null;
			}
		}

		public void Play() {
			
		}

		public void Stop() {
			
		}

		public void Pause() {
			
		}

		public void Dispose() {
			
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public event Action SongEnded;
	}
}
