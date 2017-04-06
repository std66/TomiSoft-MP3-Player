﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TomiSoft.MP3Player.Utils;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Provides a simple way of loading media to play.
	/// </summary>
	public class PlaybackFactory {
		/// <summary>
		/// Stores the last IPlaybackManager instance.
		/// </summary>
		private static IPlaybackManager lastInstance;

		/// <summary>
		/// Loads the given file.
		/// </summary>
		/// <param name="Filename">The file's path to load.</param>
		/// <returns>An IPlaybackManager instance that can handle the given file.</returns>
		/// <exception cref="NotSupportedException">when Filename is not supported by any playback methods</exception>
		public static IPlaybackManager LoadFile(string Filename) {
			string Extension = PlayerUtils.GetFileExtension(Filename);
			
			//In case of any file supported by BASS:
			if (BassManager.GetSupportedExtensions().Contains(Extension)) {
				lastInstance = new LocalAudioFilePlayback(Filename);
				return lastInstance;
			}

			Trace.TraceWarning($"[Playback] Unsupported file: {Filename}");
			throw new NotSupportedException("Nem támogatott fájlformátum");
		}

		/// <summary>
		/// Gets a Null-Playback manager instance.
		/// </summary>
		/// <param name="Volume">The volume to initialize with.</param>
		/// <returns>An <see cref="IPlaybackManager"/> that represents the Null-Playback manager instance</returns>
		public static IPlaybackManager NullPlayback(int Volume) {
			lastInstance = new NullPlayback() {
				Volume = Volume
			};

			return lastInstance;
		}

		/// <summary>
		/// Determines whether the given media is supported.
		/// </summary>
		/// <param name="Source">The source (a file's path or an URI) to check</param>
		/// <returns>True if the media is supported, false if not</returns>
		public static bool IsSupportedMedia(string Source) {
			if (File.Exists(Source)) {
				if (BassManager.IsSupportedFile(Source))
					return true;
			}

			return false;
		}
	}
}
