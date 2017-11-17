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
    public class NBCAssembler
    {
        NBCArchitecture Architecture { get; set; }
        List<NBCCommand> commands = new List<NBCCommand>();
        string[] lines = null;
        NBCLogVerbosityLevel verbosityLevel = NBCLogVerbosityLevel.error;
        List<string> Log { get; }
        List<byte> Program { get; }
        public NBCAssembler(NBCArchitecture arch, string[] str)
        {
            Architecture = arch;
            lines = str;
        }

        public void SetLogVerbosityLevel(NBCLogVerbosityLevel level)
        {
            verbosityLevel = level;
        }

        public byte[] AssembleProgram()
        {
            if (verbosityLevel == NBCLogVerbosityLevel.info)
                Log.Add("Beginning Pass 1");
            foreach (string line in lines)
            {

            }
        }
    }
}
