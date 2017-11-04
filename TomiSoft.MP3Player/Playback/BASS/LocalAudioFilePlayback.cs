using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TomiSoft.MP3Player.Encoder.Lame;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Utils;
using TomiSoft.MP3Player.Utils.Extensions;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;

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
		/// If the file is loaded from an unmanaged memory location, this field
		/// holds it's informations.
		/// </summary>
		private UnmanagedStream AllocatedMemory;

        /// <summary>
        /// Gets if the opened file is a track from an audio CD.
        /// </summary>
        public bool IsAudioCd {
            get {
                string Extension = Path.GetExtension(this.Filename).ToLower();
                return Extension == ".cda";
            }
        }

		/// <summary>
		/// Initializes a new instance of <see cref="LocalAudioFilePlayback"/> using
		/// the given filename.
		/// </summary>
		/// <param name="Filename">The file to play.</param>
		public LocalAudioFilePlayback(string Filename)
			:base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)){
			this.Filename = Filename;

			if (this.SongInfo.Title == null) {
				this.songInfo = new SongInfo(this.songInfo) {
					Title = Path.GetFileNameWithoutExtension(Filename)
				};
			}
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
					return this.Filename.RemovePathInvalidChars();

				string Extension = Path.GetExtension(this.OriginalFilename);

				if (this.SongInfo.Artist != null)
					return $"{this.SongInfo.Artist} - {this.SongInfo.Title}{Extension}".RemovePathInvalidChars();
				
				string Result = $"{this.SongInfo.Title}{Extension}".RemovePathInvalidChars();

                return (this.IsAudioCd) ? Path.ChangeExtension(Result, "mp3") : Result;
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
			
            if (this.IsAudioCd) {
                await this.CopyFromAudioCdAsync(TargetStream, Progress);
                return true;
            }

            LongOperationProgress Status = new LongOperationProgress {
                IsIndetermine = true,
                Maximum = 1,
                Position = 0
            };

            Progress?.Report(Status);

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

        /// <summary>
        /// Stops the playback and copies the audio stream from CD to a <see cref="Stream"/> in MP3
        /// format asynchronously. If the song was playing before copying, playback will be restored.
        /// </summary>
        /// <param name="TargetStream">The <see cref="Stream"/> where the media is written to.</param>
        /// <param param name="Progress">An <see cref="IProgress{T}"/> instance that will be used to report the save progress. Can be null.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
        private async Task CopyFromAudioCdAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress) {
            bool RequiresRestore = this.IsPlaying;
            double Position = this.Position;

            if (this.IsPlaying) {
                this.Stop();
            }

            ENCODEPROC proc = new ENCODEPROC(
                (handle, channel, buffer, length, user) => {
                    byte[] ManagedBuffer = new byte[length];
                    Marshal.Copy(buffer, ManagedBuffer, 0, length);

                    TargetStream.Write(ManagedBuffer, 0, length);
                }
            );
            
            int DecodingChannel = Bass.BASS_StreamCreateFile(this.Filename, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            long DecodingChannelLength = Bass.BASS_ChannelGetLength(DecodingChannel);
            int Handle = BassEnc.BASS_Encode_Start(DecodingChannel, (new Lame()).GetCommandLine(), BASSEncode.BASS_ENCODE_AUTOFREE, proc, IntPtr.Zero);

            LongOperationProgress Status = new LongOperationProgress {
                IsIndetermine = false,
                Maximum = DecodingChannelLength,
                Position = 0
            };

            Progress?.Report(Status);

            await Task.Run(() => { 
                int BufferLength = 10240;
                byte[] Buffer = new byte[BufferLength];
                
                while (Bass.BASS_ChannelGetPosition(DecodingChannel) < DecodingChannelLength) { 
                    Status.Position += Bass.BASS_ChannelGetData(DecodingChannel, Buffer, BufferLength);
                    Progress?.Report(Status);
                }

                BassEnc.BASS_Encode_Stop(Handle);
            });

            this.ChannelID = Bass.BASS_StreamCreateFile(this.Filename, 0, 0, BASSFlag.BASS_DEFAULT);
            if (RequiresRestore) {
                this.Position = Position;
                this.Play();
            }
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
