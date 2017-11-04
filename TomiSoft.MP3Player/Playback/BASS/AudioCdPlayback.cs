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
	class AudioCdPlayback : BassPlaybackAbstract, ISavable {
		/// <summary>
		/// Stores the loaded file's path.
		/// </summary>
		private readonly string Filename;

		public AudioCdPlayback(string Filename) 
			: base(Bass.BASS_StreamCreateFile(Filename, 0, 0, BASSFlag.BASS_DEFAULT)) {
			this.Filename = Filename;

			if (this.SongInfo.Title == null) {
				this.songInfo = new SongInfo(this.songInfo) {
					Title = Path.GetFileNameWithoutExtension(Filename)
				};
			}
		}

		public string OriginalFilename {
			get {
				return Path.GetFileName(this.Filename);
			}
		}

		public string OriginalSource {
			get {
				return this.Filename;
			}
		}

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
		/// Stops the playback and copies the audio stream from CD to a <see cref="Stream"/> in MP3
		/// format asynchronously. If the song was playing before copying, playback will be restored.
		/// </summary>
		/// <param name="TargetStream">The <see cref="Stream"/> where the media is written to.</param>
		/// <param param name="Progress">An <see cref="IProgress{T}"/> instance that will be used to report the save progress. Can be null.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous process.</returns>
		public async Task<bool> SaveToAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress) {
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

			return true;
		}
	}
}
