using System.Diagnostics;
using System.Threading.Tasks;

namespace TomiSoft.ExternalApis.YoutubeDl.Extesions {
	public static class ProcessExtensions {
		public static Task WaitForExitAsync(this Process p) {
			return Task.Run(() => p.WaitForExit());
		}
	}
}
