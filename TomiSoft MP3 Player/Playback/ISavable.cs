using System.IO;
using System.Threading.Tasks;

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
		/// Saves the media to the given stream.
		/// </summary>
		/// <param name="TargetStream">The stream where the media is written to.</param>
		/// <returns>
		/// A task that represents the process of the saving procedure. When the saving
		/// is finished, a bool value will represent whether the saving was successful or not.
		/// </returns>
		Task<bool> SaveToAsync(Stream TargetStream);
	}
}
