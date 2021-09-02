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
                            Console.WriteLine("exit  - exit the program");

                            break;
                        }
                    case ("run"):
                        {
                            APIRequestHandler.getNewPlaylistSongs();

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
                                                APIRequestHandler.addNewPlaylist(args[2], args[3]);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Need to have the playlist id and the title");
                                            }

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
                    default:
                        {
                            Console.WriteLine("Not a command");

                            break;
                        }
                }
            }

            Console.WriteLine("done");

            return true;
        }
    }
}
