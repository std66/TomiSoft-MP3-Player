using System;
using System.Collections.Generic;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class DashVideo : MediaFormat {
		public int FramesPerSecond { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		private DashVideo(string Codec) : base(Codec) { }

		public override MediaType Format {
			get {
				return MediaType.Video;
			}
		}

		internal static MediaFormat FromDynamic(dynamic source) {
			#region Fix source
			IDictionary<string, object> a = source as IDictionary<string, object>;

			if (!a.ContainsKey("fps"))
				source.fps = 0;

			if (!a.ContainsKey("height"))
				source.height = 0;

			if (!a.ContainsKey("width"))
				source.width = 0;
			#endregion

			return new DashVideo(source.vcodec) {
				FramesPerSecond = Convert.ToInt32(source.fps),
				Height = Convert.ToInt32(source.height),
				Width = Convert.ToInt32(source.width)
			};
		}

		public override string ToString() {
			return $"{this.Note} ({this.Codec}, Resolution: {this.Width}*{this.Height} {this.FramesPerSecond} FPS)";
		}
	}
}