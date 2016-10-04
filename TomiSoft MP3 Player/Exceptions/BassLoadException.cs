using System;

namespace TomiSoft_MP3_Player
{
    [Serializable]
	class BassLoadException: Exception {
		public BassLoadException() : base() {

		}

		public BassLoadException(string Message)
			: base(Message) {

		}
	}
}
