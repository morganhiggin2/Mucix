using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mucix
{
    class TerminalListener
    {
        private bool running;

        public TerminalListener()
        {
            running = true;

            //start the terminal listener
            Start();
        }

        private async void Start()
        {

            //input from the console
            string input = "";

            //get the console text ready
            Console.WriteLine("Welcome to Mucix! A program to download videos off of your custom youtube playlists to this computer so they can easily be put on your phone! To get started, type \"about\", and for help, type \"help\".");
            Console.Write(">> ");

            while (running)
            {
                //get the next console input
                input = Console.ReadLine();

                if (!excecuteCommand(input))
                {
                    Console.WriteLine("Invalid Command");
                }
            }
        }

        /// <summary>
        /// given a command from the user, execute it
        /// </summary>
        /// <param name="command"></param>
        /// <returns>if the command successfully executed</returns>
        public bool excecuteCommand(string command)
        {
            Console.WriteLine();
            //break down the command into arguments
            //process each argument, starting with a swtich thought the first

            //COMMANDS
            //create new playlist
            //delete playlist
            //download all
            //download PLAYLIST ID
            //    -if new playlist id, create new playlist

            //get the args
            List<string> args = command.Split(' ').ToList();
            
            if (args.Count >= 1)
            {
                switch(args[0])
                {
                    case ("exit"):
                        {
                            running = false;

                            System.Environment.Exit(0);

                            break;
                        }
                    case ("help") :
                        {
                            Console.WriteLine("about                                      - about the program and how to use it");
                            Console.WriteLine("exit                                       - exit the program");
                            Console.WriteLine("run                                        - runs the main program");
                            Console.WriteLine("add playlist {playlist_id} {playlist_name} - adds a playlist");
                            Console.WriteLine("remove playlist {playlist_name}            - removes a playlist");
                            Console.WriteLine("update key {google_api_key}                - update the google api key");

                            break;
                        }
                    case ("about"): 
                        {
                            Console.WriteLine("When you run this program, it will put all the new songs you added into the playlist folders in the home directory of this program, and move all the old ones to the old songs folder, so all you have to do it drag all the contents form those folders and you will have the newest songs, but yet still have the old ones. Go ahead, try it out! \n\nTo get started, first get a youtube api key. To do this, make sure you are signed in to the youtube account you want to use, then go this this page:\n\nhttps://developers.google.com/youtube/registering_an_application \n\nand for the second step, follow the part that says \"API keys\". After creating the credentials, copy the api key and update the api key though the terminal here. \n\nNext, add a playlist using the playlist id from the url of the playlist itself (just click on the playlist and use the url in the search bar) and the playlist title  (what you want the playlist to be called). To get the playlist id, simply copy the long string of random numbers and letters after the attribute \"list\" in the url. \n\nNext, run the program and watch the magic happen.");

                            break;
                        }
                    case ("run"):
                        {
                            APIRequestHandler.getNewPlaylistSongs();

                            Console.WriteLine("complete");

                            break;
                        }
                    case ("add"):
                        {
                            if (args.Count >= 2)
                            {
                                switch(args[1])
                                {
                                    case ("playlist"):
                                        {
                                            if (args.Count >= 4)
                                            {
                                                string restOfArgsCompiled = TitleHandler.compileArgs(args, 3);

                                                APIRequestHandler.addNewPlaylist(args[2], restOfArgsCompiled);

                                                Console.WriteLine("complete");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Need to have the playlist id and the title");
                                            }

                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Not a Valid Command");

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Have to specifiy what you want to add");
                            }

                            break;
                        }
                    case ("remove"):
                        {
                            if (args.Count >= 2)
                            {
                                switch (args[1])
                                {
                                    case ("playlist"):
                                        {
                                            if (args.Count >= 3)
                                            {
                                                string restOfArgsCompiled = TitleHandler.compileArgs(args, 2);

                                                APIRequestHandler.removePlaylist(restOfArgsCompiled);

                                                Console.WriteLine("complete");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Need to have the playlist name");
                                            }

                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Not a Valid Command");

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Have to specify what you want to add");
                            }

                            break;
                        }
                    case ("update"):
                        {
                            if (args.Count >= 2)
                            {
                                switch(args[1])
                                {
                                    case ("key"):
                                        {
                                            if (args.Count >= 3)
                                            {
                                                APIRequestHandler.changeAPIKey(args[2]);

                                                Console.WriteLine("complete");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Need to have a Google Api Key");
                                            }

                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Not a Valid Command");

                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Have to specify what to update");
                            }

                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Not a command");

                            break;
                        }
                }
            }

            Console.Write("\n>> ");

            return true;
        }
    }
}
