﻿using System;
using TomiSoft.MP3Player.Common.Playback;
using TomiSoft.MP3Player.Playback;

namespace TomiSoft.MP3Player.Communication.Modules {
	public class PlayerModule : IServerModule {
		private IPlaybackManager Playback;
		private IAudioPeakMeter PeakMeter;

		public event EventHandler<string[]> OpenFiles;
		public event EventHandler NextSong;
		public event EventHandler PreviousSong;
		
		public string ModuleName {
			get {
				return "Player";
			}
		}

		public IPlaybackManager PlaybackManager {
			get {
				return this.Playback;
			}
			set {
				this.Playback = value;
				this.PeakMeter = value as IAudioPeakMeter;
			}
		}

		[ServerCommand]
		public IAudioPeakMeter GetPeakLevel() {
			return PeakMeter;
		}

		[ServerCommand]
		public double GetPlaybackPosition() {
			return this.Playback?.Position ?? 0;
		}

        [ServerCommand]
        public double GetSongLength() {
            return this.Playback?.Length ?? 0;
        }

		[ServerCommand]
		public void Play(params string[] Sources) {
			this.OpenFiles?.Invoke(this, Sources);
		}

		[ServerCommand]
		public void PlayNext() {
			this.NextSong?.Invoke(this, EventArgs.Empty);
		}

		[ServerCommand]
		public void PlayPrevious() {
			this.PreviousSong?.Invoke(this, EventArgs.Empty);
		}

        [ServerCommand]
        public void SetPlaybackPosition(string Position) {
            double pos;

            if (double.TryParse(Position, out pos) && this.PlaybackManager != null)
                this.PlaybackManager.Position = pos;
        }

        [ServerCommand]
        public void StartPlayback() {
            this.PlaybackManager?.Play();
        }

        [ServerCommand]
        public void PausePlayback() {
            this.PlaybackManager?.Pause();
        }

        [ServerCommand]
        public void StopPlayback() {
            this.PlaybackManager?.Stop();
        }
    }
}
