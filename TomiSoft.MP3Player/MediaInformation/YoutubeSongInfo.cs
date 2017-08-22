using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TomiSoft.ExternalApis.YoutubeDl;
using TomiSoft.ExternalApis.YoutubeDl.MediaInformation;
using TomiSoft.MP3Player.Utils;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.MediaInformation {
	/// <summary>
	/// Gets informations about a YouTube media.
	/// </summary>
	class YoutubeSongInfo : ISongInfo {
		/// <summary>
		/// Gets the album of the song. Always returns null.
		/// </summary>
		public string Album {
			get {
				return null;
			}
		}

		/// <summary>
		/// Gets the cover image of the album. Always returns null.
		/// </summary>
		public Image AlbumImage {
			get;
			private set;
		}

		/// <summary>
		/// Gets the artist of the song.
		/// </summary>
		public string Artist {
			get;
			private set;
		}

		/// <summary>
		/// Gets whether the media length is represented in seconds.
		/// </summary>
		public bool IsLengthInSeconds {
			get {
				return true;
			}
		}

		/// <summary>
		/// Gets the length of the song.
		/// </summary>
		public double Length {
			get;
			private set;
		}

		/// <summary>
		/// Gets the source of the song.
		/// </summary>
		public string Source {
			get;
			private set;
		}

		/// <summary>
		/// Gets the title of the song.
		/// </summary>
		public string Title {
			get;
			private set;
		}

		/// <summary>
		/// Gets all available media formats.
		/// </summary>
		public IEnumerable<MediaFormat> Formats {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URI of the video</param>
		/// <param name="Title">The title of the video</param>
		/// <param name="DurationInSeconds">The length of the video in seconds</param>
		private YoutubeSongInfo(string Source, string Title, double DurationInSeconds) {
			this.Source = Source;
			this.Title = Title;
			this.Length = DurationInSeconds;
			this.Artist = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URI of the video</param>
		/// <param name="Title">The title of the video</param>
		/// <param name="Artist">The artist of the video</param>
		/// <param name="DurationInSeconds">The length of the video in seconds</param>
		private YoutubeSongInfo(string Source, string Title, string Artist, double DurationInSeconds)
			: this(Source, Title, DurationInSeconds) {
			this.Artist = Artist;
		}

		/// <summary>
		/// Gets an instance asynchronously of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URL of the video</param>
		/// <returns>A <see cref="YoutubeSongInfo"/> instance.</returns>
		public static async Task<YoutubeSongInfo> GetVideoInfoAsync(string Source) {
			YoutubeDl d = new YoutubeDl("youtube-dl.exe", App.Path) {
				VideoID = YoutubeUri.GetVideoID(Source)
			};

			YoutubeMediaInfo r = await d.GetVideoInfo();

			var Result = new YoutubeSongInfo(
				Source: Source,
				Title: r.RecognizedMedia?.Title ?? r.Title,
				Artist: r.RecognizedMedia?.Artist,
				DurationInSeconds: r.Duration.TotalSeconds
			) {
				Formats = r.MediaFormats
			};

			if (App.Config.YoutubeDownloadThumbnail)
				Result.AlbumImage = await r.Thumbnails.FirstOrDefault()?.DownloadAsImageAsync();

			return Result;
		}

		public MediaFormat GetBestAudioFormat() {
			return this.Formats.Where(x => x.Format == MediaType.Audio).OrderByDescending(x => ((DashAudio)x).SamplingRate).FirstOrDefault();
		}
	}
}
