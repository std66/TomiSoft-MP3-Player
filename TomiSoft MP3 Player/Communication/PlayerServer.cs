using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TomiSoft.MP3Player.Utils.Extensions;
using TomiSoft_MP3_Player;

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
        public event Action<Stream, string, string[]> CommandReceived;

        /// <summary>
        /// Létrehozza a PlayerServer osztály egy új példányát. Külön szálon várakozik
        /// bejövő kapcsolatokra.
        /// </summary>
        public PlayerServer() {
            this.ListenThread = new Thread(Listen) {
                Name = "Server listening thread"
            };
            this.ListenThread.Start();
        }

        /// <summary>
        /// Megnyitja a bejövő kapcsolatok fogadására alkalmas TCP csatornát. Amikor
        /// új kliens kapcsolódik, egy külön szálat indít el a kommunikációhoz.
        /// </summary>
        private void Listen() {
            Trace.TraceInformation("[Server] Starting server at localhost:22613");
            this.ServerSocket = new TcpListener(IPAddress.Loopback, App.Config.ServerPort);
            this.ServerSocket.Start();
            Trace.TraceInformation("[Server] Server is running.");

            int ClientThreadID = 0;

            try {
                while (true) {
                    if (!this.ServerSocket.Pending()) {
                        Thread.Sleep(500);
                        continue;
                    }

                    TcpClient ClientSocket = this.ServerSocket.AcceptTcpClient();

                    Thread ClientProc = new Thread(new ParameterizedThreadStart(ClientThread)) {
                        Name = $"Client thread #{ClientThreadID}"
                    };
                    ClientProc.Start(ClientSocket);

                    ClientThreadID++;
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

            #region Error checking
            if (Client == null)
                return;
            #endregion

            Trace.TraceInformation("[Server] A client has connected.");
            bool KeepAliveConnection = false;

            using (StreamReader sr = new StreamReader(Client.GetStream())) {
                try {
                    do {
                        Trace.TraceInformation("[Server] Waiting for incoming data...");
                        string Data = sr.ReadLine();
                        Trace.TraceInformation("[Server] Data received");

                        if (Data != null) {
                            string[] CommandLine = Data.Split(';');
                            string Command = CommandLine[0];

                            string[] Parameters = new string[0];
                            if (CommandLine.Length > 1)
                                Parameters = CommandLine.GetPartOfArray(1, CommandLine.Length - 1);

                            bool InvokeCommandReceived = !this.InternalCommandHandler(Client, ref KeepAliveConnection, Command, Parameters);

                            if (InvokeCommandReceived)
                                CommandReceived?.Invoke(Client.GetStream(), Command, Parameters);
                        }
                    }
                    while (KeepAliveConnection && Client.Connected);
                }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
            }

            Client.Close();
            Trace.TraceInformation("[Server] The connection is closed to the client.");
        }

        /// <summary>
        /// Belső parancskezelő metódus.
        /// </summary>
        /// <param name="Client">A kliens-kapcsolatot reprezentáló TcpClient példány</param>
        /// <param name="KeepAlive">Maradjon-e nyitva a kapcsolat az első parancs fogadása után?</param>
        /// <param name="Command">A végrehajtandó parancs</param>
        /// <param name="Arguments">A parancs paraméterei</param>
        /// <returns>True, ha belső parancskezelő végre tudta hajtani a parancsot, false ha nem</returns>
        private bool InternalCommandHandler(TcpClient Client, ref bool KeepAlive, string Command, params string[] Arguments) {
            bool CommandHandled = false;

            StreamWriter wrt = new StreamWriter(Client.GetStream()) {
                AutoFlush = true
            };

            switch (Command) {
                case "IsRunning":
                    CommandHandled = true;
                    wrt.WriteLine("true");
                    break;

                case "Version":
                    CommandHandled = true;
                    wrt.WriteLine(App.Version.ToString());
                    break;

                case "Name":
                    CommandHandled = true;
                    wrt.WriteLine(App.Name);
                    break;

                case "KeepAlive":
                    CommandHandled = true;
                    KeepAlive = true;
                    break;

                case "Disconnect":
                    CommandHandled = true;
                    wrt.Dispose();
                    break;
            }

            return CommandHandled;
        }

        /// <summary>
        /// Lezárja a bejövő kapcsolatokat fogadó csatornát.
        /// </summary>
        public void Dispose() {
            this.ListenThread.Interrupt();
        }
    }
}
