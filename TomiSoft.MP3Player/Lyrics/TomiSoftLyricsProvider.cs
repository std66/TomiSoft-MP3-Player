using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;
using TomiSoft_MP3_Player;

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
        public async Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo) {
            #region Error checking
            if (SongInfo == null)
                return null;
            #endregion

            using (WebClient Client = new WebClient()) {
                Client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				Client.Headers[HttpRequestHeader.UserAgent] = $"{App.Name} (version {App.Version})";

                string Data = $"title={UrlEncode(SongInfo.Title)}&artist={UrlEncode(SongInfo.Artist)}";
                string Response;
                try {
                    Response = await Client.UploadStringTaskAsync(
                        "http://tomisoft.site90.net/lyrics/api.getlrc.php?get_lyrics=true",
                        Data
                    );
                }
                catch (WebException) {
                    return null;
                }

                if (Response != "NOTFOUND") {
                    string Filename = Path.GetTempFileName();
                        
                    if (await CreateFileAsync(Filename, Response))
                        return LyricsLoader.LoadFile(Filename);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a file and writes the specified content to it asynchronously.
        /// </summary>
        /// <param name="Filename">The file's path to create</param>
        /// <param name="Contents">The contents to be written into thee file</param>
        /// <returns>True if the file was successfully created, false if not.</returns>
        private async Task<bool> CreateFileAsync(string Filename, string Contents) {
            bool Result = false;

            try {
                using (StreamWriter sr = new StreamWriter(File.OpenWrite(Filename))) {
                    await sr.WriteAsync(Contents);
                }

                Result = true;
            }
            catch (IOException) {
                Result = false;
            }

            return Result;
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
