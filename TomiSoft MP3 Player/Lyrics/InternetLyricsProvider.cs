using System.Threading.Tasks;
using TomiSoft.MP3Player.MediaInformation;
using TomiSoft.Music.Lyrics;

namespace TomiSoft.MP3Player.Lyrics {
    class InternetLyricsProvider : ILyricsProvider {
        public Task<ILyricsReader> FindLyricsAsync(ISongInfo SongInfo) {
            return Task.Run(
                () => this.DoNothing()
            );
        }

        private ILyricsReader DoNothing() {
            return null;
        }
    }
}
