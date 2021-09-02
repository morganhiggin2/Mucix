using Mucix;
using System;

class Program
{
    static void Main(string[] args)
    {
        //create the command line listener
        //listen for incoming commands on a loop
        //process them though the TerminalListener

        TitleHandlerPackage song = TitleHandler.handleTitle("Jamie Berry ft. Octavia Rose - Make Me Believe (Electro Swing)");

        Console.WriteLine(song.artist);

        Console.WriteLine(song.title);

        TerminalListener terminal = new TerminalListener();
    }
}

//use bash commands to execute the youtubedl package
//make the api calls to get the list of links and store them in a json file 

