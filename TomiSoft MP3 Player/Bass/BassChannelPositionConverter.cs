using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	public class BassChannelPositionConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (value is long) {
				BassPlaybackAbstract LastInstance = PlaybackFactory.GetLastInstance() as BassPlaybackAbstract;
				if (LastInstance != null) {
					double Seconds = (int)Bass.BASS_ChannelBytes2Seconds(LastInstance.ChannelID, (long)value);
					return TimeSpan.FromSeconds(Seconds).ToString();
				}
			}

			return "00:00:00";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return 0;
		}
	}
}
