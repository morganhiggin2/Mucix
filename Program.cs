using Mucix;
using System;
using Microsoft.Data.Sqlite;

class Program
{
    public static string GOOGLE_API_KEY = "AIzaSyAFoTzNBinvEqBsdN4legzysbMgWW3Iwzk";

    static void Main(string[] args)
    {
        //call init
        APIRequestHandler.init();

        //start the interactive terminal interface
        TerminalListener terminal = new TerminalListener();
    }
}

