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
        public NBCNoArchGivenException(string message) : base(message)
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

        public byte[] AssembleProgram()
        {
            //Pass 1: Preprocessors
            addToLog(NBCLogVerbosityLevel.info, "Starting Pass 1: Preprocessors");
            bool archGiven = false;
            bool versionGiven = false;
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
                }
            }
        }
    }
}
