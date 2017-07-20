using System;
using System.Collections.Generic;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the playback.
    /// </summary>
    public class PlaybackModule {
        private readonly ServerConnection Connection;

        internal PlaybackModule(ServerConnection Connection) {
            this.Connection = Connection;
        }

        /// <summary>
        /// Sends the command to the server to play the specified files.
        /// </summary>
        /// <param name="MediaSources">A sequence of the media sources (eg. filenames, URIs)</param>
        public void OpenMedia(IEnumerable<string> MediaSources) {
            #region Error checking
            if (MediaSources == null)
                return;
            #endregion

            this.Connection.Send($"Player.Play;{String.Join(";", MediaSources)}");
        }

        /// <summary>
        /// Sends the command to the server to play the given file.
        /// </summary>
        /// <param name="Source">The source of the media to be played</param>
        public void OpenMedia(string Source) {
            this.Connection.Send($"Player.Play;{Source}");
        }

        /// <summary>
        /// Sends the command to the server to play the next song in
        /// the playlist.
        /// </summary>
        public void PlayNextSongOnPlaylist() {
            this.Connection.Send("Player.PlayNext");
        }

        /// <summary>
        /// Sends the command to the server to play the previous song
        /// in the playlist.
        /// </summary>
        public void PlayPreviousSongOnPlaylist() {
            this.Connection.Send("Player.PlayPrevious");
        }

        /// <summary>
        /// Gets the playback position and the length of the song in
        /// seconds.
        /// </summary>
        /// <param name="Length">The variable to store the length of the song in seconds</param>
        /// <returns>The playback position in seconds</returns>
        public double GetPlaybackPosition(out double Length) {
            this.Connection.Send("Player.PlaybackPosition");
            string[] Result = this.Connection.Read().Split('/');

            if (Result.Length == 2) {
                Length = Convert.ToDouble(Result[1]);
                return Convert.ToDouble(Result[0]);
            }
            else {
                Length = 0;
                return 0;
            }
        }

        /// <summary>
        /// Gets the current peak level.
        /// </summary>
        /// <param name="LeftPeak">The variable to store the left peak level</param>
        /// <param name="RightPeak">The variable to store the right peak level</param>
        /// <returns>The maximum value of the peak level</returns>
        public int PeakLevel(out int LeftPeak, out int RightPeak) {
            this.Connection.Send("Player.PeakLevel");
            string[] Result = this.Connection.Read().Split('/');

            LeftPeak = Convert.ToInt32(Result[0]);
            RightPeak = Convert.ToInt32(Result[1]);

            return 32768;
        }
    }
}
