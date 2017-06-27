using System;
using System.Collections.Generic;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class YoutubeMediaInfo {
		public VideoRating           Rating          { get; private set; }
		public List<Thumbnail>       Thumbnails      { get;              } = new List<Thumbnail>();
		public string                Description     { get; private set; }
		public string                VideoID         { get; private set; }
		public string                Title           { get; private set; }
		public string                License         { get; private set; }
		public List<string>          Keywords        { get;              } = new List<string>();
		public TimeSpan              Duration        { get; private set; }
		public SystemRecognizedMedia RecognizedMedia { get; private set; }
		public VideoUploader         Uploader        { get; private set; }
		public List<MediaFormat>     MediaFormats    { get;              } = new List<MediaFormat>();

		public string VideoUrl {
			get {
				return $"https://www.youtube.com/watch?v={VideoID}";
			}
		}

		public string VideoUrlShortened {
			get {
				return $"https://youtu.be/{VideoID}";
			}
		}

		private YoutubeMediaInfo() {}

		internal static YoutubeMediaInfo FromDynamicJson(dynamic Source) {
			var Result = new YoutubeMediaInfo {
				VideoID     = Source.id,
				Description = Source.description,
				Title       = Source.fulltitle,
				License     = Source.license,
				Duration    = TimeSpan.FromSeconds(Convert.ToDouble(Source.duration))
			};

			Result.Rating = new VideoRating(
				Likes:    Convert.ToInt32(Source.like_count),
				Dislikes: Convert.ToInt32(Source.dislike_count)
			);
			
			Result.Uploader = new VideoUploader(
				ProfileID:   Source.uploader_id,
				ProfileName: Source.uploader,
				ProfileUri:  Source.uploader_url
			);

			if (Source.alt_title != null && Source.creator != null) {
				Result.RecognizedMedia = new SystemRecognizedMedia {
					Title  = Source.alt_title,
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

			foreach (dynamic tag in Source.tags)
				Result.Keywords.Add(tag.ToString());

			foreach (dynamic format in Source.formats) {
				MediaFormat f = MediaFormat.DetectMediaFormat(format);
				if (f != null)
					Result.MediaFormats.Add(f);
			}

			return Result;
		}
	}
}
