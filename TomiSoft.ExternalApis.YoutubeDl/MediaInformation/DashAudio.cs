using System;
using System.Collections.Generic;

namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class DashAudio : MediaFormat {
		public int SamplingRate { get; private set; }
		public int AverageBitrate { get; private set; }

		private DashAudio(string Codec) : base(Codec) {}

		public override MediaType Format {
			get {
				return MediaType.Audio;
			}
		}

		internal static MediaFormat FromDynamic(dynamic source) {
			#region Fix source
			IDictionary<string, object> a = source as IDictionary<string, object>;

			if (!a.ContainsKey("asr"))
				source.asr = 0;

			if (!a.ContainsKey("abr"))
				source.abr = 0;
			#endregion

			return new DashAudio(source.acodec) {
				SamplingRate = Convert.ToInt32(source.asr),
				AverageBitrate = Convert.ToInt32(source.abr)
			};
		}

		public override string ToString() {
			return $"{this.Note} ({this.Codec}, Sampling rate: {this.SamplingRate}, Bitrate: {this.AverageBitrate})";
		}
	}
}