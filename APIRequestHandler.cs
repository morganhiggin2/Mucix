using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;

namespace Mucix
{
    class APIRequestHandler
    {
        public static object SQLiteConnection { get; private set; }

        /// <summary>
        /// get the list of new videos from a playlist
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public static List<string> getNewPlaylistVideos(string playlistID)
        {
            //if the sqlite file does not exist, create it
            if (!System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"data.db"))) 
            {
                SQLiteConnection = new SQLiteConnection();

                SQLiteConnection.CreateFile(Path.Combine(Directory.GetCurrentDirectory(), @"data.db"));
            }

            //read the json file
            //get the list of url links from the json file
            //download all the url links from the playlist
            //get rid of the ones that are already in there
            //return that list

            return null;
        }

        /// <summary>
        /// makes the api requests and sort though the json
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private static List<string> getAllSongsFromPlaylist(string playlistID)
        {
            return null;
        }

        /// <summary>
        /// get list of existing songs from the json file
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private static List<string> getExistingSongsFromPlaylist(string playlistID)
        {
            return null;
        }

        /// <summary>
        /// save new songs to json file
        /// </summary>
        /// <returns></returns>
        private static List<string> saveNewSongs(string playlistID)
        {
            //use sqlite
            //replace the previous list with the new one
            //save it
            return null;
        }

        /// <summary>
        /// downlaod the youtube audio to a specified location within the working directory 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="videoID"></param>
        public static void downloadYoutubeAudio(string location, string videoID)
        {

        }

        /// <summary>
        /// moves all old songs to the folder "previous"
        /// </summary>
        /// <param name="location"></param>
        public static void moveOldSongs(string location)
        {

        }

        /// <summary>
        /// run a bash command for windows
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        static string ExecuteBashCommand(string command)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thans to this we will pass everything as one command
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }
    }

}
