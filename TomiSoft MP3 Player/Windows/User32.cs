using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Win32 {
	/// <summary>
	/// This class contains the interop code for user32.dll
	/// </summary>
	public static class User32 {
		/// <summary>
		/// Retrieves a handle to the top-level window whose class name and window name match the specified strings.
		/// This function does not search child windows. This function does not perform a case-sensitive search.
		/// 
		/// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499(v=vs.85).aspx
		/// </summary>
		/// <param name="ClassName">Specifies the window class name.</param>
		/// <param name="Caption">The window name (the window's title). If this parameter is NULL, all window names match.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string ClassName, string Caption);
	}
}
