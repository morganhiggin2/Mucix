using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Mucix.Models;
using System.Globalization;

namespace Mucix
{
    class APIRequestHandler
    {
        static string databasePath = Path.Combine(Directory.GetCurrentDirectory(), @"database.db");
        static string oldSongsPath = Path.Combine(Directory.GetCurrentDirectory(), @"Songs\Old");
        static string newSongsPath = Path.Combine(Directory.GetCurrentDirectory(), @"Songs\New");
        static string youtubeDLPath = Path.Combine(Directory.GetCurrentDirectory(), @"youtube-dl");

        static ProgressBar progress;

        public static void init()
        {
            //if the sqlite file does not exist, create it
            if (!System.IO.File.Exists(databasePath))
            {
                using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
                {
                    //open the connection
                    connection.Open();

                    //create playlistVideos table
                    //create the command
                    SqliteCommand command = new SqliteCommand("CREATE TABLE IF NOT EXISTS PlaylistVideos (playlist_id varchar(255), url varchar(255))", connection);

                    //execute the command
                    command.ExecuteNonQuery();

                    //create playlistList table
                    //create the command
                    command = new SqliteCommand("CREATE TABLE IF NOT EXISTS PlaylistList (playlist_id varchar(255), title varchar(255))", connection);

                    //execute the command
                    command.ExecuteNonQuery();

                    //create playlistList table
                    //create the command
                    command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Properties (property varchar(255), value varchar(255))", connection);

                    //execute the command
                    command.ExecuteNonQuery();

                    //add blank api key
                    //create the command
                    command = new SqliteCommand("INSERT INTO Properties (property, value) VALUES ('" + "Google API Key" + @"', '" + " " + @"')", connection);

                    //execute the command
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }

            //see if the old music folder exists, if not, create it
            if (!System.IO.Directory.Exists(oldSongsPath))
            {
                System.IO.Directory.CreateDirectory(oldSongsPath);
            }

            //see if the new music folder exists, if not, create it
            if (!System.IO.Directory.Exists(newSongsPath))
            {
                System.IO.Directory.CreateDirectory(newSongsPath);
            }

            //see if the playlist folders exist
            List<PlaylistHandlerPackage> playlists = getPlaylists();

            //get the current api key
            Program.GOOGLE_API_KEY = getApiKey();

            foreach(PlaylistHandlerPackage playlist in playlists)
            {
                //see if the old music playlist folder exists for this playlist, if not, create it
                if (!System.IO.Directory.Exists(oldSongsPath + @"\" + playlist.playlistName))
                {
                    System.IO.Directory.CreateDirectory(oldSongsPath + @"\" + playlist.playlistName);
                }

                //see if the new music playlist folder exists for this playlist, if not, create it
                if (!System.IO.Directory.Exists(newSongsPath + @"\" + playlist.playlistName))
                {
                    System.IO.Directory.CreateDirectory(newSongsPath + @"\" + playlist.playlistName);
                }
            }
        }

        public static void getNewPlaylistSongs()
        {
            //get a list of the playlists
            List<PlaylistHandlerPackage> playlists = getPlaylists();

            //get tyhe progressbar ready
            progress = new ProgressBar(10);

            //get the list of new videos
            List<Video> newVideos = getNewPlaylistVideos(playlists);

            //convert video objects to song objects
            List<Song> newSongs = convertVideosToSongs(newVideos);

            //if there are no new songs, then we are done
            if (newSongs.Count == 0)
            {
                progress.progressStatus(1000);
                Console.WriteLine("No new songs to download");

                return;
            }

            //move the old songs to their respecitve new songs folder
            moveOldSongs(playlists);

            //download the new songs
            downloadSongs(playlists, newSongs);

            //rename the songs
            renameAllNewSongs(playlists, newSongs);
        }

        /// <summary>
        /// get the list of new videos from a playlist
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private static List<Video> getNewPlaylistVideos(List<PlaylistHandlerPackage> playlists)
        {
            //the urls in the playlist
            List<Video> playlistVideos = new List<Video>();

            //get the existing urls for the playlist
            List<string> existingUrls = getExistingSongsFromPlaylists(playlists);

            foreach (PlaylistHandlerPackage playlist in playlists)
            {
                //get all the urls from the playlist
                string baseUrl = "https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=25&playlistId=" + playlist.playlistID + "&key=" + Program.GOOGLE_API_KEY + "&page_token=";

                //go though each page to get all the results
                //each page shows 25 results, so we will go though every page
                bool nextPage = true;
                string pageToken = "";

                while (nextPage)
                {
                    //make the request and get the info
                    string response = getBodyOfBlanketGetRequest(baseUrl + pageToken);

                    //deserialize the page
                    //Console.Write(response + "\n---------------------------------------------\n");
                    JObject pageJObject = JObject.Parse(response);

                    //get the next page token
                    if (pageJObject.ContainsKey("nextPageToken"))
                    {
                        pageToken = pageJObject.SelectToken("nextPageToken").ToString();
                    }
                    else
                    {
                        //if there isnt one, this was the final page
                        nextPage = false;
                    }
                    //list of urls in the results
                    JArray urlsJArray = (JArray)pageJObject.SelectToken("items");

                    //for each video in the playlist of the 25 max results
                    foreach (JToken token in urlsJArray)
                    {
                        //add video to list
                        try
                        {
                            playlistVideos.Add(new Video(playlist.playlistID, token.SelectToken("snippet").SelectToken("resourceId").SelectToken("videoId").ToString(), TitleHandler.filterOriginalTitle(token.SelectToken("snippet").SelectToken("title").ToString()), token.SelectToken("snippet").SelectToken("videoOwnerChannelTitle").ToString(), token.SelectToken("snippet").SelectToken("publishedAt").ToString()));
                        }
                        catch (Exception e)
                        {
                            //most likely this video was taken down.
                            //Do Nothing
                        }
                    }
                }

                //remove duplicates
                List<Video> playlistVideosFinal = new List<Video>();

                foreach (Video video in playlistVideos)
                {
                    //if there isn't a duplicate
                    if (playlistVideosFinal.FirstOrDefault(c => c.urlID == video.urlID) == null)
                    {
                        //add it
                        playlistVideosFinal.Add(video);
                    }
                }

                //set the new version to the old name
                playlistVideos = playlistVideosFinal;

                //set the new list
                playlistVideosFinal = playlistVideos.ToList();
                List<string> existingUrlsFinal = existingUrls.ToList();

                //remove all the videos that are in the old videos list from the new one
                foreach (string url in existingUrls)
                {
                    foreach (Video video in playlistVideos)
                    {
                        if (video.urlID == url)
                        {
                            existingUrlsFinal.Remove(url);
                            playlistVideosFinal.Remove(video);
                        }
                    }
                }

                //make them current
                playlistVideos = playlistVideosFinal;
                existingUrls = existingUrlsFinal;

                //this means that there are videos that got removed from the playlist, so remove them from the database
                if (existingUrls.Count != 0)
                {
                    foreach (string url in existingUrls)
                    {
                        removeSongFromPlaylist(url);

                        Console.Write(@"Good thing you got this song : https://www.youtube.com/watch?v=" + url + ", because it was deleted from the playlist!");
                    }
                }
            }


            progress.progressStatus(3);

            //add the newly found videos to the playlist videos table
            foreach (Video video in playlistVideos)
            {
                AddVideoToPlaylist(video.playlistID, video.urlID);
            }

            progress.progressStatus();

            return playlistVideos;
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
        /// get list of existing songs from the database
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private static List<string> getExistingSongsFromPlaylists(List<PlaylistHandlerPackage> playlists)
        {
            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                List<string> urls = new List<string>();

                foreach (PlaylistHandlerPackage playlist in playlists)
                {
                    //create the command
                    SqliteCommand command = new SqliteCommand(@"SELECT * FROM 'PlaylistVideos' WHERE playlist_id == '" + playlist.playlistID + @"'", connection);

                    //execute the command
                    SqliteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        urls.Add(reader.GetString(1));
                    }

                    connection.Close();
                }

                return urls;
            }
        }

        /// <summary>
        /// remnoves a video from the playlist table
        /// </summary>
        /// <param name="urlID"></param>
        private static void removeSongFromPlaylist(string urlID)
        {
            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"DELETE FROM PlaylistVideos WHERE url == '" + urlID + @"'", connection);

                //execute the command
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        /// <summary>
        /// adds a video to the playlsit database table
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoURL"></param>
        private static void AddVideoToPlaylist(string playlistID, string videoURL)
        {
            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"INSERT INTO PlaylistVideos (playlist_id, url) VALUES ('" + playlistID + @"', '" + videoURL + @"')", connection);

                //execute the command
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static List<Song> convertVideosToSongs(List<Video> videos)
        {
            //list of new songs
            List<Song> newSongs = new List<Song>();

            TitleHandlerPackage currentTitle;

            //for converting string into datetime object
            CultureInfo provider = CultureInfo.InvariantCulture;

            //convert then into song objects
            foreach (Video video in videos)
            {
                //convert the title into title and artist
                currentTitle = TitleHandler.handleTitle(video.title);

                //have a list that finds the channel id and gives it a genre
                //TODO make it the playlist name
                newSongs.Add(new Song(video.urlID, video.playlistID, video.title, currentTitle.title, currentTitle.artist, video.channelTitle, "GENRE", DateTime.ParseExact(video.created, "M/d/yyyy h:mm:ss tt", provider)));
            }

            progress.progressStatus();

            return newSongs;
        }

        private static List<PlaylistHandlerPackage> getPlaylists()
        {
            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"SELECT * FROM 'PlaylistList'", connection);

                //execute the command
                SqliteDataReader reader = command.ExecuteReader();

                //get the list of videos
                List<PlaylistHandlerPackage> playlists = new List<PlaylistHandlerPackage>();

                PlaylistHandlerPackage currentPlaylist;

                while (reader.Read())
                {
                    playlists.Add(new PlaylistHandlerPackage(reader.GetString(0), reader.GetString(1)));
                }

                connection.Close();

                return playlists;
            }
        }

        /// <summary>
        /// add a playlist
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="playlistName"></param>
        public static void addNewPlaylist(string playlistID, string playlistName)
        {
            foreach(PlaylistHandlerPackage playlist in getPlaylists())
            {
                if (playlist.playlistID == playlistID)
                {
                    Console.WriteLine("Playlist already exists!");

                    return;
                }
            }

            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"INSERT INTO PlaylistList (playlist_id, title) VALUES ('" + playlistID + @"', '" + playlistName + @"')", connection);

                //execute the command
                command.ExecuteNonQuery();

                //create the directories for it
                System.IO.Directory.CreateDirectory(oldSongsPath + @"\" + playlistName);

                //create the directories for it
                System.IO.Directory.CreateDirectory(newSongsPath + @"\" + playlistName);

                connection.Close();
            }
        }

        /// <summary>
        /// get the google api key
        /// </summary>
        /// <returns></returns>
        public static string getApiKey()
        {
            string apiKey = "";

            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"SELECT * FROM 'Properties' WHERE property == 'Google API Key'", connection);

                //execute the command
                SqliteDataReader reader = command.ExecuteReader();

                //get the list of videos
                List<PlaylistHandlerPackage> playlists = new List<PlaylistHandlerPackage>();

                PlaylistHandlerPackage currentPlaylist;

                while (reader.Read())
                {
                    apiKey = reader.GetString(1);
                    break;
                }

                //if there are multiple api keys, something is really wrong
                if (reader.Read())
                {
                    Console.WriteLine("There are more than one google api key in the database, something has gone horribly wrong...");
                }

                connection.Close();

                return apiKey;
            }
        }
        /// <summary>
        /// change the api key for the youtube google api
        /// </summary>
        /// <param name="apiKey"></param>
        public static void changeAPIKey(string apiKey)
        {
            //read the database
            using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
            {
                //open the connection
                connection.Open();

                //create the command
                SqliteCommand command = new SqliteCommand(@"UPDATE Properties SET value = '" + apiKey + @"' WHERE property == 'Google API Key'", connection);

                //execute the command
                command.ExecuteNonQuery();

                //change the current api key in the heap
                Program.GOOGLE_API_KEY = apiKey;

                connection.Close();
            }
        }

        /// <summary>
        /// remove a playlist
        /// </summary>
        /// <param name="playlistName"></param>
        public static void removePlaylist(string playlistName)
        {
            foreach (PlaylistHandlerPackage playlist in getPlaylists())
            {
                if (playlist.playlistName == playlistName)
                {
                    //read the database
                    using (SqliteConnection connection = new SqliteConnection(@"DataSource=" + databasePath))
                    {
                        //open the connection
                        connection.Open();

                        //create the command
                        SqliteCommand command = new SqliteCommand(@"DELETE FROM PlaylistList WHERE title == '" + playlistName + @"'", connection);

                        //execute the command
                        command.ExecuteNonQuery();

                        //create the directories for it
                        System.IO.Directory.CreateDirectory(oldSongsPath + @"\" + playlistName);

                        //create the directories for it
                        System.IO.Directory.CreateDirectory(newSongsPath + @"\" + playlistName);

                        connection.Close();
                    }

                    return;
                }
                else
                {
                    Console.WriteLine("Playlist does not exist!");
                }
            }

            
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

        private static void downloadSongs(List<PlaylistHandlerPackage> playlists, List<Song> songs)
        {
            foreach(Song song in songs)
            {
                downloadYoutubeAudio(song.urlID, playlists.Find(c => c.playlistID == song.playlistID).playlistName, song.originalTitle);
            }
        }

        /// <summary>
        /// downlaod the youtube audio to a specified location within the working directory 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="videoID"></param>
        private static void downloadYoutubeAudio(string urlID, string playlistName, string originalTitle)
        {
            //set the path to run the command from
            ExecuteBashCommand("cd " + youtubeDLPath);

            //download the song
            //ExecuteBashCommand("youtube-dl --extract-audio --audio-format \"mp3\" " + @"-o " + newSongsPath + @"\" + playlistName + @"https://www.youtube.com/watch?v=" + urlID);
            ExecuteBashCommand("youtube-dl --extract-audio --audio-format \"mp3\" -o \"" + "%(newsong)s.%(ext)s\" " + @"https://www.youtube.com/watch?v=" + urlID);

            //move the song to the correct location
            System.IO.File.Move(youtubeDLPath + @"\" + "NA" + @".mp3", newSongsPath + @"\" + playlistName + @"\" + originalTitle + @".mp3");
        }

        /// <summary>
        /// moves all old songs to the folder "previous"
        /// </summary>
        /// <param name="location"></param>
        private static void moveOldSongs(List<PlaylistHandlerPackage> playlists)
        {
            //directory info
            System.IO.DirectoryInfo directoryInfo;

            //for each playlist
            foreach(PlaylistHandlerPackage playlist in playlists)
            {
                //get directory info of the old songs playlist path
                directoryInfo = new DirectoryInfo(newSongsPath + @"\" + playlist.playlistName);

                foreach(FileInfo file in directoryInfo.GetFiles())
                {
                    file.MoveTo(oldSongsPath + @"\" + playlist.playlistName + @"\" + file.Name);
                }
            }
        }

        /// <summary>
        /// rename all the newly download songs to their proper name
        /// </summary>
        /// <param name="playlists"></param>
        /// <param name="songs"></param>
        public static void renameAllNewSongs(List<PlaylistHandlerPackage> playlists, List<Song> songs)
        {
            foreach(Song song in songs)
            {
                TitleHandler.renameVideoToSong(song.originalTitle, newSongsPath + @"\" + playlists.Find(c => c.playlistID == song.playlistID).playlistName, song.title, song.artist, song.genre, song.created);
            }
        }

        /// <summary>
        /// run a bash command for windows
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void ExecuteBashCommand(string command)
        {
            Console.WriteLine(command);

            // according to: https://stackoverflow.com/a/15262019/637142
            // thans to this we will pass everything as one command
            command = command.Replace("\"", "\"\"");

            /*var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();*/
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(youtubeDLPath);
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C" + " cd " + youtubeDLPath.Replace("\"", "\"\"") + " & " + command;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
        /// <summary>
        /// make a basic get request with no headers
        /// </summary>
        /// <param name="url"></param>
        /// <returns>body of the message</returns>
        private static string getBodyOfBlanketGetRequest(string url)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    HttpResponseMessage response = httpClient.SendAsync(request).Result;

                    string body = response.Content.ReadAsStringAsync().Result;

                    return body;
                }
            }
        }
    }

    /// <summary>
    /// has the information for the playlist itself
    /// </summary>
    public class PlaylistHandlerPackage
    {
        /// <summary>
        /// playlist id
        /// </summary>
        public string playlistID;

        /// <summary>
        /// name of the playlist
        /// </summary>
        public string playlistName;

        public PlaylistHandlerPackage(string playlistID, string playlistName)
        {
            this.playlistID = playlistID;
            this.playlistName = playlistName;
        }
    }
}

//handle spaces in playlist name in args