using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intellivision.PSG
{
    public class AY_3_891x
    {
        public void Write(int address, UInt16 value)
        {
            // you might need to change the memory map. it's treating writing to memory position 0x01F0 as writing to 0 here... might be easier to leave it as the system memory position instead of the actual position in the chip.
        }

        public UInt16 Read(int address)
        {
            return 0;
        }
    }
}
