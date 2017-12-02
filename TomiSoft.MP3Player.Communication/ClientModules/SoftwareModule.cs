using System;
using System.Threading.Tasks;

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

		/// <summary>
		/// Gets the media file extensions that are supported by the software.
		/// </summary>
		public string[] SupportedFileExtensions {
			get {
				var Response = this.Connection.Send<string[]>(
					new ServerRequest(ServerModule, "GetSupportedFileExtensions")
				);

				Response.Check();

				return Response.Result;
			}
		}

		internal SoftwareModule(ServerConnection Connection) {
			this.Connection = Connection;
		}

		/// <summary>
		/// Determines whether a given media is supported by the software.
		/// </summary>
		/// <param name="Source">The location of the media. Can be a URI, path to a file, ...</param>
		/// <returns>True if the media is supported by the software, false otherwise.</returns>
		public bool IsSupportedMedia(string Source) {
			var Response = this.Connection.Send<bool>(
				new ServerRequest(ServerModule, "IsSupportedMedia", Source)
			);

			Response.Check();

			return Response.Result;
		}

		/// <summary>
		/// Determines whether a given media is supported by the software.
		/// </summary>
		/// <param name="Source">The location of the media. Can be a URI, path to a file, ...</param>
		/// <returns>
		///		A <see cref="Task{bool}"/> that represents the asynchronous process.
		///		True if the media is supported by the software, false otherwise.
		///	</returns>
		public async Task<bool> IsSupportedMediaAsync(string Source) {
			var Response = await this.Connection.SendAsync<bool>(
				new ServerRequest(ServerModule, "IsSupportedMedia", Source)
			);

			Response.Check();

			return Response.Result;
		}
	}
}
