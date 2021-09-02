using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mucix.Models
{
    class Video
    {
        /// <summary>
        /// the playlist id it belongs to
        /// </summary>
        public string playlistID;

        /// <summary>
        /// the id for the url for this video in youtube
        /// </summary>
        public string urlID;

        /// <summary>
        /// the title of the video
        /// </summary>
        public string title;

        /// <summary>
        /// the title of the channel that made this video
        /// </summary>
        public string channelTitle;

        ///<summary>
        /// the date the song was created
        /// </summary>
        public string created;

        public Video(string playlistID, string urlID, string title, string channelTitle, string created)
        {
            this.playlistID = playlistID;
            this.urlID = urlID;
            this.title = title;
            this.channelTitle = channelTitle;
            this.created = created;
        }
    }
}
