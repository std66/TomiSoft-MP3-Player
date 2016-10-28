using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TomiSoft_MP3_Player
{
    /// <summary>
    /// Ez az osztály felelős a TCP kapcsolaton érkező utasítások kezeléséért.
    /// </summary>
	class PlayerServer : IDisposable {
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
			this.ServerSocket = new TcpListener(IPAddress.Loopback, 22613);
			this.ServerSocket.Start();

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
				this.ServerSocket.Stop();
			}
		}

        /// <summary>
        /// Adatot fogad a kapcsolódott klienstől, majd bontja a kapcsolatot.
        /// </summary>
        /// <param name="client">A kliens-kapcsolatot reprezentáló TcpClient példány.</param>
		private void ClientThread(object client) {
			System.Diagnostics.Debug.WriteLine("Bejövő kapcsolat");

			TcpClient Client = client as TcpClient;
            if (Client == null)
                return;

			using (StreamReader sr = new StreamReader(Client.GetStream())) {
				string Data = sr.ReadLine();
				System.Diagnostics.Debug.WriteLine("Adat:");
				System.Diagnostics.Debug.WriteLine(Data);

				string[] Command = Data.Split(';');

				CommandReceived?.Invoke(Client.GetStream(), Command[0], Command[1]);
			}

            Client.Close();
		}

        /// <summary>
        /// Lezárja a bejövő kapcsolatokat fogadó csatornát.
        /// </summary>
		public void Dispose() {
			this.ListenThread.Interrupt();
		}
	}
}
