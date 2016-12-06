using System.Windows;
using System.Diagnostics;
using System;
using System.Reflection;

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
	}
}
