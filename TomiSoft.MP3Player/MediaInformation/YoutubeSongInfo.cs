using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
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
			get {
				return null;
			}
		}

		/// <summary>
		/// Gets the artist of the song. Always returns a <see cref="string"/> similar to "Source: YouTube".
		/// </summary>
		public string Artist {
			get {
				return "Forrás: YouTube";
			}
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
		}

		/// <summary>
		/// Gets an instance asynchronously of the <see cref="YoutubeSongInfo"/> class.
		/// </summary>
		/// <param name="Source">The URL of the video</param>
		/// <returns>A <see cref="YoutubeSongInfo"/> instance.</returns>
		public static async Task<YoutubeSongInfo> GetVideoInfoAsync(string Source) {
			string ApiKey = await GetApiKeyAsync();

			YouTubeService s = new YouTubeService(new BaseClientService.Initializer() {
				ApiKey = ApiKey,
				ApplicationName = App.Name
			});

			VideosResource.ListRequest Request = s.Videos.List("contentDetails,snippet");
			Request.Id = GetVideoID(Source);

			VideoListResponse Response = await Request.ExecuteAsync();

			return new YoutubeSongInfo(
				Source: Source,
				Title: Response.Items[0].Snippet.Title,
				DurationInSeconds: XmlConvert.ToTimeSpan(Response.Items[0].ContentDetails.Duration).TotalSeconds
			);
		}

		/// <summary>
		/// Gets the API key for YouTube Data APi access.
		/// </summary>
		/// <returns>The API key that is used to access the YouTube Data API.</returns>
		private static async Task<string> GetApiKeyAsync() {
			string ApiKey = null;
			using (WebClient cl = new WebClient()) {
				ApiKey = await cl.DownloadStringTaskAsync("http://tomisoft.site90.net/TomiSoft-MP3-Player/ApiKey.data");
			}

			return ApiKey;
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
