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

    public class NBCAssembler
    {
        public NBCArchitecture Architecture { get; set; }
        public List<NBCCommand> commands = new List<NBCCommand>();
        string[] lines = null;
        NBCLogVerbosityLevel verbosityLevel = NBCLogVerbosityLevel.error;
        public List<string> Log { get; }
        public bool OutputLogToConsole { get; set; }
        public List<byte> Program { get; }
        static public Version CurrentVersion { get { return new Version("1.0.0") ; } }
        public NBCAssembler(NBCArchitecture arch, string[] str)
        {
            Architecture = arch;
            lines = str;
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

        public void AssembleProgram()
        {
            //Pass 1: Preprocessors
            addToLog(NBCLogVerbosityLevel.info, "Starting Pass 1: Preprocessors");
            bool archGiven = false;
            bool versionGiven = false;
            bool osByteGiven = false;
            byte osByte = 0;
            List<DefineStructure> defines = new List<DefineStructure>();
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

                }
            }
        }
    }
}
