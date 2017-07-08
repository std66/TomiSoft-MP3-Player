﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TomiSoft.MP3Player.Encoder.Lame;
using TomiSoft.MP3Player.MediaInformation;
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
		/// <returns>
		/// A <see cref="Task"/> that represents the process of the saving procedure. When the saving
		/// is finished, a bool value will represent whether the saving was successful or not.
		/// </returns>
		public virtual async Task<bool> SaveToAsync(Stream TargetStream) {
			#region Error checking
			if (TargetStream == null)
				return false;

			if (!TargetStream.CanWrite)
				return false;
			#endregion
			
            if (this.IsAudioCd) {
                await this.CopyFromAudioCd(TargetStream);
                return true;
            }

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

        private async Task CopyFromAudioCd(Stream TargetStream) {
            if (this.IsPlaying)
                this.Stop();

            ENCODEPROC proc = new ENCODEPROC(
                (handle, channel, buffer, length, user) => {
                    byte[] ManagedBuffer = new byte[length];
                    Marshal.Copy(buffer, ManagedBuffer, 0, length);

                    TargetStream.Write(ManagedBuffer, 0, length);
                }
            );

            int DecodingChannel = Bass.BASS_StreamCreateFile(this.Filename, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            int Handle = BassEnc.BASS_Encode_Start(DecodingChannel, (new Lame()).GetCommandLine(), BASSEncode.BASS_ENCODE_AUTOFREE, proc, IntPtr.Zero);

            await Task.Run(() => { 
                int BufferLength = 1024;
                byte[] Buffer = new byte[1024];

                int DataRead = 0;
                while (Bass.BASS_ChannelGetPosition(DecodingChannel) < Bass.BASS_ChannelGetLength(DecodingChannel)) { 
                    DataRead = Bass.BASS_ChannelGetData(DecodingChannel, Buffer, BufferLength);
                    //BassEnc.BASS_Encode_Write(Handle, Buffer, DataRead);
                }

                BassEnc.BASS_Encode_Stop(Handle);
            });

            this.ChannelID = Bass.BASS_StreamCreateFile(this.Filename, 0, 0, BASSFlag.BASS_DEFAULT);
        }
	}
}
