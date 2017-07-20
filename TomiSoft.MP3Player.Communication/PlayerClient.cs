using System;
using System.Net.Sockets;
using TomiSoft.MP3Player.Communication.ClientModules;

namespace TomiSoft.MP3Player.Communication {
    /// <summary>
    /// This class represents a client connection to the server. Use
    /// this class to send specific commands to the already running
    /// instance.
    /// </summary>
    public class PlayerClient : IDisposable {
        /// <summary>
        /// Stores the connection to the server.
        /// </summary>
        private readonly ServerConnection Connection;

        private readonly SoftwareModule softwareModule;
        private readonly PlaylistModule playlistModule;
        private readonly PlaybackModule playbackModule;
        private readonly LyricsModule lyricsModule;

        /// <summary>
        /// Gets the server's default port.
        /// </summary>
        public static int DefaultPort {
            get {
                return 22613;
            }
        }

        /// <summary>
        /// Gets if the connection is open.
        /// </summary>
        public bool IsConnected {
            get {
                return this.Connection.ConnectionIsOpen;
            }
        }

        /// <summary>
        /// Gets if the connection is kept alive. If false,
        /// you can invoke <see cref="KeepAlive"/>.
        /// </summary>
        public bool KeepAliveConnection {
            get {
                return this.Connection.KeepAlive;
            }
        }

        /// <summary>
        /// Provides commands for controlling the software itself.
        /// </summary>
        public SoftwareModule Software {
            get {
                return softwareModule;
            }
        }

        /// <summary>
        /// Provides commands for controlling the playlist.
        /// </summary>
        public PlaylistModule Playlist {
            get {
                return playlistModule;
            }
        }

        /// <summary>
        /// Provides commands for controlling the playback.
        /// </summary>
        public PlaybackModule Playback {
            get {
                return playbackModule;
            }
        }
        
        /// <summary>
        /// Provides commands for controlling the lyrics.
        /// </summary>
        public LyricsModule Lyrics {
            get {
                return lyricsModule;
            }
        }

        /// <summary>
        /// Determines whether the server is running on the default port.
        /// </summary>
        /// <returns>True if the server is running, false if not.</returns>
        public static bool IsServerRunning() {
            return IsServerRunning(DefaultPort);
        }

        /// <summary>
        /// Determines whether the server is running.
        /// </summary>
        /// <param name="Port">The port number that the server is listening on</param>
        /// <returns>True if the server is running, false if not.</returns>
        public static bool IsServerRunning(int Port) {
            bool Result = false;

            try {
                using (PlayerClient c = new PlayerClient(Port)) {
                    Result = c.softwareModule.ServerReady;
                }
            }
            catch (SocketException) {
                Result = false;
            }

            return Result;
        }

        /// <summary>
        /// Initializes a new instance of the PlayerClient class. Connets to the already
        /// running instance on the default port.
        /// </summary>
        /// <param name="Port">The port number that the server is listening on</param>
        /// <exception cref="SocketException">when some connection problems occur</exception>
        public PlayerClient() : this(DefaultPort) {}

        /// <summary>
        /// Initializes a new instance of the PlayerClient class. Connets to the already
        /// running instance.
        /// </summary>
        /// <param name="Port">The port number that the server is listening on</param>
        /// <exception cref="SocketException">when some connection problems occur</exception>
        public PlayerClient(int Port) {
            this.Connection = new ServerConnection(Port);

            this.softwareModule = new SoftwareModule(this.Connection);
            this.playlistModule = new PlaylistModule(this.Connection);
            this.playbackModule = new PlaybackModule(this.Connection);
            this.lyricsModule = new LyricsModule(this.Connection);
        }
        
        /// <summary>
        /// Sends a command to the server to keep alive the connection.
        /// </summary>
        public void KeepAlive() {
            this.Connection.SendKeepAlive();
        }

        /// <summary>
        /// Closes the connection to the server.
        /// </summary>
        public void Dispose() {
            this.Connection.Dispose();
        }
    }
}
