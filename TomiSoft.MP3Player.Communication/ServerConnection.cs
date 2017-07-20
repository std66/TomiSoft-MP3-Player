using System;
using System.IO;
using System.Net.Sockets;

namespace TomiSoft.MP3Player.Communication {
    internal class ServerConnection : IDisposable {
        private readonly TcpClient Client;
        private readonly StreamWriter ServerWriter;
        private readonly StreamReader ServerReader;

        private bool keepAlive = false;

        /// <summary>
        /// Gets if the connection is kept alive. If false,
        /// you can change it by invoking <see cref="SendKeepAlive"/>.
        /// False by default.
        /// </summary>
        public bool KeepAlive {
            get {
                return keepAlive;
            }
        }

        /// <summary>
        /// Gets if the connection is open.
        /// </summary>
        public bool ConnectionIsOpen {
            get {
                return this.Client.Connected;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConnection"/> class.
        /// Connects to localhost on the given port number.
        /// </summary>
        /// <param name="Port">The port number.</param>
        /// <exception cref="SocketException">when some connection problems occur</exception>
        internal ServerConnection(int Port) {
            this.Client = new TcpClient("localhost", Port);

            this.ServerWriter = new StreamWriter(this.Client.GetStream()) {
                AutoFlush = true
            };

            this.ServerReader = new StreamReader(this.Client.GetStream());
        }

        /// <summary>
        /// Sends a <see cref="string"/> data to the server in a safe way.
        /// </summary>
        /// <param name="Data">The data to send</param>
        public void Send(string Data) {
            if (this.ConnectionIsOpen && this.ServerWriter.BaseStream.CanWrite)
                this.ServerWriter.WriteLine(Data);
        }

        /// <summary>
        /// Reads a <see cref="string"/> data from the server in a safe way.
        /// </summary>
        /// <returns>The data read from the server</returns>
        public string Read() {
            if (this.ConnectionIsOpen && this.ServerReader.BaseStream.CanRead)
                return ServerReader.ReadLine();

            return String.Empty;
        }

        /// <summary>
        /// Sends a command to the server to keep alive the connection.
        /// </summary>
        public void SendKeepAlive() {
            this.Send("Connection.KeepAlive");
            this.keepAlive = true;
        }

        /// <summary>
        /// Closes the connection to the server.
        /// </summary>
        public void Dispose() {
            if (this.keepAlive)
                this.Send("Connection.Disconnect");

            this.ServerReader.Dispose();
            this.ServerWriter.Dispose();
            this.Client.Close();
        }
    }
}
