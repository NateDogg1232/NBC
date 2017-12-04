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
        { }
    }

    public class NBCIncorrectArgumentTypeException : Exception
    {
        public NBCIncorrectArgumentTypeException(NBCArgType argType) : base(String.Format("Incorrect argument type: {0}", argType.ToString()))
        { }
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


        private void addLongToProgram(UInt16 add)
        {
            Program.Add((byte) (add & 0xFF));
            Program.Add((byte) ((add & 0xFF00) >> 0x100));
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
                        Program.Add(0x10);
                        addLongToProgram(tmpCommand.GetArgumentHeader());
                        //And now we add the arguments
                        foreach (NBCArg arg in tmpCommand.Arguments)
                        {
                            if (arg.Long)
                            {
                                addLongToProgram(arg.Value);
                            }
                            else
                            {
                                Program.Add((byte) arg.Value);
                            }
                        }
                    }
                    //MOV
                    //  Opcode 0x11
                    //  2 Args
                    if (tmpCommand.Command == "mov")
                    {
                        if (tmpCommand.Arguments.Count != 2)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 2);
                        }
                        //Now we need to build the command header
                        Program.Add(0x11);
                        addLongToProgram(tmpCommand.GetArgumentHeader());
                        //And now we add the arguments
                        //We start with argument 1, which has no type constrictions
                        if (tmpCommand.Arguments[0].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[0].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[0].Value & 0xFF));
                        }
                        //Then we go with argument two, which needs to be an address
                        if ((tmpCommand.Arguments[1].Type == NBCArgType.Constant) || (tmpCommand.Arguments[1].Type == NBCArgType.Label))
                        {
                            throw new NBCIncorrectArgumentTypeException(tmpCommand.Arguments[1].Type);
                        }
                        //Since we know it's an address, now we can just add it to the program
                        if (tmpCommand.Arguments[1].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[1].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[1].Value & 0xFF));
                        }
                    }
                    //MOL
                    //  Opcode 0x12
                    //  3 args
                    if (tmpCommand.Command == "mol")
                    {
                        if (tmpCommand.Arguments.Count != 3)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 3);
                        }
                        //Now we build the command header
                        Program.Add(0x12);
                        addLongToProgram(tmpCommand.GetArgumentHeader());

                        if (tmpCommand.Arguments[0].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[0].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[0].Value & 0xFF));
                        }
                        if ((tmpCommand.Arguments[1].Type == NBCArgType.Constant) || (tmpCommand.Arguments[1].Type == NBCArgType.Label))
                        {
                            throw new NBCIncorrectArgumentTypeException(tmpCommand.Arguments[1].Type);
                        }
                        if (tmpCommand.Arguments[1].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[1].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[1].Value & 0xFF));
                        }
                        if (tmpCommand.Arguments[2].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[2].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[2].Value & 0xFF));
                        }
                    }
                    //INC
                    //  Opcode: 0x20
                    //  Args: 1
                    if (tmpCommand.Command == "inc")
                    {
                        if (tmpCommand.Arguments.Count != 1)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 1);
                        }
                        if (tmpCommand.Arguments[0].Type != NBCArgType.Address || tmpCommand.Arguments[0].Type != NBCArgType.Indirect)
                        {
                            throw new NBCIncorrectArgumentTypeException(tmpCommand.Arguments[0].Type);
                        }
                        //Now we add the command header
                        Program.Add(0x20);
                        addLongToProgram(tmpCommand.GetArgumentHeader());
                        //And now we add the arguments
                        if (tmpCommand.Arguments[0].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[0].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[0].Value & 0xFF));
                        }
                    }
                    //DEC
                    //  Opcode: 0x21
                    //  Args: 1
                    if (tmpCommand.Command == "dec")
                    {
                        if (tmpCommand.Arguments.Count != 1)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 1);
                        }
                        if (tmpCommand.Arguments[0].Type != NBCArgType.Address || tmpCommand.Arguments[0].Type != NBCArgType.Indirect)
                        {
                            throw new NBCIncorrectArgumentTypeException(tmpCommand.Arguments[0].Type);
                        }
                        //Now we add the command header
                        Program.Add(0x21);
                        addLongToProgram(tmpCommand.GetArgumentHeader());
                        //And now we add the arguments
                        if (tmpCommand.Arguments[0].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[0].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[0].Value & 0xFF));
                        }
                    }
                    //ADD
                    //  Opcode: 0x22
                    //  Args: 3
                    if (tmpCommand.Command == "add")
                    {
                        if (tmpCommand.Arguments.Count != 3)
                        {
                            throw new NBCIncorrectNumberOfArgumentsException(tmpCommand.Arguments.Count, 3);
                        }
                        if (tmpCommand.Arguments[2].Type != NBCArgType.Address || tmpCommand.Arguments[2].Type != NBCArgType.Indirect)
                        {
                            throw new NBCIncorrectArgumentTypeException(tmpCommand.Arguments[2].Type);
                        }
                        //Now we add the command header
                        Program.Add(0x22);
                        addLongToProgram(tmpCommand.GetArgumentHeader());
                        if (tmpCommand.Arguments[0].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[0].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[0].Value & 0xFF));
                        }
                        if (tmpCommand.Arguments[1].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[1].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[1].Value & 0xFF));
                        }
                        if (tmpCommand.Arguments[2].Long)
                        {
                            addLongToProgram(tmpCommand.Arguments[2].Value);
                        }
                        else
                        {
                            Program.Add((byte) (tmpCommand.Arguments[2].Value & 0xFF));
                        }
                    }
                    //TODO: Finish the commands                    
                }
            }

        }
    }
}
