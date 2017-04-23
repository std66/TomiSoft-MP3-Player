using System.IO;
using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Lyrics {
    /// <summary>
    /// Represents a lyrics provider that loads the lyrics from a local file.
    /// </summary>
    internal class FileLyricsProvider : ILyricsProvider {
        /// <summary>
        /// Finds a lyrics file asynchronously using the informations provided by an
        /// ISongInfo instance.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>A Task for an ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        public Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo) {
            #region Error checking
            if (SongInfo == null)
                return null;
            #endregion

            return Task.Run(
                () => this.ScanFileDirectory(SongInfo)
            );
        }

        /// <summary>
        /// Performs a scan in the directory that contains the music file for a
        /// matching lyrics file.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>An ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        private ILyricsReader ScanFileDirectory(ISongInfo SongInfo) {
            string Dir = Path.GetDirectoryName(SongInfo.Source);

            #region Error checking
            if (!Directory.Exists(Dir))
                return null;
            #endregion

            //Try to find a lyrics file with the same name as in SongInfo.
            //Eg. if we have "test.mp3", try to find "test.lrc".
            foreach (var Ext in LyricsLoader.SupportedExtensions) {
                string Filename = Path.ChangeExtension(SongInfo.Source, Ext);

                if (File.Exists(Filename)) {
                    return LyricsLoader.LoadFile(Filename);
                }
            }

            //Scan all the lyrics files in the same folder.
            foreach (var File in LyricsLoader.FindAllLyricsFiles(Dir)) {
                try {
                    ILyricsReader reader = LyricsLoader.LoadFile(File);
                    if (reader.Title == SongInfo.Title && reader.Artist == SongInfo.Artist)
                        return reader;
                }
                catch {}
            }

            //If lyrics was not found, null is returned.
            return null;
        }
    }
}
