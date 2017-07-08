namespace TomiSoft.MP3Player.Communication {
	public class ServerResponse<T> : IServerResponse<T> {
		public bool RequestSucceeded {
			get;
			private set;
		}

		public T Result {
			get;
			private set;
		}

		public ServerResponse(bool IsSuccessful, T Result) {
			this.RequestSucceeded = IsSuccessful;
			this.Result = Result;
		}
	}
}
