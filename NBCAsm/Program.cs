using System;

namespace NBCAsm
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.GetUpperBound(0)!=0)
            {
                foreach (string arg in args)
                {
                    //Help argument
                    if (arg == "-help" || arg == "-h")
                    {
                        Console.WriteLine("NBC Assembler Ver {0}", System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
                        Console.ReadLine();
                    }
                }
            }
        }
    }
}
