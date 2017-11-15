using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{

    public class NBCAssembler
    {
        NBCArchitecture Architecture { get; set; }
        List<NBCCommand> commands = new List<NBCCommand>();

        public NBCAssembler()
        {
        }
    }
}
