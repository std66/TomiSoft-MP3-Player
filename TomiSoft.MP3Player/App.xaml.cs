using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using TomiSoft.MP3Player.Communication;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Extensions;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		private static readonly MemoryStream LogStore = new MemoryStream();

		/// <summary>
		/// Gets a new instance of the application configuration class.
		/// </summary>
		public static TomiSoft.MP3Player.Properties.Settings Config {
			get {
				return new TomiSoft.MP3Player.Properties.Settings();
			}
		}

		/// <summary>
		/// Gets the version of the application.
		/// </summary>
		public static Version Version {
			get {
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		public static string Name {
			get {
				return "TomiSoft MP3 Player";
			}
		}

		/// <summary>
		/// Gets the website of the application.
		/// </summary>
		public static Uri Website {
			get {
				return new Uri("https://github.com/std66/TomiSoft-MP3-Player");
			}
		}

		/// <summary>
		/// Gets the name of the application's author.
		/// </summary>
		public static string Author {
			get {
				return "Sinku Tamás";
			}
		}

		/// <summary>
		/// Gets the path of the application.
		/// </summary>
		public static string Path {
			get {
				string Result = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				if (!Result.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
					Result += System.IO.Path.DirectorySeparatorChar;

				return Result;
			}
		}

		/// <summary>
		/// Gets the entire trace log.
		/// </summary>
		public static string TraceLog {
			get {
				string Result = String.Empty;

				using (MemoryStream ms = new MemoryStream()) {
					LogStore.WriteTo(ms);
					ms.Position = 0;

					StreamReader sr = new StreamReader(ms);
					Result = sr.ReadToEnd();
				}

				return Result;
			}
		}

		/// <summary>
		/// Initializes a new instance of the TomiSoft MP3 Player application.
		/// </summary>
		public App() {
			this.Exit += (o, e) => LogStore.Dispose();
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			Trace.AutoFlush = true;
			Trace.Listeners.Add(new TextWriterTraceListener(LogStore));
			Trace.WriteLine($"New instance started at {DateTime.Now} (Is64BitProcess={Environment.Is64BitProcess}, Version={Version})");

			CheckIfAlreadyRunning();
		}

		/// <summary>
		/// This method is executed when an unhandled exception occures.
		/// Flushes the log to the disk.
		/// </summary>
		/// <param name="sender">The sender object's instance</param>
		/// <param name="e">Event parameters</param>
		private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) {
			try {
				Exception ex = e.ExceptionObject as Exception;

				#region Error checking
				if (ex == null)
					return;
				#endregion

				Trace.WriteLine("");
				Trace.WriteLine($"{ex.GetType().Name} occured:");
				Trace.WriteLine($"Reason: {ex.Message}");
				Trace.WriteLine("Stack trace:");
				Trace.WriteLine(ex.StackTrace);

				ex = ex.InnerException;
				while (ex != null) {
					Trace.WriteLine("");
					Trace.WriteLine($"Inner exception: {ex.GetType().Name}:");
					Trace.WriteLine($"Reason: {ex.Message}");
					Trace.WriteLine("Stack trace:");
					Trace.WriteLine(ex.StackTrace);

					ex = ex.InnerException;
				}

				string LogFileName = System.IO.Path.GetTempFileName();
				using (Stream s = File.OpenWrite(LogFileName)) {
					LogStore.WriteTo(s);
					s.Flush();
				}

				MessageBoxResult r = MessageBox.Show(
					caption: "TomiSoft MP3 Player",
					messageBoxText: "Egy hiba miatt az alkalmazásnak le kell állnia. Szeretnéd megnézni a naplót?",
					button: MessageBoxButton.YesNo,
					icon: MessageBoxImage.Question
				);

				if (r == MessageBoxResult.Yes) {
					Process.Start(new ProcessStartInfo(
						fileName: "notepad.exe",
						arguments: LogFileName
					));
				}
			}

			finally {
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// Checks if an instance of the application is already running. If yes,
		/// sends the command line arguments to it and then terminates this
		/// instance.
		/// </summary>
		private static void CheckIfAlreadyRunning() {
			//If an instance is already running, send the file list to it.
			//Otherwise, this is the only running instance, so we start a server to
			//listen to the other instances.
			Trace.TraceInformation("[Player startup] Checking if an instance is already running...");

			if (PlayerClient.IsServerRunning(App.Config.ServerPort)) {
				Trace.TraceInformation("[Player startup] Found a running instance");
				SendFileListToServer();
				Environment.Exit(0);
			}
			else {
				Trace.TraceInformation("[Player startup] No other instances are running.");
			}
		}

		/// <summary>
		/// Sends the file list to the server then closes the application.
		/// </summary>
		private static void SendFileListToServer() {
			try {
				using (PlayerClient Client = new PlayerClient(App.Config.ServerPort)) {
					string[] args = Environment.GetCommandLineArgs();

					if (args.Length > 1)
						Client.Play(args.GetPartOfArray(1, args.Length - 1));
				}
			}
			catch (Exception e) {
				PlayerUtils.ErrorMessageBox(App.Name, e.Message);
			}
		}
	}
}
