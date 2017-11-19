using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    public class NBCArchitecture
    {
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
            //Port 10h: Cursor Control
            tmpPort = new NBCPort
            {
                PortID = 0x10
            };
            Ports.Add(tmpPort);
            
        }

        public string Name { get; set; }
        public byte ID { get; set; }
        public List<NBCPort> Ports { get; set; }
    }
}
