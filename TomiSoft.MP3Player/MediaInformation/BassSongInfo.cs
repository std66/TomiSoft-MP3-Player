using System;
using System.Drawing;
using System.IO;
using TomiSoft.MP3Player.Communication;
using Un4seen.Bass;
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
				return String.IsNullOrWhiteSpace(tagInfo.album) ? null : tagInfo.album;
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
				return String.IsNullOrWhiteSpace(tagInfo.artist) ? null : tagInfo.artist;
			}
		}

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title {
			get {
				return String.IsNullOrWhiteSpace(tagInfo.title) ? null : tagInfo.title;
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
		/// Initializes a new instance of the <see cref="BassSongInfo"/> class.
		/// </summary>
		private BassSongInfo() {
			BassTags.ReadPictureTAGs = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BassSongInfo"/> class.
		/// </summary>
		/// <param name="Path">The file's path to read</param>
		public BassSongInfo(string Path) : this() {
			#region Error checking
			if (!File.Exists(Path))
				throw new FileNotFoundException($"{Path} does not exists.", Path);
			#endregion

			this.tagInfo = BassTags.BASS_TAG_GetFromFile(Path);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BassSongInfo"/> class.
		/// </summary>
		/// <param name="Info">A <see cref="RemoteResourceInfo"/> instance that holds informations about the requested URI.</param>
		public BassSongInfo(RemoteResourceInfo Info) {
			#region Error checking
			if (Info == null)
				throw new ArgumentNullException(nameof(Info));

			if (!Info.RequestSucceeded)
				throw new ArgumentException($"{nameof(Info)} represents a failed request.");

			if (Info.IsPlaylist)
				throw new ArgumentException($"{nameof(Info)} is a playlist, not a media resource.");
			#endregion
			
			if (Info.IsInternetRadioStream) {
				int Channel = Bass.BASS_StreamCreateURL(
					url:    Info.RequestUri.ToString(),
					offset: 0,
					flags:  BASSFlag.BASS_STREAM_STATUS,
					proc:   null,
					user:   IntPtr.Zero
				);

				if (Channel == 0)
					throw new Exception("Could not connect to the radio stream.");

				this.tagInfo = new TAG_INFO();
				if (!BassTags.BASS_TAG_GetFromURL(Channel, this.tagInfo))
					throw new Exception("Could not retrieve informations about the radio stream.");
			}
			else {
				this.tagInfo = BassTags.BASS_TAG_GetFromFile(Info.RequestUri.ToString());
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BassSongInfo"/> class.
		/// </summary>
		/// <param name="ChannelID">The BASS handle of the channel</param>
		/// <exception cref="ArgumentException">when ChannelID is 0</exception>
		public BassSongInfo(int ChannelID) : this() {
			#region Error checking
			if (ChannelID == 0)
				throw new ArgumentException($"{nameof(ChannelID)} cannot be 0.");
			#endregion

			this.tagInfo = new TAG_INFO();
			BassTags.BASS_TAG_GetFromFile(ChannelID, this.tagInfo);
		}
	}
}
