using System.ComponentModel;

namespace TomiSoft.MP3Player.Common.Playback {
	/// <summary>
	/// This interface provides a way of managing audio peak meter.
	/// </summary>
	public interface IAudioPeakMeter : INotifyPropertyChanged {
		/// <summary>
		/// Gets the current left peak level.
		/// </summary>
		int LeftPeak { get; }

		/// <summary>
		/// Gets the current right peak level.
		/// </summary>
		int RightPeak { get; }

		/// <summary>
		/// Gets the maximum possible value of the peak meter.
		/// </summary>
		int Maximum { get; }
	}
}
