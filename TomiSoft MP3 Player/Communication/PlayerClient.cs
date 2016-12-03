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

		/// <summary>
		/// Gets whether the server is ready to accept commands.
		/// </summary>
		public bool ServerReady {
			get {
				this.sw.WriteLine("IsRunning;");
				return (this.sr.ReadLine().Contains("true"));
			}
		}

		/// <summary>
		/// Determines whether the server is running.
		/// </summary>
		/// <returns>True if the server is running, false if not.</returns>
		public static bool IsServerRunning() {
			bool Result = false;

			try {
				using (PlayerClient c = new PlayerClient()) {
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
		public PlayerClient() {
			this.Client = new TcpClient("localhost", 22613);
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
			this.sw.WriteLine(String.Format("Play;{0}", Filename));
		}

		/// <summary>
		/// Closes the connection to the server.
		/// </summary>
		public void Dispose() {
			this.Client.Close();
		}
	}
}
