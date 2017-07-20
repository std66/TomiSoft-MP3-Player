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
                this.Connection.Send("Software.IsRunning");
                return (this.Connection.Read().Contains("true"));
            }
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string Name {
            get {
                this.Connection.Send("Software.Name");
                return this.Connection.Read();
            }
        }

        /// <summary>
        /// Gets the version of the server.
        /// </summary>
        public Version Version {
            get {
                this.Connection.Send("Software.Version");
                return new Version(this.Connection.Read());
            }
        }

        internal SoftwareModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
