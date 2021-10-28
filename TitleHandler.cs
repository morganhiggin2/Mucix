using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mucix
{
    class TitleHandler
    {
        /// <summary>
        /// reformats the song title so that it gets the artist and the name of the song
        /// </summary>
        /// <param name="title">entire title of the song</param>
        /// <returns></returns>
        public static TitleHandlerPackage handleTitle(string title)
        {
            //the song to return
            TitleHandlerPackage song = new TitleHandlerPackage();

            //if the match is a success
            bool success = true;

            //information about the match
            Match match = null;

            //get the artist
            match = Regex.Match(title, @"[\w &.]*");

            if (match.Success)
            {
                //get the artist of the song
                song.artist = match.Value;

                //get the next occurance, which should be the title
                match = match.NextMatch();

                if (match.Success)
                {
                    //get the title of the song
                    song.title = match.NextMatch().Value;
                }
                else
                {
                    //match not found
                    success = false;
                }
            }
            else
            {
                //match not found
                success = false;
            }

            //if success, return song, else, return null
            if (success)
            {
                //remove space at begining of title if there is any
                if (song.title.IndexOf(' ') == 0)
                {
                    song.title = song.title.Substring(1);
                }

                //if space at end of artist, remove it 
                if (song.title.Length > 2 && song.title.Substring(song.title.Length - 2) == " ")
                {
                    song.title = song.title.Substring(0, song.title.Length - 1);
                }

                return song;
            }
            else
            {
                //return a standard unsuccessful titlehandlerpackage
                song.title = title;
                song.artist = "";

                return song;
            }
        }

        /// <summary>
        /// rename video to song
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pathToFile"></param>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <param name="genere"></param>
        /// <param name="created"></param>
        public static void renameVideoToSong(string fileName, string pathToFile, string title, string artist, string genere, DateTime created)
        {
            //new file name
            string newFileName = title + " - " + artist + ".mp3";

            //change the filename
            Console.WriteLine(pathToFile + @"\" + newFileName);
            System.IO.File.Move(pathToFile + @"\" + fileName + ".mp3", pathToFile + @"\" + newFileName);

            //change the properties of the mp3
            TagLib.File file = TagLib.File.Create(pathToFile + @"\" + newFileName);
            file.Tag.AlbumArtists = new string[] { artist };
            file.Tag.Year = (uint)created.Year;
            file.Tag.Genres = new string[] { genere };
            file.Tag.Title = title;
            file.Save();
        }

        //filter the original title
        public static string filterOriginalTitle(string originalTitle)
        {
            string newTitle = originalTitle;

            newTitle = newTitle.Replace("|", "");
            newTitle = newTitle.Replace("%", "");
            //newTitle.Replace("&", "");
            newTitle = newTitle.Replace("@", "");
            newTitle = newTitle.Replace("\\", "");
            newTitle = newTitle.Replace("/", "");
            newTitle = newTitle.Replace("   ", " ");
            newTitle = newTitle.Replace("  ", " ");

            return newTitle;
        }

        /// <summary>
        /// compiles the args with a space in between them
        /// </summary>
        /// <param name="args"></param>
        /// <param name="startIndex">start index</param>
        /// <returns></returns>
        public static string compileArgs(List<string> args, int startIndex = 0)
        {
            //the total string 
            string totalString = "";

            //add each of the args to the string
            for(int i = startIndex; i < args.Count - 1; i++)
            {
                if (i == args.Count - startIndex - 1)
                {
                    totalString += args[i];
                } 
                else
                {
                    totalString += args[i] + " ";
                }
            }

            totalString += args[args.Count - 1];

            return totalString;
        }
    }

    public class TitleHandlerPackage
    {
        /// <summary>
        /// the artist of the song
        /// </summary>
        public string artist;

        /// <summary>
        /// the title of the song
        /// </summary>
        public string title;
    }
}
