using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intellivision.Memory.RAM
{
    public class GraphicsRAM
    {
        private UInt16[] _memory;

        public GraphicsRAM()
        {
            _memory = new UInt16[512];
        }

        public UInt16 Read(int address)
        {
            return _memory[address];
        }

        public void Write(int address, UInt16 value)
        {
            _memory[address] = value;
        }
    }
}
