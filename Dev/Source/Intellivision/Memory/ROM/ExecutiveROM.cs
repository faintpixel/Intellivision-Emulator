using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intellivision.Memory.ROM
{
    public class ExecutiveROM  
    {
        private UInt16[] _memory; // technically the data is only 10-bits, so will be treating everything outside of that as 0
        private UInt16 _bitMask = 0x3FF; // 0000001111111111 - Everything that falls into a 1 space is going to be ignored since the memory is 10 bits wide.

        public ExecutiveROM()
        {
            _memory = new UInt16[4096];
        }

        public UInt16 Read(int address)
        {
            return (UInt16)(_memory[address] & _bitMask);
        }

        public void Write(int address, UInt16 value)
        {
            _memory[address] = (UInt16)(value & _bitMask);
        }
    }
}
