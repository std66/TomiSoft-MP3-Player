namespace TomiSoft.MP3Player.Communication {
	public interface IServerResponse<T> {
		bool RequestSucceeded { get; }
		T Result { get; }
	}
}
