using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft_MP3_Player {
	[Serializable]
	class BassInitializationException : Exception {
		public BassInitializationException() : base() {

		}

		public BassInitializationException(string Message) : base(Message) {

		}
	}
}
