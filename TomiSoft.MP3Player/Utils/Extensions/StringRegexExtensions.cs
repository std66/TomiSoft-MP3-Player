using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TomiSoft.MP3Player.Utils.Extensions {
	/// <summary>
	/// Extends the System.String instances with regular expression
	/// methods.
	/// </summary>
	public static class StringRegexExtensions {
		/// <summary>
		/// Gets all matches in the given string using the given pattern.
		/// </summary>
		/// <param name="Input">The string which in the pattern is searched.</param>
		/// <param name="Pattern">The searched pattern. Must contain exactly one capturing group.</param>
		/// <returns>The first capturing group's value in each matches.</returns>
		public static IEnumerable<string> GetMatches(this string Input, string Pattern) {
			Regex r = new Regex(Pattern);
			var result = r.Matches(Input);

			foreach (Match Current in result) {
				yield return Current.Groups[1].Value;
			}
		}

		/// <summary>
		/// Determines whether a string matches the given pattern.
		/// </summary>
		/// <param name="Input">The string to check.</param>
		/// <param name="Pattern">The regular expression.</param>
		/// <returns>True if the string matches the pattern or false if not.</returns>
		public static bool IsMatch(this string Input, string Pattern) {
			Regex r = new Regex(Pattern);
			return r.IsMatch(Input);
		}

		/// <summary>
		/// Gets the first match in the given string using the given pattern.
		/// </summary>
		/// <param name="Input">The string which in the pattern is searched.</param>
		/// <param name="Pattern">The searched pattern. Must contain exactly one capturing group.</param>
		/// <returns>The first capturing group's value of the first match or null if the pattern not found.</returns>
		public static string GetFirstMatch(this string Input, string Pattern) {
			Regex r = new Regex(Pattern);
			Match m = r.Match(Input);

			if (m != null) {
				return m.Groups[1].Value;
			}

			return null;
		}

		/// <summary>
		/// Extracts key-value pairs from the given string.
		/// </summary>
		/// <param name="Input">The string to extract key-value pairs from.</param>
		/// <param name="Pattern">The pattern to search. Must contain two named capturing groups.</param>
		/// <param name="KeyName">The name of the capturing group in the pattern which holds the key.</param>
		/// <param name="ValueName">The name of the capturing group in the pattern which holds the value.</param>
		/// <returns>All key-value pairs that found in the string.</returns>
		public static IEnumerable<KeyValuePair<string, string>> GetKeyValueMatches(this string Input, string Pattern, string KeyName, string ValueName) {
			Regex r = new Regex(Pattern);
			var result = r.Matches(Input);

			foreach (Match Current in result) {
				yield return new KeyValuePair<string, string> (
					Current.Groups[KeyName].Value,
					Current.Groups[ValueName].Value
				);
			}
		}

		/// <summary>
		/// Removes all the characters from the <see cref="string"/> that are forbidden in a path.
		/// </summary>
		/// <param name="Input">The <see cref="string"/> that represents the path</param>
		/// <returns>A <see cref="string"/>that does not contain forbidden characters</returns>
		public static string RemovePathInvalidChars(this string Input) {
			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(Input, "");
		}
	}
}
