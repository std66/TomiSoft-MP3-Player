using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Communication {
	internal class ServerConnection : IDisposable {
		private readonly TcpClient Client;
		private readonly StreamWriter ServerWriter;
		private readonly StreamReader ServerReader;

		/// <summary>
		/// Gets if the connection is kept alive. If false,
		/// you can change it by invoking <see cref="SendKeepAlive"/>.
		/// False by default.
		/// </summary>
		public bool KeepAlive {
			get;
			private set;
		} = false;

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
            this.Client = new TcpClient();
            if (!this.Client.ConnectAsync(IPAddress.Loopback, Port).Wait(100)) {
                throw new SocketException();
            }

			this.ServerWriter = new StreamWriter(this.Client.GetStream()) {
				AutoFlush = true
			};

			this.ServerReader = new StreamReader(this.Client.GetStream());
		}

		/// <summary>
		/// Sends a <see cref="string"/> data to the server in a safe way.
		/// </summary>
		/// <param name="Data">The data to send</param>
		internal void Send(string Data) {
			if (this.ConnectionIsOpen && this.ServerWriter.BaseStream.CanWrite)
				this.ServerWriter.WriteLine(Data);
		}

		/// <summary>
		/// Sends a <see cref="string"/> data to the server in a safe way asynchronously.
		/// </summary>
		/// <param name="Data">The data to send<</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		internal async Task SendAsync(string Data) {
			if (this.ConnectionIsOpen && this.ServerWriter.BaseStream.CanWrite)
				await this.ServerWriter.WriteLineAsync(Data);
		}

		/// <summary>
		/// Sends a request without reading a response.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		private void SendWithoutResponse(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			this.Send(
				JsonConvert.SerializeObject(Request)
			);
		}

		/// <summary>
		/// Sends a request without reading a response asynchronously.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		private async Task SendWithoutResponseAsync(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			await this.SendAsync(
				JsonConvert.SerializeObject(Request)
			);
		}

		/// <summary>
		/// Sends a request to the server.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		internal ServerResponse Send(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			this.Send(
				JsonConvert.SerializeObject(Request)
			);

			var Result = JsonConvert.DeserializeObject<ServerResponse>(
				this.Read()
			);

			return Result;
		}

		/// <summary>
		/// Sends a request to the server asynchronously.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		/// <exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		internal async Task<ServerResponse> SendAsync(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			await this.SendAsync(
				JsonConvert.SerializeObject(Request)
			);

			var Result = JsonConvert.DeserializeObject<ServerResponse>(
				await this.ReadAsync()
			);

			return Result;
		}

		/// <summary>
		/// Sends a request to the server.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <returns>
		///		A <see cref="ServerResponse{T}"/> instance that represents the
		///		data sent by the server.
		///	</returns>
		///	<exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		internal ServerResponse<T> Send<T>(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			this.Send(
				JsonConvert.SerializeObject(Request)
			);

			var Result = JsonConvert.DeserializeObject<ServerResponse<T>>(
				this.Read()
			);

			return Result;
		}

		/// <summary>
		/// Sends a request to the server asynchronously.
		/// </summary>
		/// <param name="Request">The request to be sent</param>
		/// <returns>
		///		A <see cref="Task"/> that represents the asynchronous process.
		///		A <see cref="ServerResponse{T}"/> instance is returned by the <see cref="Task"/> that represents the
		///		data sent by the server.
		///	</returns>
		///	<exception cref="ArgumentNullException">when <paramref name="Request"/> is null</exception>
		internal async Task<ServerResponse<T>> SendAsync<T>(ServerRequest Request) {
			#region Error checking
			if (Request == null)
				throw new ArgumentNullException(nameof(Request));
			#endregion

			await this.SendAsync(
				JsonConvert.SerializeObject(Request)
			);

			var Result = JsonConvert.DeserializeObject<ServerResponse<T>>(
				await this.ReadAsync()
			);

			return Result;
		}

		/// <summary>
		/// Reads a <see cref="string"/> data from the server in a safe way.
		/// </summary>
		/// <returns>
		///		The data read from the server. Empty <see cref="string"/> is returned when
		///		the read operation fails.
		///	</returns>
		internal string Read() {
			if (this.ConnectionIsOpen && this.ServerReader.BaseStream.CanRead)
				return ServerReader.ReadLine();

			return String.Empty;
		}

		/// <summary>
		/// Reads a <see cref="string"/> data from the server in a safe way.
		/// </summary>
		/// <returns>
		///		A <see cref="Task{string}"/> that represents the asynchronous process.
		///		The data read from the server is returned by the <see cref="Task{string}"/> or an
		///		empty <see cref="string"/> when the read operation fails.
		///	</returns>
		internal async Task<string> ReadAsync() {
			if (this.ConnectionIsOpen && this.ServerReader.BaseStream.CanRead)
				return await ServerReader.ReadLineAsync();

			return String.Empty;
		}

		/// <summary>
		/// Sends a command to the server to keep alive the connection.
		/// </summary>
		internal void SendKeepAlive() {
			this.SendWithoutResponse(
				new ServerRequest("Connection", "KeepAlive")
			);

			this.KeepAlive = true;
		}

		/// <summary>
		/// Sends a command to the server to keep alive the connection.
		/// </summary>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		internal async Task SendKeepAliveAsync() {
			await this.SendWithoutResponseAsync(
				new ServerRequest("Connection", "KeepAlive")
			);

			this.KeepAlive = true;
		}

		/// <summary>
		/// Closes the connection to the server.
		/// </summary>
		public void Dispose() {
			if (this.KeepAlive)
				this.SendWithoutResponse(new ServerRequest("Connection", "Disconnect"));

			this.ServerReader.Dispose();
			this.ServerWriter.Dispose();
			this.Client.Close();
		}
	}
}
