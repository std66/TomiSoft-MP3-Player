using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft_MP3_Player {
	[Serializable]
	class BassLoadException: Exception {
		public BassLoadException() : base() {

		}

		public BassLoadException(string Message)
			: base(Message) {

		}
	}
}
