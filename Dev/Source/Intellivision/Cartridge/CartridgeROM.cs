using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intellivision.Cartridge
{
    public class CartridgeROM
    {
        private UInt16[] _memory;

        public CartridgeROM()
        {
            _memory = new UInt16[45000]; // no idea
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
