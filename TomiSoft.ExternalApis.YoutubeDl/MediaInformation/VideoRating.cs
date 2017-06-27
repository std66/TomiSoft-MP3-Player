namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class VideoRating {
		public int Likes { get; }
		public int Dislikes { get; }

		public int Votes {
			get {
				return this.Likes + this.Dislikes;
			}
		}

		public double RatingInPercent {
			get {
				return (double)Likes / Votes * 100;
			}
		}

		internal VideoRating(int Likes, int Dislikes) {
			this.Likes = Likes;
			this.Dislikes = Dislikes;
		}
	}
}
