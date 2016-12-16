using System.Drawing;
using Un4seen.Bass.AddOn.Tags;

namespace TomiSoft.MP3Player.MediaInformation {
	/// <summary>
	/// Provides information of a song using BassTag plugin.
	/// </summary>
	public class BassSongInfo : ISongInfo {
		/// <summary>
		/// Stores the metadatas of the song.
		/// </summary>
		private TAG_INFO tagInfo;

		/// <summary>
		/// Gets the album name of the song.
		/// </summary>
		public string Album {
			get {
				return tagInfo.album;
			}
		}

		/// <summary>
		/// Gets the cover image of the album. If there is no image given,
		/// null is returned.
		/// </summary>
		public Image AlbumImage {
			get {
				return (tagInfo.PictureCount > 0) ? tagInfo.PictureGetImage(0) : null;
			}
		}

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		public string Artist {
			get {
				return tagInfo.artist;
			}
		}

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title {
			get {
				return tagInfo.title;
			}
		}

		/// <summary>
		/// Gets the length of the song in seconds.
		/// </summary>
		public double Length {
			get {
				return this.tagInfo.duration;
			}
		}

		/// <summary>
		/// Returns true if the Length is represented in seconds,
		/// false if the Length is represented in bytes.
		/// </summary>
		public bool IsLengthInSeconds {
			get {
				return true;
			}
		}

		/// <summary>
		/// Gets the file's path or URI.
		/// </summary>
		public string Source {
			get {
				return this.tagInfo.filename;
			}
		}

		/// <summary>
		/// Initializes a new instance of the BassSongInfo class.
		/// </summary>
		private BassSongInfo() {
			BassTags.ReadPictureTAGs = true;
		}

		/// <summary>
		/// Initializes a new instance of the BassSongInfo class.
		/// </summary>
		/// <param name="Source">The file's name or URI to read</param>
		public BassSongInfo(string Source) : this() {
			this.tagInfo = BassTags.BASS_TAG_GetFromFile(Source);
		}

		/// <summary>
		/// Initializes a new instance of the BassSongInfo class.
		/// </summary>
		/// <param name="ChannelID">The BASS handle of the channel</param>
		public BassSongInfo(int ChannelID) : this() {
			this.tagInfo = new TAG_INFO();
			BassTags.BASS_TAG_GetFromFile(ChannelID, this.tagInfo);
		}
	}
}
