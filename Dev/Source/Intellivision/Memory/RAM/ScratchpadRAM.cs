using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intellivision.Memory.RAM
{
    public class ScratchpadRAM
    {
        private byte[] _memory;

        public ScratchpadRAM()
        {
            _memory = new byte[256];
        }

        public UInt16 Read(int address)
        {
            return _memory[address];
        }

        public void Write(int address, UInt16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            _memory[address] = bytes[0];
        }
    }
}
