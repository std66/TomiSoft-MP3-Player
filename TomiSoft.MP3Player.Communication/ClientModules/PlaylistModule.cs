using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
        public IEnumerable<KeyValuePair<string, string>> Playlist() {
            XDocument doc = XDocument.Parse(this.GetPlaylistAsXml());

            return from c in doc.Descendants("song")
                   select new KeyValuePair<string, string>(
                       key: c.Element("a").Value,
                       value: c.Element("t").Value
                   );
        }

        /// <summary>
        /// Gets the current playlist in XML format.
        /// </summary>
        /// <returns>The XML document that represents the playlist.</returns>
        public string GetPlaylistAsXml() {
            this.Connection.Send("Player.ShowPlaylist");
            int Count = Convert.ToInt32(this.Connection.Read());

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
