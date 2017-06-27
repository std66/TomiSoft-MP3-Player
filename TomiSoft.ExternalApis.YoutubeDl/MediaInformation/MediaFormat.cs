using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public enum MediaType {
		Audio, Video
	}

	public abstract class MediaFormat {
		public abstract MediaType Format { get; }
		public string Note { get; private set; }
		public int FileSize { get; private set; }
		public int FormatID { get; private set; }
		public string Extension { get; private set; }
		public string Codec { get; private set; }

		protected MediaFormat(string Codec) {
			this.Codec = Codec;
		}

		internal static MediaFormat DetectMediaFormat(dynamic Source) {
			MediaFormat Result = null;

			if (Source.format_note == "DASH audio")
				Result = DashAudio.FromDynamic(Source);
			else if (Source.format_note == "DASH video")
				Result = DashVideo.FromDynamic(Source);
			else
				return null;

			Result.Note = Source.format_note;
			Result.FormatID = Convert.ToInt32(Source.format_id);
			Result.FileSize = Convert.ToInt32(Source.filesize);
			Result.Extension = Source.ext;

			return Result;
		}
	}
}
