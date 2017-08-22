using System;

namespace TomiSoft.MP3Player.Communication.ClientModules {
	/// <summary>
	/// Provides commands for controlling the software itself.
	/// </summary>
	public class SoftwareModule {
		private readonly string ServerModule = "Software";
		private readonly ServerConnection Connection;

		/// <summary>
		/// Gets whether the server is ready to accept commands.
		/// </summary>
		public bool ServerReady {
			get {
				var Response = this.Connection.Send<bool>(
					new ServerRequest(ServerModule, "IsRunning")
				);

				return Response.RequestSucceeded && Response.Result;
			}
		}

		/// <summary>
		/// Gets the name of the server.
		/// </summary>
		public string Name {
			get {
				var Response = this.Connection.Send<string>(
					new ServerRequest(ServerModule, "Name")
				);

				Response.Check();

				return Response.Result;
			}
		}

		/// <summary>
		/// Gets the version of the server.
		/// </summary>
		public Version Version {
			get {
				var Response = this.Connection.Send<Version>(
					new ServerRequest(ServerModule, "Version")
				);

				Response.Check();

				return Response.Result;
			}
		}

		/// <summary>
		/// Gets the website of the software.
		/// </summary>
		public Uri Website {
			get {
				var Response = this.Connection.Send<Uri>(
					new ServerRequest(ServerModule, "Website")
				);

				Response.Check();

				return Response.Result;
			}
		}

		/// <summary>
		/// Gets the version of the server's API.
		/// </summary>
		public Version ApiVersion {
			get {
				var Response = this.Connection.Send<Version>(
					new ServerRequest(ServerModule, "ApiVersion")
				);

				Response.Check();

				return Response.Result;
			}
		}

		/// <summary>
		/// Gets the trace log from the software.
		/// </summary>
		public string TraceLog {
			get {
				var Response = this.Connection.Send<string>(
					new ServerRequest(ServerModule, "TraceLog")
				);

				Response.Check();

				return Response.Result;
			}
		}

		internal SoftwareModule(ServerConnection Connection) {
			this.Connection = Connection;
		}
	}
}
