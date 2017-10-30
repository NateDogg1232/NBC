using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NBCAsm
{
    class Program
    {
        static string fileName = "";
        static void Main(string[] args)
        {
            if (args.GetLength(0)!=0)
            {
                Debug.WriteLine("There are args");
                foreach (string arg in args)
                {
                    //Help argument
                    if (arg == "-help" || arg == "-h")
                    {
                        Debug.WriteLine("ArgumentFound: " + arg);
                        ShowHelp();
                    }

                    if (!arg.StartsWith("-"))
                    {
                        fileName = arg;
                    }
                }
            }
            else
            {
                Debug.WriteLine("There are no args");
            }
            Console.ReadLine();
        }
        
        static void ShowHelp()
        {
            Console.WriteLine("NBC Assembler Ver {0}", "b1.0");
            Console.WriteLine("Archeticture: PC-.NETCore");
            Console.WriteLine();
            Console.WriteLine("Syntax: NBCAsm <file> [-h|-help]");
            Console.ReadLine();
        }
    }
}
