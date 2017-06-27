namespace TomiSoft.ExternalApis.YoutubeDl.MediaInformation {
	public class VideoUploader {
		public string Name { get; private set; }
		public string ID { get; private set; }
		public string ProfileUri { get; private set; }

		internal VideoUploader(string ProfileName, string ProfileID, string ProfileUri) {
			this.ID = ProfileID;
			this.Name = ProfileName;
			this.ProfileUri = ProfileUri;
		}
	}
}
