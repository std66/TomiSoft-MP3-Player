using System;
using System.Linq;
using TomiSoft.MP3Player.Utils.Extensions;

namespace TomiSoft.MP3Player.Utils {
	public class YoutubeUri {
		private static readonly string RegexPattern = @"(?:youtube\.com\/\S*(?:(?:\/e(?:mbed))?\/|watch\?(?:\S*?&?v\=))|youtu\.be\/)(?<VideoID>[a-zA-Z0-9_-]{6,11})";

		/// <summary>
		/// Determines that an URI string is a valid YouTube URI.
		/// </summary>
		/// <param name="Uri">The URI to check</param>
		/// <returns>True if valid, false if not</returns>
		public static bool IsValidYoutubeUri(string Uri) {
			return Uri.IsMatch(RegexPattern);
		}

		/// <summary>
		/// Normalizes a YouTube video URI to a format something like
		/// https://www.youtube.com/watch?v=123456789ab
		/// </summary>
		/// <param name="Uri">The YouTube URI to normalize.</param>
		/// <returns>The normalized URI</returns>
		/// <exception cref="ArgumentException">when <paramref name="Uri"/> is not a valid YouTube URI</exception>
		public static string NormalizeUri(string Uri) {
			if (!IsValidYoutubeUri(Uri))
				throw new ArgumentException($"{nameof(Uri)} is not a valid YouTube URI");

			return $"https://www.youtube.com/watch?v={GetVideoID(Uri)}";
		}

		/// <summary>
		/// Creates a new YouTube URI that contains only the video ID.
		/// </summary>
		/// <param name="Uri">A YouTube URI to clean up.</param>
		/// <returns>A YouTube URI that contains only the video ID</returns>
		public static string GetVideoID(string Uri) {
			return Uri.GetNamedMatches(RegexPattern, "VideoID").First();
		}
	}
}
