using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Un4seen.Bass;

namespace TomiSoft_MP3_Player {
	/// <summary>
	/// This class manages the BASS library.
	/// </summary>
	static class BassManager {
		private static List<string> SupportedExtensions = new List<string>();

		/// <summary>
		/// Loads BASS and all of its plugins. Then initializes BASS on the default audio
		/// output device. BASS DLLs must be located in the directory
		/// \Bass\x64 and \Bass\x86.
		/// </summary>
		public static void Load() {
			string Directory = String.Format(
				@"{0}\Bass\{1}\",
				Environment.CurrentDirectory,
				Environment.Is64BitOperatingSystem ? "x64" : "x86"
			);

			LoadBass(Directory);
			LoadBassPlugins(Directory);

			if (!Bass.BASS_Init(-1, 44800, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)) {
				throw new Exception("Nem sikerült elindítani a BASS-t.");
			}
		}

		/// <summary>
		/// Gets all supported file extensions that is supported by BASS.
		/// </summary>
		/// <returns>A collection that contains the supported file extensions (without the prepending dot).</returns>
		public static IEnumerable<string> GetSupportedExtensions() {
			return SupportedExtensions;
		}

		/// <summary>
		/// Loads BASS library from the given directory.
		/// </summary>
		/// <param name="Directory">The directory that contains bass.dll and its plugins.</param>
		private static void LoadBass(string Directory) {
			string Filename = Directory + "bass.dll";
			if (!File.Exists(Filename)) {
				throw new Exception("A BASS nem tölthető be: " + Filename);
			}

			if (!Bass.LoadMe()) {
				throw new Exception("Nem sikerült betölteni a BASS-t.");
			}

			SupportedExtensions.AddRange(
				Bass.SupportedStreamExtensions.GetMatches(@"\W+([\w\d]+)").Select(x => x.ToLower())
			);
		}

		/// <summary>
		/// Loads all BASS plugins from the given directory.
		/// </summary>
		/// <param name="Directory">The directory that contains the plugin DLLs.</param>
		private static void LoadBassPlugins(string Directory) {
			DirectoryInfo DirInfo = new DirectoryInfo(Directory);

			var Plugins = DirInfo.EnumerateFiles("bass*.dll").Where(x => !x.FullName.Contains("bass.dll"));

			foreach (var File in Plugins) {
				int Result = Bass.BASS_PluginLoad(File.FullName);

				if (Result == 0) {
					string PluginSupportedExtensions = Utils.BASSAddOnGetSupportedFileExtensions(File.FullName);
					SupportedExtensions.AddRange(
						PluginSupportedExtensions.GetMatches(@"\W+([\w\d]+)").Select(x => x.ToLower())
					);
				}
			}
		}
	}
}
