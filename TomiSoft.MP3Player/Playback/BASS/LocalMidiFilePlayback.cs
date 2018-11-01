using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.MP3Player.Utils;
using Un4seen.Bass.AddOn.Midi;

namespace TomiSoft.MP3Player.Playback.BASS {
    class LocalMidiFilePlayback : LocalAudioFilePlayback {
        public LocalMidiFilePlayback(string Filename, string SoundfontFilename) : base(Filename) {
            this.ApplySoundfontToStream(SoundfontFilename);
        }

        public LocalMidiFilePlayback(UnmanagedStream MediaStream, ISongInfo SongInfo, string SoundfontFilename)
            : base(MediaStream, SongInfo) {
            this.ApplySoundfontToStream(SoundfontFilename);
        }

        private void ApplySoundfontToStream(string SoundfontFilename) {
            int FontHandle = BassMidi.BASS_MIDI_FontInit(SoundfontFilename);
            if (FontHandle != 0) {
                BASS_MIDI_FONT Font = new BASS_MIDI_FONT(FontHandle, -1, 0);

                BassMidi.BASS_MIDI_StreamSetFonts(this.ChannelID, new BASS_MIDI_FONT[] { Font }, 1); // apply it to the stream
            }
        }
    }
}
