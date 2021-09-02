using Mucix;
using System;
using Microsoft.Data.Sqlite;

class Program
{
    public const string GOOGLE_API_KEY = "AIzaSyAFoTzNBinvEqBsdN4legzysbMgWW3Iwzk";

    static void Main(string[] args)
    {
        //create the command line listener
        //listen for incoming commands on a loop
        //process them though the TerminalListener

        //APIRequestHandler.getNewPlaylistSongs("PL5MlDErkUccCFHvVvb45pJRzxnjBOjrzp");

        //call init
        APIRequestHandler.init();

        TerminalListener terminal = new TerminalListener();
    }
}

//use bash commands to execute the youtubedl package
//make the api calls to get the list of links and store them in a json file 

