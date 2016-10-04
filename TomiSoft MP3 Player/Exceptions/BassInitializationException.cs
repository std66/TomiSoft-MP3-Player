using System;

namespace TomiSoft_MP3_Player
{
    [Serializable]
	class BassInitializationException : Exception {
		public BassInitializationException() : base() {

		}

		public BassInitializationException(string Message) : base(Message) {

		}
	}
}
