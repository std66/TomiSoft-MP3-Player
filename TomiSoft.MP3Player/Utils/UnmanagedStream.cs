using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Utils {
	class UnmanagedStream : IDisposable {
		private GCHandle gcHandle;

		public IntPtr PointerToUnmanagedMemory {
			get {
				return this.gcHandle.AddrOfPinnedObject();
			}
		}

		public int Length {
			get;
			private set;
		}

		private UnmanagedStream(GCHandle Handle, int Length) {
			this.gcHandle = Handle;
			this.Length = Length;
		}

		public void Dispose() {
			if (this.gcHandle.IsAllocated)
				this.gcHandle.Free();
		}

		public async Task<bool> CopyToAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress) {
			byte[] Source = null;

			if (this.gcHandle.IsAllocated)
				Source = this.gcHandle.Target as byte[];

			#region Error checking
			if (!TargetStream.CanWrite)
				return false;

			if (Source == null)
				return false;
			#endregion

			LongOperationProgress p = new LongOperationProgress {
				IsIndetermine = false,
				Maximum = Source.Length,
				Position = 0
			};

			int Position = 0;
			while (Position < Source.Length) {
				long BytesLeft = Source.Length - Position;
				int BytesToWrite = BytesLeft > 10240 ? 10240 : (int)BytesLeft;

				await TargetStream.WriteAsync(Source, Position, BytesToWrite);

				Position += BytesToWrite;
				p.Position += BytesToWrite;

				Progress?.Report(p);
			}

			return true;
		}

		public static async Task<UnmanagedStream> CreateFromStream(Stream SourceStream, IProgress<LongOperationProgress> Progress) {
			LongOperationProgress status = new LongOperationProgress {
				IsIndetermine = false,
				Maximum = SourceStream.Length,
				Position = 0
			};

			SourceStream.Position = 0;

			byte[] ManagedBuffer = new byte[SourceStream.Length];

			int ManagedBufferPosition = 0;
			
			while (SourceStream.Position < SourceStream.Length) {
				long BytesLeft = SourceStream.Length - SourceStream.Position;
				int BytesToRead = BytesLeft > 10240 ? 10240 : (int)BytesLeft;

				ManagedBufferPosition += await SourceStream.ReadAsync(ManagedBuffer, (int)ManagedBufferPosition, BytesToRead);

				status.Position = ManagedBufferPosition;
				Progress?.Report(status);
			}

			GCHandle Handle = GCHandle.Alloc(ManagedBuffer, GCHandleType.Pinned);

			return new UnmanagedStream(Handle, (int)SourceStream.Length);
		}
	}
}
