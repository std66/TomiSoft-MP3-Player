using System;
using System.IO;
using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Extensions;
using Un4seen.Bass;

namespace TomiSoft.MP3Player.Playback.BASS {
    /// <summary>
    /// Provides a file playback method using BASS.
    /// </summary>
    internal class LocalAudioFilePlayback : BassPlaybackAbstract, ISavable {
		/// <summary>
		/// If the file is loaded from an unmanaged memory location, this field
		/// holds it's informations.
		/// </summary>
		private UnmanagedStream AllocatedMemory;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalAudioFilePlayback"/> using
		/// the given filename.
		/// </summary>
		/// <param name="Filename">The file to play.</param>
		public LocalAudioFilePlayback(string Filename)
			:base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)){
			
		}

		/// <summary>
		/// Initializes a new instance of <see cref="LocalAudioFilePlayback"/> using
		/// the given <see cref="UnmanagedStream"/> and its song informations.
		/// </summary>
		/// <param name="MediaStream">An <see cref="UnmanagedStream"/> that represents the file copied to an unmanaged memory location.</param>
		/// <param name="SongInfo">An <see cref="ISongInfo"/> instance that holds informations about the media.</param>
		public LocalAudioFilePlayback(UnmanagedStream MediaStream, ISongInfo SongInfo)
			: base(Bass.BASS_StreamCreateFile(MediaStream.PointerToUnmanagedMemory, 0, MediaStream.Length, BASSFlag.BASS_DEFAULT)) {
			this.songInfo = SongInfo;
			this.AllocatedMemory = MediaStream;
		}

		/// <summary>
		/// Gets the original file name of the media.
		/// </summary>
		public string OriginalFilename {
			get {
				return Path.GetFileName(this.songInfo.Source);
			}
		}

		/// <summary>
		/// Gets the original source (a local path or an URI) of the media.
		/// </summary>
		public string OriginalSource {
			get {
				return this.songInfo.Source;
			}
		}

		/// <summary>
		/// Gets the recommended file name of the media.
		/// </summary>
		public string RecommendedFilename {
			get {
				if (this.SongInfo.Title == null)
					return this.songInfo.Source.RemovePathInvalidChars();

				string Extension = ".mp3";

				if (this.SongInfo.Artist != null)
					return $"{this.SongInfo.Artist} - {this.SongInfo.Title}{Extension}".RemovePathInvalidChars();
				
                return $"{this.SongInfo.Title}{Extension}".RemovePathInvalidChars();
			}
		}

        /// <summary>
        /// Saves the media to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="TargetStream">The <see cref="Stream"/> where the media is written to.</param>
        /// <param param name="Progress">An <see cref="IProgress{T}"/> instance that will be used to report the save progress. Can be null.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the process of the saving procedure. When the saving
        /// is finished, a bool value will represent whether the saving was successful or not.
        /// </returns>
        public virtual async Task<bool> SaveToAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress) {
			#region Error checking
			if (TargetStream == null)
				return false;

			if (!TargetStream.CanWrite)
				return false;
			#endregion
			
            LongOperationProgress Status = new LongOperationProgress {
                IsIndetermine = true,
                Maximum = 1,
                Position = 0
            };

            Progress?.Report(Status);

            try {
				await this.AllocatedMemory.CopyToAsync(TargetStream, Progress);
			}
			catch (Exception) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Releases all resources used by this instance.
		/// </summary>
		public override void Dispose() {
			base.Dispose();

			if (this.AllocatedMemory != null)
				this.AllocatedMemory.Dispose();
		}
	}
}
