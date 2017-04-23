using System;
using System.IO;
using System.Windows;

namespace TomiSoft.MP3Player.Utils {
	/// <summary>
	/// Contains useful methods.
	/// </summary>
	public class PlayerUtils {
		/// <summary>
		/// Extracts the extension from the given filename.
		/// </summary>
		/// <param name="Filename">The filename with or without the path.</param>
		/// <returns>The extension of the file in lowercase without the prepending dot.</returns>
		public static string GetFileExtension(string Filename) {
			#region Error checking
			if (!File.Exists(Filename))
				return String.Empty;
			#endregion

			FileInfo f = new FileInfo(Filename);
			return f.Extension.Substring(1).ToLower();
		}

		/// <summary>
		/// Determines whether the given string is a well-formatted URI.
		/// </summary>
		/// <param name="Input">The string to check.</param>
		/// <returns>True if the string is an URI, false if not.</returns>
		public static bool IsURI(string Input) {
			return Uri.IsWellFormedUriString(Input, UriKind.RelativeOrAbsolute);
		}

		/// <summary>
		/// Displays an error message box with the given title, content and an
		/// OK button.
		/// </summary>
		/// <param name="Title">The title of the window.</param>
		/// <param name="Text">The content of the window.</param>
		public static void ErrorMessageBox(string Title, string Text) {
			MessageBox.Show(Text, Title, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
