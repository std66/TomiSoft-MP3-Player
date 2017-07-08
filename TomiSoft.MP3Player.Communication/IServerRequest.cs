using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Communication {
	public interface IServerRequest<T> {
		string ModuleName { get; }
		string Command { get; }
		T Arguments { get; }
	}
}
