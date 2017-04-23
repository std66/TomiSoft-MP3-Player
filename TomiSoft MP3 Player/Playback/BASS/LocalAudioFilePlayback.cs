using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace TomiSoft.MP3Player.Playback.BASS {
	/// <summary>
	/// Provides a file playback method using BASS.
	/// </summary>
	internal class LocalAudioFilePlayback : BassPlaybackAbstract, ISavable {
		/// <summary>
		/// Stores the loaded file's path.
		/// </summary>
		private readonly string Filename;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalAudioFilePlayback"/> using
		/// the given filename.
		/// </summary>
		/// <param name="Filename">The file to play.</param>
		public LocalAudioFilePlayback(string Filename)
			:base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)){
			this.Filename = Filename;
		}

		/// <summary>
		/// Gets the original file name of the media.
		/// </summary>
		public string OriginalFilename {
			get {
				return Path.GetFileName(this.Filename);
			}
		}

		/// <summary>
		/// Gets the original source (a local path or an URI) of the media.
		/// </summary>
		public string OriginalSource {
			get {
				return this.Filename;
			}
		}

		/// <summary>
		/// Gets the recommended file name of the media.
		/// </summary>
		public string RecommendedFilename {
			get {
				if (this.SongInfo.Title == null)
					return this.Filename;

				string Extension = Path.GetExtension(this.OriginalFilename);

				if (this.SongInfo.Artist != null)
					return $"{this.SongInfo.Artist} - {this.SongInfo.Title}{Extension}";

				return $"{this.SongInfo.Title}{Extension}";
			}
		}

		/// <summary>
		/// Saves the media to the given <see cref="Stream"/>.
		/// </summary>
		/// <param name="TargetStream">The <see cref="Stream"/> where the media is written to.</param>
		/// <returns>
		/// A <see cref="Task"/> that represents the process of the saving procedure. When the saving
		/// is finished, a bool value will represent whether the saving was successful or not.
		/// </returns>
		public async Task<bool> SaveToAsync(Stream TargetStream) {
			#region Error checking
			if (TargetStream == null)
				return false;

			if (!TargetStream.CanWrite)
				return false;
			#endregion
			
			try {
				using (Stream Source = File.OpenRead(this.Filename)) {
					await Source.CopyToAsync(TargetStream);
				}
			}
			catch (Exception) {
				return false;
			}

			return true;
		}
	}
}
