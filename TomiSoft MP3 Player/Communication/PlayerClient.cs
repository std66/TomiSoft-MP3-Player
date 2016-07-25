using System;
using System.IO;
using System.Net.Sockets;

namespace TomiSoft_MP3_Player {
	class PlayerClient : IDisposable {
		private TcpClient Client;
		private StreamWriter sw;

		public PlayerClient() {
			this.Client = new TcpClient("localhost", 22613);
			this.sw = new StreamWriter(this.Client.GetStream()) {
				AutoFlush = true
			};
		}

		public void Play(string Filename) {
			this.sw.WriteLine(String.Format("Play;{0}", Filename));
		}

		public void Dispose() {
			this.Client.Close();
		}
	}
}
