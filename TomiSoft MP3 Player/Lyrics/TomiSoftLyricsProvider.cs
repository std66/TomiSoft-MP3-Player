using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Lyrics {
    /// <summary>
    /// A lyrics provider that downloads the lyrics from http://tomisoft.site90.net.
    /// </summary>
    internal class TomiSoftLyricsProvider : ILyricsProvider {
        /// <summary>
        /// Finds a lyrics file asynchronously using the informations provided by an
        /// ISongInfo instance.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>A Task for an ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        public Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo) {
            return Task.Run(
                () => this.Search(SongInfo)
            );
        }

        /// <summary>
        /// Searches a lyrics on the internet.
        /// </summary>
        /// <param name="SongInfo">An ISongInfo instance that holds informations about a song.</param>
        /// <returns>An ILyricsReader instance if the lyrics is found or null otherwise.</returns>
        private ILyricsReader Search(ISongInfo SongInfo) {
            #region Error checking
            if (SongInfo == null)
                return null;
            #endregion

            using (WebClient Client = new WebClient()) {
                Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                string Data = $"title={UrlEncode(SongInfo.Title)}&artist={UrlEncode(SongInfo.Artist)}";
                string Response = Client.UploadString(
                    "http://tomisoft.site90.net/lyrics/api.getlrc.php?get_lyrics=true",
                    Data
                );

                if (Response != "NOTFOUND") {
                    try {
                        string Filename = Path.GetTempFileName();
                        File.WriteAllText(Filename, Response);

                        return LyricsLoader.LoadFile(Filename);
                    }
                    catch { }
                }
            }
            
            return null;
        }

        /// <summary>
        /// Encodes a string.
        /// </summary>
        /// <param name="Input">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        private string UrlEncode(string Input) {
            return HttpUtility.UrlEncode(Input);
        }
    }
}
