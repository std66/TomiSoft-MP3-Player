using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomiSoft.Music.Lyrics
{
    /// <summary>
    /// Gets informations about a lyrics file.
    /// </summary>
    public interface ILyricsMetadata
    {
        /// <summary>
		/// Gets the album of the song.
		/// </summary>
		string Album { get; }

        /// <summary>
        /// Gets the artist of the song.
        /// </summary>
        string Artist { get; }

        /// <summary>
		/// Gets the length of the song that this lyrics fits for.
		/// </summary>
		double Length { get; }

        /// <summary>
        /// Gets the title of the song.
        /// </summary>
        string Title { get; }
    }
}
