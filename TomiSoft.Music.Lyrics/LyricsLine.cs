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
		/// Initializes a new instance of the <see cref="LyricsLine"/> class.
		/// </summary>
		/// <param name="StartTime">The start time of the line in seconds.</param>
		/// <param name="EndTime">The end time of the line in seconds.</param>
		/// <param name="Text">The line text of the lyrics.</param>
		public LyricsLine(double StartTime, double EndTime, string Text) {
			this.StartTime = StartTime;
			this.EndTime = EndTime;
			this.Text = Text;
		}

		/// <summary>
		/// Checks if this object is equal to another given object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> instance to compare with.</param>
		/// <returns>True if the two objects are equal, false if not.</returns>
		public override bool Equals(object obj) {
			LyricsLine o = obj as LyricsLine;

			if (o == null)
				return false;

			return
				o.StartTime == this.StartTime &&
				o.EndTime == this.EndTime &&
				o.Text == this.Text;
		}

		/// <summary>
		/// Calculates a hash code for the object.
		/// </summary>
		/// <returns>The calculated hash code.</returns>
		public override int GetHashCode() {
			return 7 * StartTime.GetHashCode() * EndTime.GetHashCode() * Text.GetHashCode();
		}
	}
}
