using System.Drawing;
using Un4seen.Bass.AddOn.Tags;

namespace TomiSoft.MP3Player.MediaInformation {
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

		public double Length {
			get {
				return this.tagInfo.duration;
			}
		}

		public bool IsLengthInSeconds {
			get {
				return true;
			}
		}


		public string Source {
			get {
				return this.tagInfo.filename;
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
