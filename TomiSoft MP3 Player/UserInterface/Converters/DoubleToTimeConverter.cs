using System;
using System.Windows.Data;
using TomiSoft.MP3Player.Playback;
using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	public class DoubleToTimeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is double) {
				IPlaybackManager LastInstance = PlaybackFactory.GetLastInstance();
				if (LastInstance != null) {
					double Seconds = (double)value;
					var t = TimeSpan.FromSeconds(Seconds);
					return String.Format(
						"{0}:{1}:{2}", 
						t.Hours.ToString("d2"), 
						t.Minutes.ToString("d2"), 
						t.Seconds.ToString("d2")
					);
				}
			}

			return "00:00:00";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return 0;
		}
	}
}
