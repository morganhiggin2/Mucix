using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mucix.Models
{
    class Song
    {
        /// <summary>
        /// the id for the url for this video in youtube
        /// </summary>
        public string urlID;

        /// <summary>
        /// the title of the video
        /// </summary>
        public string title;

        /// <summary>
        /// the artist of the song
        /// </summary>
        public string artist;
        
        /// <summary>
        /// the title of the channel that made this video
        /// </summary>
        public string channelTitle;

        /// <summary>
        /// the genre of the song
        /// </summary>
        public string genre;

        ///<summary>
        /// the date the song was created
        /// </summary>
        DateTime created;

        public Song(string urlID, string title, string artist, string channelTitle, string genere, DateTime created)
        {
            this.urlID = urlID;
            this.title = title;
            this.artist = artist;
            this.channelTitle = channelTitle;
            this.genre = genre;
            this.created = created;
        }
    }
}
