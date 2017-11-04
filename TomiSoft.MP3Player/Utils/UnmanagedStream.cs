using System;
using System.Diagnostics;
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
