using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TomiSoft.MP3Player.Common.MediaInformation;

namespace TomiSoft.MP3Player.Communication.ClientModules {
    /// <summary>
    /// Provides commands for controlling the playlist.
    /// </summary>
    public class PlaylistModule {
        private readonly ServerConnection Connection;

        /// <summary>
        /// Gets a sequence that represents the current playlist.
        /// The key is the artist of the song and the value
        /// is it's title.
        /// </summary>
        /// <returns>A sequence that represents the playlist.</returns>
        public IEnumerable<ISongMetadata> Playlist() {
            XDocument doc = XDocument.Parse(this.GetPlaylistAsXmlString());

            return from c in doc.Descendants("song")
                   select new SongMetadata(
                       Title: c.Element("t").Value,
                       Artist: c.Element("a").Value,
                       Album: null
                   );
        }

        /// <summary>
        /// Gets the current playlist in XML format.
        /// </summary>
        /// <returns>The XML document that represents the playlist.</returns>
        public string GetPlaylistAsXmlString() {
            this.Connection.Send("Player.ShowPlaylist");
            int Count = this.Connection.ReadInt32();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<playlist>");
            for (int i = 0; i < Count; i++) {
                string CurrentMedia = this.Connection.Read();
                sb.Append($"<song>{CurrentMedia}</song>");
            }
            sb.Append("</playlist>");

            return sb.ToString();
        }

        internal PlaylistModule(ServerConnection Connection) {
            this.Connection = Connection;
        }
    }
}
