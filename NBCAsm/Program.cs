using System;
using System.Collections.Generic;
using System.Diagnostics;
using NBCAssembler;

namespace NBCAsm
{

    //TODO: Fix argument parser

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
                        return;
                    }

                    if (arg == "-o")

                    if (!arg.StartsWith("-"))
                    {
                        fileName = arg;
                    }
                }
            }
            else
            {
                Debug.WriteLine("There are no args");
                ShowHelp();
            }
            if (fileName == "")
            {
                Console.WriteLine("No file given!");
                return;
            }
        }
        
        static void ShowHelp()
        {
            Console.WriteLine("NBC Assembler Ver {0}", "b1.0");
            Console.WriteLine("Archeticture: PC-.NETCore");
            Console.WriteLine();
            Console.WriteLine("Syntax: NBCAsm <file> [-h|-help] [-o output]");
            Console.WriteLine();
            Console.WriteLine("All arguments are case sensitive!");
            Console.WriteLine();
            Console.WriteLine("Arguments: ");
            Console.WriteLine("\t-h/-help: Show this message");
            Console.WriteLine("\tfile: File to assemble");
            Console.WriteLine("\t-o output: Output the result to the file specified. By default this file is the same as the input except with the extension .nbc");
            Console.WriteLine();
        }
    }
}
