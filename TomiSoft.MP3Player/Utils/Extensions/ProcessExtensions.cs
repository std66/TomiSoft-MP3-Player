using System.Diagnostics;
using System.Threading.Tasks;

namespace TomiSoft.MP3Player.Utils.Extensions {
	public static class ProcessExtensions {
		public static Task WaitForExitAsync(this Process p) {
			return Task.Run(() => p.WaitForExit());
		}
	}
}
