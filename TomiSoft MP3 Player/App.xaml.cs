using System.Windows;
using System.Diagnostics;
using System;
using System.Reflection;
using TomiSoft.MP3Player.Communication;
using TomiSoft.MP3Player.Utils;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
        /// <summary>
        /// Initializes a new instance of the TomiSoft MP3 Player application.
        /// </summary>
		public App() {
			Trace.Listeners.Add(new TextWriterTraceListener($"{AppDomain.CurrentDomain.BaseDirectory}\\application.log"));
			Trace.AutoFlush = true;

			Trace.WriteLine("");
			Trace.WriteLine($"New instance started at {DateTime.Now} (Is64BitProcess={Environment.Is64BitProcess}, Version={Version})");

            CheckIfAlreadyRunning();
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
                        Client.Play(args);
                }
            }
            catch (Exception e) {
                PlayerUtils.ErrorMessageBox(App.Name, e.Message);
            }

            Environment.Exit(0);
        }

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
	}
}
