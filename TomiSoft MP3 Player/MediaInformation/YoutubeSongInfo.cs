using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.MediaInformation {
	class YoutubeSongInfo : ISongInfo {
		public string Album {
			get {
				throw new NotImplementedException();
			}
		}

		public Image AlbumImage {
			get {
				return null;
			}
		}

		public string Artist {
			get {
				return "Forrás: YouTube";
			}
		}

		public bool IsLengthInSeconds {
			get {
				return true;
			}
		}

		public double Length {
			get {
				return 0;
			}
		}

		public string Source {
			get;
			private set;
		}

		public string Title {
			get;
			private set;
		}

		public YoutubeSongInfo(string Source) {
			this.Source = Source;
			this.Title = Source;
		}
	}
}
