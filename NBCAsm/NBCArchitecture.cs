using System;
using System.Collections.Generic;
using System.Text;

namespace NBCAssembler
{
    public class NBCArchitecture
    {
        public NBCArchitecture()
        {
            
        }

        public string Name { get; set; }
        public byte ID { get; set; }
        public List<NBCPort> Ports { get; set; }
    }
}
