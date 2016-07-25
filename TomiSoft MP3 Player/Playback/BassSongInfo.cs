using System.Drawing;
using Un4seen.Bass.AddOn.Tags;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Provides information of a song using BassTag plugin.
	/// </summary>
	public class BassSongInfo : ISongInfo {
		private TAG_INFO tagInfo;

		public string Album {
			get {
				return tagInfo.album;
			}
		}

		public Image AlbumImage {
			get {
				return (tagInfo.PictureCount > 0) ? tagInfo.PictureGetImage(0) : null;
			}
		}

		public string Artist {
			get {
				return tagInfo.artist;
			}
		}

		public string Title {
			get {
				return tagInfo.title;
			}
		}

		public BassSongInfo(string Filename) {
			BassTags.ReadPictureTAGs = true;
			this.tagInfo = BassTags.BASS_TAG_GetFromFile(Filename);
		}

		public BassSongInfo(int ChannelID) {
			BassTags.ReadPictureTAGs = true;
			BassTags.BASS_TAG_GetFromFile(ChannelID, this.tagInfo);
		}
	}
}
