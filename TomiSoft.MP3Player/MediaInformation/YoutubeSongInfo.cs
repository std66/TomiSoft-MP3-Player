using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TomiSoft.ExternalApis.YoutubeDl;
using TomiSoft.ExternalApis.YoutubeDl.MediaInformation;
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
		/// Gets the artist of the song. Always returns a <see cref="string"/> similar to "Source: YouTube".
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
		/// Initializes a new instance of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URI of the video</param>
		/// <param name="Title">The title of the video</param>
		/// <param name="DurationInSeconds">The length of the video in seconds</param>
		private YoutubeSongInfo(string Source, string Title, double DurationInSeconds) {
			this.Source = Source;
			this.Title = Title;
			this.Length = DurationInSeconds;
			this.Artist = "Forrás: YouTube";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URI of the video</param>
		/// <param name="Title">The title of the video</param>
		/// <param name="Artist">The artist of the video</param>
		/// <param name="DurationInSeconds">The length of the video in seconds</param>
		private YoutubeSongInfo(string Source, string Title, string Artist, double DurationInSeconds)
			:this (Source, Title, DurationInSeconds) {
			this.Artist = Artist ?? "Forrás: YouTube";
		}

		/// <summary>
		/// Gets an instance asynchronously of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URL of the video</param>
		/// <returns>A <see cref="YoutubeSongInfo"/> instance.</returns>
		public static async Task<YoutubeSongInfo> GetVideoInfoAsync(string Source) {
			YoutubeDl d = new YoutubeDl("youtube-dl.exe", App.Path) {
				VideoID = GetVideoID(Source)
			};

			YoutubeMediaInfo r = await d.GetVideoInfo();

			var Result = new YoutubeSongInfo(
				Source: Source,
				Title: r.RecognizedMedia?.Title ?? r.Title,
				Artist: r.RecognizedMedia?.Artist,
				DurationInSeconds: r.Duration.TotalSeconds
			);

			if (App.Config.YoutubeDownloadThumbnail)
				Result.AlbumImage = await r.Thumbnails.FirstOrDefault()?.DownloadAsImageAsync();

			return Result;
		}

		/// <summary>
		/// Gets the YouTube Video ID from the url of the video.
		/// </summary>
		/// <param name="url">The url of the video.</param>
		/// <returns>The ID of the video</returns>
		private static string GetVideoID(string url) {
			Uri u = new Uri(url);
			return HttpUtility.ParseQueryString(u.Query).GetValues("v").FirstOrDefault();
		}
	}
}
