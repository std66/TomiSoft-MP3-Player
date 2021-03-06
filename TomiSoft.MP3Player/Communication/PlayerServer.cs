﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using TomiSoft_MP3_Player;

namespace TomiSoft.MP3Player.Communication {
	/// <summary>
	/// Ez az osztály felelős a TCP kapcsolaton érkező utasítások kezeléséért.
	/// </summary>
	public class PlayerServer : IDisposable {
		private TcpListener ServerSocket;
		private Thread ListenThread;

		private readonly JsonSerializerSettings JsonConfig = new JsonSerializerSettings {
			TypeNameHandling = TypeNameHandling.All
		};

		/// <summary>
		/// Tárolja a csatlakoztatott modulokat.
		/// </summary>
		private volatile IList<IServerModule> Modules = new List<IServerModule>();

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

			bool KeepAliveConnection = false;

			string Data = "";

			using (StreamReader ClientReader = new StreamReader(Client.GetStream())) {
				try {
					StreamWriter ClientWriter = new StreamWriter(Client.GetStream()) { AutoFlush = true };

					do {
						Data = ClientReader.ReadLine();

						#region Error checking
						if (String.IsNullOrWhiteSpace(Data)) {
							ClientWriter.WriteLine("ERROR: Nothing received.");
							continue;
						}
						#endregion

						ServerRequest Request = JsonConvert.DeserializeObject<ServerRequest>(Data);

						bool HandledByInternalCommandHandler = !this.InternalCommandHandler(ClientWriter, ref KeepAliveConnection, Request.Module, Request.Command);

						if (HandledByInternalCommandHandler) {
							if (!this.ExecuteHandlerMethod(ClientWriter, Request.Module, Request.Command, Request.Arguments?.ToArray())) {
								string Response = JsonConvert.SerializeObject(
									ServerResponse<object>.GetFailed($"Failed to execute: Module={Request.Module} Command={Request.Command}")
								);

								ClientWriter.WriteLine(Response);
							}
						}
					}

					while (KeepAliveConnection && Client.Connected);
				}
				catch (ObjectDisposedException) { }
				catch (IOException) { }
				catch (ThreadInterruptedException) { }
				catch (JsonReaderException) {
					Trace.WriteLine(Data);
				}
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

			object Result = this.InvokeHandlerMethod(HandlerModule, Method, Arguments);

			ServerResponse<object> Response = ServerResponse<object>.GetSuccess(Result);

			ClientStream.WriteLine(
				JsonConvert.SerializeObject(Response, JsonConfig)
			);

			return true;
		}

		/// <summary>
		/// Invokes the specified method on the given <see cref="IServerModule"/> instance with the
		/// given arguments.
		/// </summary>
		/// <param name="HandlerModule">The instance on which the method will be invoked.</param>
		/// <param name="Method">The <see cref="MethodInfo"/> instance that holds information about the method that will be invoked.</param>
		/// <param name="Arguments">An array of arguments to be passed to the method.</param>
		/// <returns>The invoked method's return value.</returns>
		private object InvokeHandlerMethod(IServerModule HandlerModule, MethodInfo Method, object[] Arguments) {
			#region Error checking
			if (HandlerModule == null || Method == null || Arguments == null)
				return String.Empty;
			#endregion

			ParameterInfo[] ParamInfo = Method.GetParameters();

			object Result = null;

			//If the method has no parameters:
			if (ParamInfo.Length == 0) {
				Result = Method.Invoke(HandlerModule, null);
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

				Result = Method.Invoke(HandlerModule, Arguments);
			}

			return Result;
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
		/// <param name="wrt">A kliens-kapcsolatot reprezentáló TcpClient példány</param>
		/// <param name="KeepAlive">Maradjon-e nyitva a kapcsolat az első parancs fogadása után?</param>
		/// <param name="Module">The name of the module that will execute the command.</param>
		/// <param name="Command">The name of the command that will be executed by the module.</param>
		/// <returns>True, ha belső parancskezelő végre tudta hajtani a parancsot, false ha nem</returns>
		private bool InternalCommandHandler(StreamWriter wrt, ref bool KeepAlive, string Module, string Command) {
			bool CommandHandled = false;

			switch ($"{Module}.{Command}") {
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

			if (CommandHandled) {
				ServerResponse<object>.GetSuccess(null);
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
