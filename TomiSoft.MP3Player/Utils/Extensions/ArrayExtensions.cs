using System;

namespace TomiSoft.MP3Player.Utils.Extensions {
	/// <summary>
	/// Contains extension methods for arrays.
	/// </summary>
	public static class ArrayExtensions {
		/// <summary>
		/// Returns a part of the array.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the array</typeparam>
		/// <param name="Array">The source array</param>
		/// <param name="FromIndex">The first element that is copied.</param>
		/// <param name="ToIndex">The last element that is copied.</param>
		/// <returns>A new array containing elements of the original array from FromIndex to ToIndex</returns>
		/// <exception cref="ArgumentException">when FromIndex is greater than ToIndex; when FromIndex or ToIndex is negative</exception>
		/// <exception cref="ArgumentNullException">when Array is null</exception>
		/// <exception cref="ArgumentOutOfRangeException">when ToIndex is equal to or greater than the element's count in Array</exception>
		public static T[] GetPartOfArray<T>(this T[] Array, int FromIndex, int ToIndex) {
			#region Error checking
			if (FromIndex > ToIndex)
				throw new ArgumentException($"{nameof(FromIndex)} must be less than or equal to {nameof(ToIndex)}");
			if (FromIndex < 0)
				throw new ArgumentException($"{nameof(FromIndex)} must be greater than or equal to 0.");
			if (ToIndex < 0)
				throw new ArgumentException($"{nameof(ToIndex)} must be greater than or equal to 0.");
			if (Array == null)
				throw new ArgumentNullException(nameof(Array));
			if (Array.Length <= ToIndex)
				throw new ArgumentOutOfRangeException(nameof(ToIndex));
			#endregion

			T[] Result = new T[ToIndex - FromIndex + 1];
			int ResultIndex = 0;
			for (int SourceIndex = FromIndex; SourceIndex <= ToIndex; SourceIndex++, ResultIndex++) {
				Result[ResultIndex] = Array[SourceIndex];
			}

			return Result;
		}
	}
}
