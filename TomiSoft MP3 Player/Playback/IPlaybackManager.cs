using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft_MP3_Player {
	public interface IPlaybackManager : IDisposable, INotifyPropertyChanged {
		long Position { get; set; }
		long Length { get; }
		bool IsPlaying { get; }
		
		int LeftPeak { get; }
		int RightPeak { get; }
		
		int Volume { get; set; }
		
		void Play();
		void Stop();
		void Pause();

		event Action SongEnded;
	}
}
