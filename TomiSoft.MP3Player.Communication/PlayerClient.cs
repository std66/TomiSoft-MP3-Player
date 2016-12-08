using System;
using System.IO;
using System.Net.Sockets;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// This class represents a client connection to the server. Use
	/// this class to send specific commands to the already running
	/// instance.
	/// </summary>
	public class PlayerClient : IDisposable {
		private TcpClient Client;
		private StreamWriter sw;
		private StreamReader sr;

        private bool keepAlive = false;

		/// <summary>
		/// Gets whether the server is ready to accept commands.
		/// </summary>
		public bool ServerReady {
			get {
				this.Send("IsRunning");
				return (this.sr.ReadLine().Contains("true"));
			}
		}

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        public string Name {
            get {
                this.Send("Name");
                return this.Read();
            }
        }

        /// <summary>
        /// Gets the version of the server.
        /// </summary>
        public Version Version {
            get {
                this.Send("Version");
                return new Version(this.Read());
            }
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
					Result = c.ServerReady;
				}
			}
			catch (SocketException) {
				Result = false;
			}

			return Result;
		}

		/// <summary>
		/// Initializes a new instance of the PlayerClient class. Connets to the already
		/// running instance.
		/// </summary>
        /// <param name="Port">The port number that the server is listening on</param>
        /// <exception cref="SocketException">when some connection problems occur</exception>
		public PlayerClient(int Port) {
			this.Client = new TcpClient("localhost", Port);
			this.sw = new StreamWriter(this.Client.GetStream()) {
				AutoFlush = true
			};
			this.sr = new StreamReader(this.Client.GetStream());
		}

		/// <summary>
		/// Sends the command to the server to play the given file.
		/// </summary>
		/// <param name="Filename">The file's full path to play</param>
		public void Play(string Filename) {
			this.Send($"Play;{Filename}");
        }

        /// <summary>
        /// Sends the command to the server to play the specified files.
        /// </summary>
        /// <param name="Filenames">The array of the files</param>
        public void Play(string[] Filenames) {
            this.Send($"Play;{String.Join(";", Filenames)}");
        }

        /// <summary>
        /// Sends the command to the server to play the next song in
        /// the playlist.
        /// </summary>
        public void PlayNext() {
            this.Send("PlayNext");
        }

        /// <summary>
        /// Sends the command to the server to play the previous song
        /// in the playlist.
        /// </summary>
        public void PlayPrevious() {
            this.Send("PlayPrevious");
        }

        /// <summary>
        /// Sends a command to the server to keep alive the connection.
        /// </summary>
        public void KeepAlive() {
            this.Send("KeepAlive");
            this.keepAlive = true;
        }
        
		/// <summary>
		/// Closes the connection to the server.
		/// </summary>
		public void Dispose() {
            if (this.keepAlive)
                this.Send("Disconnect");

            this.Client.Close();
		}

        /// <summary>
        /// Gets the playback position and the length of the song in
        /// seconds.
        /// </summary>
        /// <param name="Length">The variable to store the length of the song in seconds</param>
        /// <returns>The playback position in seconds</returns>
        public double PlaybackPosition(out double Length) {
            this.Send("PlaybackPosition");
            string[] Result = this.Read().Split(' ');

            if (Result.Length == 2) {
                Length = Convert.ToDouble(Result[1]);
                return Convert.ToDouble(Result[0]);
            }
            else {
                Length = 0;
                return 0;
            }
        }

        /// <summary>
        /// Sends data to the server in a safe way.
        /// </summary>
        /// <param name="Data">The data to send</param>
        private void Send(string Data) {
            if (this.Client.Connected && this.sw.BaseStream.CanWrite)
                this.sw.WriteLine(Data);
        }

        /// <summary>
        /// Reads data from the server in a safe way.
        /// </summary>
        /// <returns>The data read from the server</returns>
        private string Read() {
            if (this.Client.Connected && this.sr.BaseStream.CanRead)
                return sr.ReadLine();

            return String.Empty;
        }
	}
}
