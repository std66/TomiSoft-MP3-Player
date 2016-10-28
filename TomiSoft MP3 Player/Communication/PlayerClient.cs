using System;
using System.IO;
using System.Net.Sockets;

namespace TomiSoft_MP3_Player {
	class PlayerClient : IDisposable {
		private TcpClient Client;
		private StreamWriter sw;
		private StreamReader sr;

		public bool ServerRunning {
			get {
				this.sw.WriteLine("IsRunning;");
				return (this.sr.ReadLine().Contains("true"));
			}
		}

		public static bool IsServerRunning() {
			bool Result = false;

			try {
				using (PlayerClient c = new PlayerClient()) {
					Result = c.ServerRunning;
				}
			}
			catch (SocketException) {
				Result = false;
			}

			return Result;
		}

		public PlayerClient() {
			this.Client = new TcpClient("localhost", 22613);
			this.sw = new StreamWriter(this.Client.GetStream()) {
				AutoFlush = true
			};
			this.sr = new StreamReader(this.Client.GetStream());
		}

		public void Play(string Filename) {
			this.sw.WriteLine(String.Format("Play;{0}", Filename));
		}

		public void Dispose() {
			this.Client.Close();
		}
	}
}
