namespace TomiSoft.MP3Player.Communication.ServerModules {
	/// <summary>
	/// Represents a module of the PlayerServer.
	/// </summary>
	public interface IServerModule {
		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets if this module can handle the specified command.
		/// </summary>
		/// <param name="Command">The name of the command to be checked.</param>
		/// <returns>True if the module can handle the command, false if not.</returns>
		bool SupportsCommand(string Command);

		/// <summary>
		/// Executes the specified command with the given arguments.
		/// </summary>
		/// <param name="Command">The command to execute.</param>
		/// <param name="Parameters">The parameters of the command.</param>
		/// <returns>True if the command was handled successfully, false if not.</returns>
		bool ExecuteCommand(string Command, params string[] Parameters);
	}
}
