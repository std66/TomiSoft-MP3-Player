using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Un4seen.Bass.AddOn.Tags;

namespace TomiSoft_MP3_Player {
	class SongInfo {
		public string Source { get; private set; }
		public string Title { get; private set; }
		public string Artist { get; private set; }
		public string AlbumArtist { get; private set; }
		public string Album { get; private set; }
		public int Year { get; private set; }
		public int TrackNo { get; private set; }
		public double Length { get; private set; }
		public bool IsLengthInSeconds { get; private set; }

		public SongInfo(string Source) {
			this.Source = Source;

			if (PlayerUtils.IsURI(Source)) {
				
			}
			else if (File.Exists(Source)) {
				string Extension = PlayerUtils.GetFileExtension(Source);
				if (BassManager.GetSupportedExtensions().Contains(Source)) {
					TAG_INFO info = BassTags.BASS_TAG_GetFromFile(Source);
					this.MapTagInfo(info);
				}
			}
		}

		private void MapTagInfo(TAG_INFO info) {
			this.Title = info.title;
			this.Album = info.album;
			this.Artist = info.artist;
			this.AlbumArtist = info.albumartist;
			this.TrackNo = Convert.ToInt32(info.track);
			this.Year = Convert.ToInt32(info.year);

			this.IsLengthInSeconds = false;
			this.Length = info.duration;
		}
	}
}
