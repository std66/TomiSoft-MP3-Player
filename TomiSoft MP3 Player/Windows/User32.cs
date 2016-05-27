using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Win32 {
	public static class User32 {
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string ClassName, string Caption);
	}
}
