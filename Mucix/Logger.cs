using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mucix
{
    class Logger
    {


        /// <summary>
        /// remove the most current console line
        /// </summary>
        public static void ClearCurrentConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write("");
        }
    }
    public class ProgressBar
    {
        private int progress = 0;

        /// <summary>
        /// total length of the progress bar
        /// </summary>
        int length;
        public ProgressBar(int length)
        {
            this.length = length;

            //print 0%
            progressStatus(0);
        }

        /// <summary>
        /// progress another level
        /// </summary>
        /// <returns>if you are at 100%</returns>
        public bool progressStatus(int amount = 1)
        {
            //if we are complete already, stop
            if (progress == length)
            {
                return true;
            }


            //if they are going to over step, just set it to 100%
            if (progress + amount > length)
            {
                progress = length;
            }
            else
            {
                //progress a step
                progress += amount;
            }

            //if we have printed progress before, erase it
            if (progress != 0)
            {
                Logger.ClearCurrentConsoleLine();
            }

            //log progress
            string printStatement = "";

            printStatement += "[";

            for(int i = 0; i < progress; i++)
            {
                printStatement += ".";
            }

            for(int i = 0; i < length - progress; i++)
            {
                printStatement += " ";
            }

            printStatement += "] ";
            printStatement += (progress * 100 / length).ToString();
            printStatement += "% complete";

            Console.WriteLine(printStatement);

            //if we are complete
            if (progress >= length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
