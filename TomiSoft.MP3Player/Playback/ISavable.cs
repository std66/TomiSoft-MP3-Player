using System;
using System.IO;
using System.Threading.Tasks;
using TomiSoft.MP3Player.Utils;

namespace TomiSoft.MP3Player.Playback {
	/// <summary>
	/// Represents a saveable media.
	/// </summary>
	public interface ISavable {
		/// <summary>
		/// Gets the original source (a local path or an URI) of the media.
		/// </summary>
		string OriginalSource {
			get;
		}

		/// <summary>
		/// Gets the original file name of the media.
		/// </summary>
		string OriginalFilename {
			get;
		}

		/// <summary>
		/// Gets the recommended file name of the media.
		/// </summary>
		string RecommendedFilename {
			get;
		}

		/// <summary>
		/// Saves the media to the given <see cref="Stream"/>.
		/// </summary>
		/// <param name="TargetStream">The <see cref="Stream"/> where the media is written to.</param>
        /// <param param name="Progress">An <see cref="IProgress{T}"/> instance that will be used to report the save progress. Can be null.</param>
		/// <returns>
		/// A task that represents the process of the saving procedure. When the saving
		/// is finished, a bool value will represent whether the saving was successful or not.
		/// </returns>
		Task<bool> SaveToAsync(Stream TargetStream, IProgress<LongOperationProgress> Progress);
	}
}
