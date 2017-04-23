using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Lyrics {
    /// <summary>
    /// Represents a module that can look up a lyrics file.
    /// </summary>
    public interface ILyricsProvider {
        /// <summary>
        /// Finds a lyrics file asynchronously using the informations provided by an
        /// ISongInfo instance.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>A Task for an ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo);
    }
}
