using System.Collections.Generic;
using System;

namespace NBCAssembler
{
    public class NBCCommand
    {
        public List<NBCArg> Arguments { get; set; }
        public string Command { get; set; }


        /// <summary>
        /// Attempt to parse an argument
        /// </summary>
        /// <param name="str">The string to attempt to parse</param>
        /// <returns>true if possible to parse, false if not</returns>
        public static bool TryParse(string str)
        {
            if (str == "")
                return false;
            //Split the whole thing into words to check to be parsed
            List<string> words = new List<string>();
            words.AddRange(str.Split(" "));
            //We remove the command, since that's a given
            words.RemoveAt(0);
            foreach (string word in words)
            {
                if (!NBCArg.TryParse(word))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Parse a string
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>NBCCommand from the string passed</returns>
        public static NBCCommand Parse(string str)
        {
            if (str == "")
            {
                throw new System.FormatException("String is blank");
            }
            //This is what we will use to return a value
            NBCCommand tmp = new NBCCommand();
            //We split the whole string into seprate words to parse
            List<string> words = new List<string>();
            words.AddRange(str.Split(" "));
            //We extract the command from those words. The command should be the first thing on the line
            tmp.Command = words[0];
            //We remove it so it doesn't get parsed as an argument
            words.RemoveAt(0);
            //And now we parse every word after as arguments
            foreach (string word in words)
            {
                if (NBCArg.TryParse(word))
                {
                    tmp.Arguments.Add(NBCArg.Parse(word));
                }
                else
                {
                    throw new System.FormatException("String is in the incorrect format");
                }
            }
            return tmp;
        }
        //TODO: Finish this
        public UInt16 GetArgumentHeader()
        {
            UInt16 returnLong = 0;
            int i = 0;
            while (i<Arguments.Count)
            {

                i++;
            }
        } 
    }
}
