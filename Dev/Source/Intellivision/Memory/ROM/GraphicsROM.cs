using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intellivision.Memory.ROM
{
    public class GraphicsROM
    {
        private UInt16[] _memory;

        public GraphicsROM()
        {
            _memory = new UInt16[2048];
            // may or may not need to manually fill this in for this data:
            // http://wiki.intellivision.us/index.php?title=Graphics_ROM
        }

        public UInt16 Read(int address)
        {
            switch (address) // this is just to help debug and get started
            {
                case 0x21: Console.WriteLine("GOT THE LETTER 'A' FROM GROM"); break;
                case 0x23: Console.WriteLine("GOT THE LETTER 'C' FROM GROM"); break;
                case 0x24: Console.WriteLine("GOT THE LETTER 'D' FROM GROM"); break;
                case 0x25: Console.WriteLine("GOT THE LETTER 'D' FROM GROM"); break;
            }
            return _memory[address];
        }

        public void Write(int address, UInt16 value)
        {
            _memory[address] = value;
        }
    }
}
