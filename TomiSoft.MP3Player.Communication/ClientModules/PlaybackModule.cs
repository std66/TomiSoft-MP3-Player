using System;
using System.Collections.Generic;
using System.Linq;

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
                ServerResponse<double> Response = this.Connection.Send<double>(
                    new ServerRequest("Player", "GetPlaybackPosition")
                );

                Response.Check();

                return Response.Result;
            }

            set {
                if (value < 0 || value > SongLength)
                    throw new ArgumentOutOfRangeException($"{nameof(value)} should be between 0 and {SongLength}");

                ServerResponse Response = this.Connection.Send(
                    new ServerRequest("Player", "SetPlaybackPosition", new List<string>() {
                        value.ToString()
                    })
                );

                Response.Check();
            }
        }

        /// <summary>
        /// Gets the length of the currently playing song.
        /// </summary>
        public double SongLength {
            get {
                ServerResponse<double> Response = this.Connection.Send<double>(
                    new ServerRequest("Player", "GetSongLength")
                );

                Response.Check();

                return Response.Result;
            }
        }

        internal PlaybackModule(ServerConnection Connection) {
            this.Connection = Connection;
        }

        /// <summary>
        /// Sends the command to the server to play the specified files.
        /// </summary>
        /// <param name="MediaSources">A sequence of the media sources (eg. filenames, URIs)</param>
        public bool OpenMedia(IEnumerable<string> MediaSources) {
            #region Error checking
            if (MediaSources == null)
                return false;
            #endregion

            ServerResponse Response = this.Connection.Send(
                new ServerRequest("Player", "Play", MediaSources.ToList())
            );

            return Response.RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the given file.
        /// </summary>
        /// <param name="Source">The source of the media to be played</param>
        public bool OpenMedia(string Source) {
            return this.Connection.Send(
                new ServerRequest("Player", "Play", new List<string> {
                    Source
                })
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the next song in
        /// the playlist.
        /// </summary>
        public bool PlayNextSongOnPlaylist() {
            return this.Connection.Send(
                new ServerRequest("Player", "PlayNext")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the previous song
        /// in the playlist.
        /// </summary>
        public bool PlayPreviousSongOnPlaylist() {
            return this.Connection.Send(
                new ServerRequest("Player", "PlayPrevious")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to start the playback.
        /// </summary>
        public bool Play() {
            return this.Connection.Send(
                new ServerRequest("Player", "StartPlayback")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to pause the playback.
        /// </summary>
        public bool Pause() {
            return this.Connection.Send(
                new ServerRequest("Player", "PausePlayback")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to stop the playback.
        /// </summary>
        public bool Stop() {
            return this.Connection.Send(
                new ServerRequest("Player", "StopPlayback")
            ).RequestSucceeded;
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
