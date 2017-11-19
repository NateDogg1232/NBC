using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    public class NBCArchitecture
    {
        public string Name { get; set; }
        public byte ID { get; set; }
        public List<NBCPort> Ports { get; set; }

        public NBCArchitecture()
        {
            //Set up standard/semi-standard ports
            NBCPort tmpPort = new NBCPort
            {
                //Port 0: TTY Character out
                PortID = 0,
                InAllowed = false
            };
            Ports.Add(tmpPort);
            tmpPort = new NBCPort
            {
                //Port 1: Character in
                PortID = 1,
                OutAllowed = false
            };
            Ports.Add(tmpPort);
            tmpPort = new NBCPort
            {
                //Port 2: Get Current Architecture
                PortID = 2
            };
            Ports.Add(tmpPort);
            tmpPort = new NBCPort
            {
                //Port 10h: Cursor Control
                PortID = 0x10
            };
            Ports.Add(tmpPort);
        }

        static public NBCArchitecture getDefaultArchitecture(byte id)
        {
            NBCArchitecture tmp = new NBCArchitecture
            {
                ID = id
            };
            switch (id)
            {
                //Universal
                case (0):
                    return tmp;
                //PC
                case (1):
                    return tmp;
                //3DS
                case (2):
                    return tmp;
                //Unknown
                default:
                    return tmp;
            }
        }

    }
}
