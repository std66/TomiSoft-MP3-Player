using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// Ez az osztály felelős a TCP kapcsolaton érkező utasítások kezeléséért.
	/// </summary>
	public class PlayerServer : IDisposable {
		private TcpListener ServerSocket;
		private Thread ListenThread;

		/// <summary>
		/// Ez az esemény akkor fut le, ha parancs érkezik valamely klienstől.
		/// </summary>
		public event Action<Stream, string, string> CommandReceived;

		/// <summary>
		/// Létrehozza a PlayerServer osztály egy új példányát. Külön szálon várakozik
		/// bejövő kapcsolatokra.
		/// </summary>
		public PlayerServer() {
			this.ListenThread = new Thread(Listen);
			this.ListenThread.Start();
		}

		/// <summary>
		/// Megnyitja a bejövő kapcsolatok fogadására alkalmas TCP csatornát. Amikor
		/// új kliens kapcsolódik, egy külön szálat indít el a kommunikációhoz.
		/// </summary>
		private void Listen() {
			Trace.TraceInformation("[Server] Starting server at localhost:22613");
			this.ServerSocket = new TcpListener(IPAddress.Loopback, 22613);
			this.ServerSocket.Start();
			Trace.TraceInformation("[Server] Server is running.");

			try {
				while (true) {
					if (!this.ServerSocket.Pending()) {
						Thread.Sleep(500);
						continue;
					}

					TcpClient ClientSocket = this.ServerSocket.AcceptTcpClient();

					Thread ClientProc = new Thread(new ParameterizedThreadStart(ClientThread));
					ClientProc.Start(ClientSocket);
				}
			}
			catch (ThreadInterruptedException) {
				Trace.TraceInformation("[Server] Stopping server...");
				this.ServerSocket.Stop();
			}

			Trace.TraceInformation("[Server] Server stopped");
		}

		/// <summary>
		/// Adatot fogad a kapcsolódott klienstől, majd bontja a kapcsolatot.
		/// </summary>
		/// <param name="client">A kliens-kapcsolatot reprezentáló TcpClient példány.</param>
		private void ClientThread(object client) {
			TcpClient Client = client as TcpClient;
			if (Client == null)
				return;

			Trace.TraceInformation("[Server] A client has connected.");

			using (StreamReader sr = new StreamReader(Client.GetStream())) {
				Trace.TraceInformation("[Server] Waiting for incoming data...");
				string Data = sr.ReadLine();
				Trace.TraceInformation("[Server] Data received");

				if (Data != null) {
					string[] Command = Data.Split(';');
					CommandReceived?.Invoke(Client.GetStream(), Command[0], Command[1]);
				}
			}

			Client.Close();
			Trace.TraceInformation("[Server] The connection is closed to the client.");
		}

		/// <summary>
		/// Lezárja a bejövő kapcsolatokat fogadó csatornát.
		/// </summary>
		public void Dispose() {
			this.ListenThread.Interrupt();
		}
	}
}
