using System;
using System.Collections.Generic;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the playback.
    /// </summary>
    public class PlaybackModule {
        private readonly ServerConnection Connection;

        /// <summary>
        /// Gets or sets the playback position.
        /// </summary>
        public double PlaybackPosition {
            get {
                this.Connection.Send("Player.GetPlaybackPosition");
                return this.Connection.ReadDouble();
            }

            set {
                if (value < 0 || value > SongLength)
                    throw new ArgumentOutOfRangeException($"{nameof(value)} should be between 0 and {SongLength}");

                this.Connection.Send($"Player.SetPlaybackPosition;{value}");
            }
        }

        /// <summary>
        /// Gets the length of the currently playing song.
        /// </summary>
        public double SongLength {
            get {
                this.Connection.Send("Player.GetSongLength");
                return this.Connection.ReadDouble();
            }
        }

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
        /// Sends the command to the server to start the playback.
        /// </summary>
        public void Play() {
            this.Connection.Send("Player.StartPlayback");
        }

        /// <summary>
        /// Sends the command to the server to pause the playback.
        /// </summary>
        public void Pause() {
            this.Connection.Send("Player.PausePlayback");
        }

        /// <summary>
        /// Sends the command to the server to stop the playback.
        /// </summary>
        public void Stop() {
            this.Connection.Send("Player.StopPlayback");
        }

        /// <summary>
        /// Gets the current peak level.
        /// </summary>
        /// <param name="LeftPeak">The variable to store the left peak level</param>
        /// <param name="RightPeak">The variable to store the right peak level</param>
        /// <returns>The maximum value of the peak level</returns>
        public int GetPeakLevel(out int LeftPeak, out int RightPeak) {
            this.Connection.Send("Player.PeakLevel");
            string[] Result = this.Connection.Read().Split('/');

            LeftPeak = Convert.ToInt32(Result[0]);
            RightPeak = Convert.ToInt32(Result[1]);

            return 32768;
        }
    }
}
