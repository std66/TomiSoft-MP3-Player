namespace TomiSoft.MP3Player.Communication {
	public class ServerRequest<T> : IServerRequest<T> {
		public T Arguments {
			get;
			set;
		}

		public string Command {
			get;
			private set;
		}

		public string ModuleName {
			get;
			private set;
		}

		public ServerRequest(string Module, string Command) {
			this.ModuleName = Module;
			this.Command = Command;
		}

		public ServerRequest(string Module, string Command, T Arguments) : this(Module, Command) {
			this.Arguments = Arguments;
		}
	}
}
