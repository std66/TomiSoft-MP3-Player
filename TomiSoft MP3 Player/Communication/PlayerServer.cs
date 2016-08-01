﻿using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TomiSoft_MP3_Player {
	class PlayerServer : IDisposable {
		private TcpListener ServerSocket;
		private Thread ListenThread;

		public event Action<string, string> CommandReceived;

		public PlayerServer() {
			this.ListenThread = new Thread(Listen);
			this.ListenThread.Start();
		}

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

		private void ClientThread(object client) {
			System.Diagnostics.Debug.WriteLine("Bejövő kapcsolat");

			TcpClient Client = client as TcpClient;
			using (StreamReader sr = new StreamReader(Client.GetStream())) {
				string Data = sr.ReadLine();
				System.Diagnostics.Debug.WriteLine("Adat:");
				System.Diagnostics.Debug.WriteLine(Data);

				string[] Command = Data.Split(';');

				CommandReceived?.Invoke(Command[0], Command[1]);
			}
		}

		public void Dispose() {
			this.ListenThread.Interrupt();
		}
	}
}
