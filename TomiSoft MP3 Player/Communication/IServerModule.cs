namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// Represents a server module.
	/// </summary>
	public interface IServerModule {
		/// <summary>
		/// Gets the name of the server module.
		/// </summary>
		string ModuleName { get; }
	}
}
