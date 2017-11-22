using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    public enum NBCLogVerbosityLevel
    {
        off,
        error,
        warning,
        info,
        verbose
    }

    /*
     * Exceptions
     */

    public class NBCIncorrectVersionException : Exception
    {
        public NBCIncorrectVersionException(Version versionGiven) : base(
            String.Format("Version expected: {0}. Version given: {1}", NBCAssembler.CurrentVersion, versionGiven)
            )
        { }
    }
    public class NBCNoArchGivenException : Exception
    {
        public NBCNoArchGivenException() : base("No architecture was given")
        { }
    }

    public class NBCNoOSByteGivenException : Exception
    {
        public NBCNoOSByteGivenException() : base("No OS Byte was given")
        { }
    }

    public class NBCIncorrectNumberOfArgumentsException : Exception
    {
        public NBCIncorrectNumberOfArgumentsException(int numberOfArgumentsGiven, int numberOfArgumentsExpected) : base(
            String.Format("Incorrect number of arguments (Expected: {0} Given: {1}", numberOfArgumentsExpected, numberOfArgumentsGiven)
        )
        {}
    }

    public class NBCAssembler
    {
        public NBCArchitecture Architecture { get; set; }
        public List<NBCCommand> commands = new List<NBCCommand>();
        List<string> lines = new List<string>();
        NBCLogVerbosityLevel verbosityLevel = NBCLogVerbosityLevel.error;
        public List<string> Log { get; }
        public bool OutputLogToConsole { get; set; }
        public List<byte> Program { get; }
        static public Version CurrentVersion { get { return new Version("1.0.0") ; } }
        public NBCAssembler(NBCArchitecture arch, string str)
        {
            Architecture = arch;
            //Split the string into lines, trim any lines, and set them all to lowercase
            foreach (string line in System.Text.RegularExpressions.Regex.Split(str, "\r\n|\r|\n"))
            {
                lines.Add(line.Trim().ToLower());
            }
        }

        public void SetLogVerbosityLevel(NBCLogVerbosityLevel level)
        {
            verbosityLevel = level;
        }

        private void addToLog(NBCLogVerbosityLevel verbosity, string str)
        {
            if (verbosityLevel <= verbosity)
            {
                Log.Add(str);
                if (OutputLogToConsole)
                    Console.WriteLine(str);
            }
        }

        private struct DefineStructure
        {
            public string Name;
            public string ReplaceWith;
        }
        private struct LabelStructure
        {
            public string Name;
            public UInt16 Address;
        }
        private struct NextInQueueStructure
        {
            public NBCCommand Command;
            public UInt16 Address;
        }

        public void AssembleProgram()
        {
            bool archGiven = false;
            bool versionGiven = false;
            bool osByteGiven = false;
            byte osByte = 0;
            List<string> savedPages = new List<string>();
            List<DefineStructure> defines = new List<DefineStructure>();
            List<LabelStructure> labels = new List<LabelStructure>();
            //Pass 1: Preprocessors
            addToLog(NBCLogVerbosityLevel.info, "Starting Pass 1: Preprocessors");
            foreach (string line in lines)
            {
                if (line.StartsWith("."))
                {
                    addToLog(NBCLogVerbosityLevel.verbose, "Preprocessor: " + line);
                    if (line.StartsWith(".nbcasm"))
                    {
                        if (line.Split(" ")[1] != CurrentVersion.ToString())
                        {
                            throw new NBCIncorrectVersionException(new Version(line.Split(" ")[1]));
                        }
                        versionGiven = true;
                    }
                    if (line.StartsWith(".targetarch"))
                    {
                        archGiven = true;
                        Architecture = NBCArchitecture.getDefaultArchitecture(Byte.Parse(line.Split(" ")[1]));
                    }
                    if (line.StartsWith(".osbyte"))
                    {
                        osByteGiven = true;
                        osByte = Byte.Parse(line.Split(" ")[1]);
                    }
                    if (line.StartsWith(".define"))
                    {
                        defines.Add(new DefineStructure {
                            Name = line.Split(" ")[1],
                            ReplaceWith = line.Split(" ")[2]
                        });
                    }
                    if (line.StartsWith(".savepage"))
                    {
                        savedPages.Add(line.Split(" ")[1]);
                    }
                }
            }
            addToLog(NBCLogVerbosityLevel.verbose, "Pass 1: Done!");
            addToLog(NBCLogVerbosityLevel.warning, "Building program header");
            Program.Add(Architecture.ID);
            Program.Add(osByte);
            Program.Add((Byte) savedPages.Count);
            //Add fillers for the pages
            for (int i = 0; i<savedPages.Count; i++)
            {
                Program.Add(0);
            }
            //Pass 2: Main commands and caching labels for later use
            addToLog(NBCLogVerbosityLevel.verbose, "Starting Pass 2: Main commands and caching labels for later use");
            foreach(string line in lines)
            {
                if (line.StartsWith(':'))
                {
                    labels.Add(new LabelStructure
                    {
                        Name = line,
                        Address = (UInt16) Program.Count
                    });
                    continue;
                }
                if (NBCCommand.TryParse(line))
                {
                    //Now that we know that it is a syntatically correct command,
                    //we can move on to checking what the command actually is
                    NBCCommand tmpCommand = NBCCommand.Parse(line);
                    //NOP
                    //  Opcode 0x00
                    //  0 args
                    if (tmpCommand.Command == "nop")
                    {
                        if (tmpCommand.Arguments.Count != 0)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 0);
                        }
                        //Command
                        Program.Add(0x00);
                        //Arg header byte 0
                        Program.Add(0x00);
                        //Arg header byte 1
                        Program.Add(0x00);
                        continue;
                    }
                    //HLT
                    //  Opcode 0x01
                    //  0 args
                    if (tmpCommand.Command == "hlt")
                    {
                        if (tmpCommand.Arguments.Count != 0)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 0);
                        }
                        //Command
                        Program.Add(0x01);
                        //Arg header byte 0
                        Program.Add(0x00);
                        //Arg header byte 1
                        Program.Add(0x00);
                    }
                    //CHP
                    //  Opcode 0x10
                    //  1 Arg
                    if (tmpCommand.Command == "chp")
                    {
                        if (tmpCommand.Arguments.Count != 1)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 1);
                        }
                        //Now we need to build the command header
                        //TODO: Finish this
                    }
                }
            }

        }
    }
}
