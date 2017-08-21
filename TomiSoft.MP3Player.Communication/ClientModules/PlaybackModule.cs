using System;
using System.Collections.Generic;
using System.Linq;
using TomiSoft.MP3Player.Common.Playback;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the playback.
    /// </summary>
    public class PlaybackModule {
		private readonly string ServerModule = "Player";
		private readonly ServerConnection Connection;

        /// <summary>
        /// Gets or sets the playback position.
        /// </summary>
        public double PlaybackPosition {
            get {
                ServerResponse<double> Response = this.Connection.Send<double>(
                    new ServerRequest(ServerModule, "GetPlaybackPosition")
                );

                Response.Check();

                return Response.Result;
            }

            set {
                if (value < 0 || value > SongLength)
                    throw new ArgumentOutOfRangeException($"{nameof(value)} should be between 0 and {SongLength}");

                ServerResponse Response = this.Connection.Send(
                    new ServerRequest(ServerModule, "SetPlaybackPosition", value.ToString())
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
                    new ServerRequest(ServerModule, "GetSongLength")
                );

                Response.Check();

                return Response.Result;
            }
        }

		/// <summary>
		/// Gets the current level of the audio signal.
		/// </summary>
		public IAudioPeakMeter PeakLevel {
			get {
				var Response = this.Connection.Send<AudioPeakMeter>(
					new ServerRequest(ServerModule, "GetPeakLevel")
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
                new ServerRequest(ServerModule, "Play", MediaSources.ToArray())
            );

            return Response.RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the given file.
        /// </summary>
        /// <param name="Source">The source of the media to be played</param>
        public bool OpenMedia(string Source) {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "Play", Source)
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the next song in
        /// the playlist.
        /// </summary>
        public bool PlayNextSongOnPlaylist() {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "PlayNext")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to play the previous song
        /// in the playlist.
        /// </summary>
        public bool PlayPreviousSongOnPlaylist() {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "PlayPrevious")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to start the playback.
        /// </summary>
        public bool Play() {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "StartPlayback")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to pause the playback.
        /// </summary>
        public bool Pause() {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "PausePlayback")
            ).RequestSucceeded;
        }

        /// <summary>
        /// Sends the command to the server to stop the playback.
        /// </summary>
        public bool Stop() {
            return this.Connection.Send(
                new ServerRequest(ServerModule, "StopPlayback")
            ).RequestSucceeded;
        }
    }
}
