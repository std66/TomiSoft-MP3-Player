using System;
using System.Collections.Generic;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class YoutubeMediaInfo {
		public int Likes { get; private set; }
		public int Dislikes { get; private set; }
		public List<Thumbnail> Thumbnails { get; } = new List<Thumbnail>();
		public string Description { get; private set; }
		public string VideoID { get; private set; }
		public string Title { get; private set; }
		public List<string> Tags { get; } = new List<string>();
		public TimeSpan Duration { get; private set; }
		public SystemRecognizedMedia RecognizedMedia { get; private set; }

		private YoutubeMediaInfo() {

		}

		internal static YoutubeMediaInfo FromDynamicJson(dynamic Source) {
			var Result = new YoutubeMediaInfo {
				Likes = Convert.ToInt32(Source.like_count),
				Dislikes = Convert.ToInt32(Source.dislike_count),
				VideoID = Source.id,
				Description = Source.description,
				Title = Source.fulltitle,
				Duration = TimeSpan.FromSeconds(Convert.ToDouble(Source.duration))
			};

			if (Source.alt_title != null || Source.creator != null) {
				Result.RecognizedMedia = new SystemRecognizedMedia {
					Title = Source.alt_title,
					Artist = Source.creator
				};
			}
			
			foreach (dynamic thumb in Source.thumbnails) {
				Result.Thumbnails.Add(
					new Thumbnail {
						ID = Convert.ToInt32(thumb.id),
						Uri = thumb.url
					}
				);
			}

			return Result;
		}
	}
}
