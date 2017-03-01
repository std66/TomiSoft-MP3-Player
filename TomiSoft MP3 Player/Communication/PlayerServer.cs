using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
		/// Tárolja a csatlakoztatott modulokat.
		/// </summary>
		private volatile IList<IServerModule> Modules = new List<IServerModule>();

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
			Trace.TraceInformation($"[Server] Starting server at localhost:{App.Config.ServerPort}");
			this.ServerSocket = new TcpListener(IPAddress.Loopback, App.Config.ServerPort);
			this.ServerSocket.Start();
			Trace.TraceInformation("[Server] Server is running.");
			
			try {
				while (true) {
					if (!this.ServerSocket.Pending()) {
						Thread.Sleep(500);
						continue;
					}

					TcpClient ClientSocket = this.ServerSocket.AcceptTcpClient();

					ThreadPool.QueueUserWorkItem(this.ClientThread, ClientSocket);
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

			using (StreamReader ClientReader = new StreamReader(Client.GetStream())){
				try {
					StreamWriter ClientWriter = new StreamWriter(Client.GetStream()) { AutoFlush = true };

					do {
						Trace.TraceInformation("[Server] Waiting for incoming data...");
						string Data = ClientReader.ReadLine();
						Trace.TraceInformation($"[Server] Data received: {Data}");

						#region Error checking
						if (String.IsNullOrWhiteSpace(Data)) {
							ClientWriter.WriteLine("ERROR: Nothing received.");
							continue;
						}
						#endregion

						string[] CommandLine = Data.Split(';');
						string ModuleAndCommand = CommandLine[0];

						string[] Parameters = new string[0];
						if (CommandLine.Length > 1)
							Parameters = CommandLine.GetPartOfArray(1, CommandLine.Length - 1);

						bool HandledByInternalCommandHandler = !this.InternalCommandHandler(ClientWriter, ref KeepAliveConnection, ModuleAndCommand, Parameters);

						if (HandledByInternalCommandHandler) {
							string[] CommandParts = ModuleAndCommand.Split('.');

							#region Error checking
							if (CommandParts.Length != 2) {
								ClientWriter.WriteLine("ERROR: Syntax error.");
								continue;
							}
							#endregion

							string Module = CommandParts[0];
							string Command = CommandParts[1];

							if (!this.ExecuteHandlerMethod(ClientWriter, Module, Command, Parameters)) {
								ClientWriter.WriteLine("ERROR: Command not supported.");
							}
						}

					}

					while (KeepAliveConnection && Client.Connected);
				}
				catch (ObjectDisposedException) { }
				catch (IOException) { }
				catch (ThreadInterruptedException) { }
			}

			Client.Close();
			Trace.TraceInformation("[Server] The connection is closed to the client.");
		}

		/// <summary>
		/// Finds the module that can execute the specified command and executes it.
		/// Also sends the result of the invocation to the client.
		/// </summary>
		/// <param name="ClientStream">A <see cref="StreamWriter"/> instance where the response is written.</param>
		/// <param name="Module">The name of the module.</param>
		/// <param name="Command">The command to execute.</param>
		/// <param name="Arguments">An array of the command's arguments.</param>
		/// <returns>True if the command is executed successfully, false if not.</returns>
		private bool ExecuteHandlerMethod(StreamWriter ClientStream, string Module, string Command, string[] Arguments) {
			#region Error checking
			if (ClientStream == null)
				return false;

			Arguments = Arguments ?? new string[0];
			#endregion

			IServerModule HandlerModule = this.Modules.FirstOrDefault(x => x.ModuleName == Module);
			MethodInfo Method = HandlerModule?.GetType().GetMethod(Command);

			#region Error checking
			if (HandlerModule == null || Method == null)
				return false;

			if (!Method.GetCustomAttributes(typeof(ServerCommandAttribute)).Any())
				return false;
			#endregion
			
			string Result = this.InvokeHandlerMethod(HandlerModule, Method, Arguments);

			if (!String.IsNullOrEmpty(Result)) {
				if (!Result.EndsWith(Environment.NewLine))
					ClientStream.WriteLine(Result);
				else
					ClientStream.Write(Result);
			}

			return true;
		}

		/// <summary>
		/// Invokes the specified method on the given <see cref="IServerModule"/> instance with the
		/// given arguments.
		/// </summary>
		/// <param name="HandlerModule">The instance on which the method will be invoked.</param>
		/// <param name="Method">The <see cref="MethodInfo"/> instance that holds information about the method that will be invoked.</param>
		/// <param name="Arguments">An array of arguments to be passed to the method.</param>
		/// <returns>The invoked method's return value represented as <see cref="string"/></returns>
		private string InvokeHandlerMethod(IServerModule HandlerModule, MethodInfo Method, object[] Arguments) {
			#region Error checking
			if (HandlerModule == null || Method == null || Arguments == null)
				return String.Empty;
			#endregion

			ParameterInfo[] ParamInfo = Method.GetParameters();

			//If the method has no parameters:
			if (ParamInfo.Length == 0) {
				return Method.Invoke(HandlerModule, null)?.ToString() ?? String.Empty;
			}

			//If the method has parameters:
			else {
				bool HasParams = ParamInfo.Last().GetCustomAttribute(typeof(ParamArrayAttribute)) != null;

				//If the method has "params"-type parameters:
				if (HasParams) {
					object[] RealParameters = new object[ParamInfo.Length];
					int ParamsIndex = ParamInfo.Length - 1;

					//Map the regular parameters
					for (int i = 0; i < ParamsIndex; i++)
						RealParameters[i] = Arguments[i];

					//Map the "params" parameters
					Type ParamsType = ParamInfo.Last().ParameterType.GetElementType();
					Array ParamsValues = Array.CreateInstance(ParamsType, Arguments.Length - ParamsIndex);
					for (int i = 0; i < ParamsValues.Length; i++)
						ParamsValues.SetValue(Arguments[i + ParamsIndex], i);

					RealParameters[ParamsIndex] = ParamsValues;

					Arguments = RealParameters;
				}
			}

			return Method.Invoke(HandlerModule, Arguments)?.ToString() ?? String.Empty;
		}

		/// <summary>
		/// Hozzáad egy modult a szerverhez.
		/// </summary>
		/// <param name="Module">A szerverhez csatolandó modul.</param>
		public void AttachModule(IServerModule Module) {
			#region Error checking
			if (Module == null)
				return;

			if (this.Modules.Any(x => x.ModuleName == Module.ModuleName))
				return;
			#endregion

			this.Modules.Add(Module);
		}

		/// <summary>
		/// Hozzáad egy vagy több modult a szerverhez.
		/// </summary>
		/// <param name="Modules">A szerverhez csatolandó modulok.</param>
		public void AttachModule(params IServerModule[] Modules) {
			#region Error checking
			if (Modules == null)
				return;
			#endregion

			foreach (var Module in Modules)
				this.AttachModule(Module);
		}

		/// <summary>
		/// Belső parancskezelő metódus.
		/// </summary>
		/// <param name="Client">A kliens-kapcsolatot reprezentáló TcpClient példány</param>
		/// <param name="KeepAlive">Maradjon-e nyitva a kapcsolat az első parancs fogadása után?</param>
		/// <param name="ModuleAndCommand">A végrehajtandó parancs</param>
		/// <param name="Arguments">A parancs paraméterei</param>
		/// <returns>True, ha belső parancskezelő végre tudta hajtani a parancsot, false ha nem</returns>
		private bool InternalCommandHandler(StreamWriter wrt, ref bool KeepAlive, string ModuleAndCommand, params string[] Arguments) {
			bool CommandHandled = false;

			switch (ModuleAndCommand) {
				case "Connection.KeepAlive":
					CommandHandled = true;
					KeepAlive = true;
					break;

				case "Connection.Disconnect":
					CommandHandled = true;
					wrt.Dispose();
					break;

				case "Server.GetModules":
					CommandHandled = true;
					wrt.WriteLine(String.Join(";", this.Modules.Select(x => x.ModuleName)));
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
