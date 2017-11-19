using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    public class NBCPort
    {
        public NBCPort()
        {
            PortID = 0;
            InAllowed = true;
            OutAllowed = true;
        }

        public byte PortID { get; set; }
        public bool InAllowed { get; set; }
        public bool OutAllowed { get; set; }
        
    }
}
