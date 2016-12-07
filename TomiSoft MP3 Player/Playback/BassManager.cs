using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

using Un4seen.Bass;
using TomiSoft.MP3Player.Utils.Extensions;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// This class manages the BASS library.
	/// </summary>
	static class BassManager {
		private static List<string> SupportedExtensions = new List<string>();

        /// <summary>
        /// Gets if BASS is loaded
        /// </summary>
        public static bool BassLoaded {
            get;
            private set;
        } = false;

        /// <summary>
        /// Gets if BASS is initialized.
        /// </summary>
        public static bool BassInitialized {
            get;
            private set;
        } = false;

        /// <summary>
        /// Gets if all BASS plugins has been loaded successfully.
        /// </summary>
        public static bool AllPluginsLoaded {
            get;
            private set;
        } = false;

        /// <summary>
        /// Gets the version of the BASS library.
        /// </summary>
        /// <exception cref="InvalidOperationException">when BassLoaded is false</exception>
        public static Version BassVersion {
            get {
                #region Error checking
                if (!BassLoaded) {
                    Trace.TraceWarning("Cannot get BassManager.BassVersion because BASS is not loaded yet.");
                    throw new InvalidOperationException("BASS must be loaded first.");
                }
                #endregion

                return Bass.BASS_GetVersion(4);
            }
        }

		/// <summary>
		/// Loads BASS and all of its plugins. BASS DLLs must be located in the directory
		/// \Bass\x64 and \Bass\x86.
		/// </summary>
		/// <returns>True if BASS is successfully loaded, false if not.</returns>
		public static bool Load() {
            #region Error checking
            if (BassLoaded) {
                Trace.TraceInformation("There's no need to load BASS library again because it is already loaded.");
                return true;
            }
            #endregion

            string Directory = String.Format(
				@"{0}\Bass\{1}\",
				AppDomain.CurrentDomain.BaseDirectory,
				Environment.Is64BitProcess ? "x64" : "x86"
			);

			if (!LoadBass(Directory))
				return false;

			LoadBassPlugins(Directory);
            
			return true;
		}

        /// <summary>
        /// Initializes or reinitializes BASS output device.
        /// </summary>
        /// <returns>True if initialization was successful, false if not</returns>
        public static bool InitializeOutputDevice() {
            Trace.TraceInformation($"[BASS output init] Initializing BASS (Reinitialize={BassInitialized})");

            #region Error checking
            if (!BassLoaded) {
                Trace.TraceError($"[BASS output init] Cannot initialize BASS because it is not loaded. (BassLoaded={BassLoaded}");
                return false;
            }
            #endregion

            if (BassInitialized) {
                if (!Bass.BASS_Free()) {
                    Trace.TraceError($"[BASS output init] Cannot release BASS for reinitializing: Bass.BASS_Free returned false (BassErrorCode={Bass.BASS_ErrorGetCode().ToString()})");
                    return false;
                }
                else {
                    Trace.TraceInformation("[BASS output init] BASS released for reinitializing.");
                    BassInitialized = false;
                }
            }

            if (!Bass.BASS_Init(-1, 44800, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)) {
                Trace.TraceError($"[BASS output init] Cannot initialize BASS: (BassErrorCode={Bass.BASS_ErrorGetCode().ToString()})");
                return false;
            }

            Trace.TraceInformation("[BASS output init] BASS initialized successfully.");

            BassInitialized = true;
            return true;
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
		/// <returns>True if BASS is successfully loaded, false if not.</returns>
		private static bool LoadBass(string Directory) {
			string Filename = Directory + "bass.dll";

            #region Error checking (+Trace)
            if (!File.Exists(Filename)) {
				Trace.TraceError($"[BASS init] bass.dll file not found ({Filename})");
				return false;
			}
			else {
				Trace.TraceInformation("[BASS init] bass.dll found, trying to load it...");
			}
            #endregion

            if (!Bass.LoadMe(Directory)) {
				Trace.TraceError("[BASS init] bass.dll could not be loaded");
				return false;
			}
			else {
                BassLoaded = true;
                Trace.TraceInformation($"[BASS init] bass.dll loaded (BassVersion={BassVersion.ToString()})");
            }

			SupportedExtensions.AddRange(
				Bass.SupportedStreamExtensions.GetMatches(@"\W+([\w\d]+)").Select(x => x.ToLower())
			);

			return true;
		}

		/// <summary>
		/// Loads all BASS plugins from the given directory.
		/// </summary>
		/// <param name="Directory">The directory that contains the plugin DLLs.</param>
		private static void LoadBassPlugins(string Directory) {
			DirectoryInfo DirInfo = new DirectoryInfo(Directory);

			var Plugins = DirInfo.EnumerateFiles("bass*.dll").Where(x => !x.FullName.Contains("bass.dll"));

            AllPluginsLoaded = true;
			foreach (var File in Plugins) {
				int Result = Bass.BASS_PluginLoad(File.FullName);

				if (Result == 0) {
					Trace.TraceInformation($"[BASS init] Plugin loaded: {File.Name}");

					string PluginSupportedExtensions = Un4seen.Bass.Utils.BASSAddOnGetSupportedFileExtensions(File.FullName);
					SupportedExtensions.AddRange(
						PluginSupportedExtensions.GetMatches(@"\W+([\w\d]+)").Select(x => x.ToLower())
					);
				}
				else {
					Trace.TraceWarning($"[BASS init] Failed to load plugin: {File.Name} (Code {Result})");
                    AllPluginsLoaded = false;
				}
			}
		}
	}
}
