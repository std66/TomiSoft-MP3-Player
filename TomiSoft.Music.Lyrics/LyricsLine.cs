namespace TomiSoft.Music.Lyrics {
	/// <summary>
	/// Represents a line of a lyrics.
	/// </summary>
	public class LyricsLine : ILyricsLine {
		/// <summary>
		/// The end time of the line in seconds.
		/// </summary>
		public double EndTime {
			get;
			private set;
		}

		/// <summary>
		/// The start time of the line in seconds.
		/// </summary>
		public double StartTime {
			get;
			private set;
		}

		/// <summary>
		/// The line text of the lyrics.
		/// </summary>
		public string Text {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the LyricsLine class.
		/// </summary>
		/// <param name="StartTime">The start time of the line in seconds.</param>
		/// <param name="EndTime">The end time of the line in seconds.</param>
		/// <param name="Text">The line text of the lyrics.</param>
		public LyricsLine(double StartTime, double EndTime, string Text) {
			this.StartTime = StartTime;
			this.EndTime = EndTime;
			this.Text = Text;
		}
	}
}
