using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Lyrics {
    /// <summary>
    /// This class can be used to search for a lyrics file.
    /// </summary>
    public class LyricsProvider {
        /// <summary>
        /// Stores all the lyrics providers.
        /// </summary>
        private static readonly ILyricsProvider[] Providers = {
            new FileLyricsProvider(),
            new TomiSoftLyricsProvider()
        };

        /// <summary>
        /// Finds a lyrics file asynchronously using the informations provided by an
        /// ISongInfo instance.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>A Task for an ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        public static async Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo) {
            foreach (ILyricsProvider Provider in Providers) {
                ILyricsReader reader = await Provider.FindLyricsAsync(SongInfo);

                if (reader != null)
                    return reader;
            }

            return null;
        }
    }
}
