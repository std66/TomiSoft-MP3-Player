using System;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the software itself.
    /// </summary>
    public class SoftwareModule {
        private readonly ServerConnection Connection;

        /// <summary>
		/// Gets whether the server is ready to accept commands.
		/// </summary>
		public bool ServerReady {
            get {
                ServerResponse<bool> Response = this.Connection.Send<bool>(new ServerRequest("Software", "IsRunning"));
                return Response.RequestSucceeded && Response.Result;
            }
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string Name {
            get {
                ServerResponse<string> Response = this.Connection.Send<string>(new ServerRequest("Software", "Name"));

                if (!Response.RequestSucceeded)
                    throw new Exception($"Request failed. Server message: {Response.Message}");

                return Response.Result;
            }
        }

        /// <summary>
        /// Gets the version of the server.
        /// </summary>
        public Version Version {
            get {
                ServerResponse<Version> Response = this.Connection.Send<Version>(new ServerRequest("Software", "Version"));

                if (!Response.RequestSucceeded)
                    throw new Exception($"Request failed. Server message: {Response.Message}");

                return Response.Result;
            }
        }

        internal SoftwareModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
