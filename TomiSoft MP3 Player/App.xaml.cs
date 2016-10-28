using System.Windows;
using System.Diagnostics;
using System;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public App() {
			Trace.Listeners.Add(new TextWriterTraceListener("application.log"));
			Trace.AutoFlush = true;

			Trace.WriteLine("");
			Trace.WriteLine($"New instance started at {DateTime.Now}");
		}
	}
}
