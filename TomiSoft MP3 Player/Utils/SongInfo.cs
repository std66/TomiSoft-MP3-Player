using System;
using System.Linq;
using System.IO;
using Un4seen.Bass.AddOn.Tags;
using TomiSoft.MP3Player.Playback;

namespace TomiSoft.MP3Player.Utils {
	/// <summary>
	/// Stores information about a song.
	/// </summary>
	public class SongInfo {
		/// <summary>
		/// Gets the source of the media (usually a file system path or URI)
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title { get; private set; }

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		public string Artist { get; private set; }

		/// <summary>
		/// Gets the artist of the album.
		/// </summary>
		public string AlbumArtist { get; private set; }

		/// <summary>
		/// Gets the title of the album.
		/// </summary>
		public string Album { get; private set; }

		/// <summary>
		/// Gets the year of release of the song.
		/// </summary>
		public int? Year { get; private set; }

		/// <summary>
		/// Gets the track number of the song in the album.
		/// </summary>
		public int? TrackNo { get; private set; }

		/// <summary>
		/// Gets the length of the song (in bytes or seconds, see IsLengthInSeconds).
		/// </summary>
		public double Length { get; private set; }

		/// <summary>
		/// Gets if the Length property is in bytes (false) or seconds (true).
		/// </summary>
		public bool IsLengthInSeconds { get; private set; }

		/// <summary>
		/// Determines the song information from the media at the given source.
		/// </summary>
		/// <param name="Source">A path or a URI to the media.</param>
		public SongInfo(string Source) {
			this.Source = Source;

			if (PlayerUtils.IsURI(Source)) {
				
			}
			else if (File.Exists(Source)) {
				string Extension = PlayerUtils.GetFileExtension(Source);
				if (BassManager.GetSupportedExtensions().Contains(Extension)) {
					TAG_INFO info = BassTags.BASS_TAG_GetFromFile(Source);
					this.MapTagInfo(info);
				}
			}
		}

		/// <summary>
		/// Loads the information from a BassTags.TAG_INFO instance.
		/// </summary>
		/// <param name="info">The TAG_INFO that stores the informations.</param>
		private void MapTagInfo(TAG_INFO info) {
			this.Title = info.title;
			this.Album = info.album;
			this.Artist = info.artist;
			this.AlbumArtist = info.albumartist;

            if (!String.IsNullOrWhiteSpace(info.track))
			    this.TrackNo = Convert.ToInt32(info.track);

            if (!String.IsNullOrWhiteSpace(info.year))
			    this.Year = Convert.ToInt32(info.year);

			this.IsLengthInSeconds = false;
			this.Length = info.duration;
		}
	}
}
