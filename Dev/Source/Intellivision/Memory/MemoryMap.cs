using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intellivision.Memory.RAM;
using Intellivision.Memory.ROM;

namespace Intellivision.Memory
{
    public class MemoryMap
    {
        private SystemRAM _systemRAM = new SystemRAM();
        private ScratchpadRAM _scratchpadRAM = new ScratchpadRAM();
        private ExecutiveROM _executiveROM = new ExecutiveROM();

        public void Write16BitsToAddress(UInt16 address, UInt16 value)
        {
            int nothing = 0;

            if (address >= 0 && address <= 0x3F)
                nothing = 0; // STIC REGISTERS
            else if (address >= 0x0100 && address <= 0x01EF)
                _scratchpadRAM.Write(address - 0x100, value);
            else if (address >= 0x01F0 && address <= 0x01FF)
                nothing = 0; // PSG REGISTERS
            else if (address >= 0x0200 && address <= 0x035F)
                _systemRAM.Write(address - 0x0200, value);
            else if (address >= 0x1000 && address <= 0x1FFF)
                _executiveROM.Write(address - 0x1000, value);
            else if (address >= 0x3000 && address <= 0x37FF)
                nothing = 0; // GRAPHICS ROM
            else if (address >= 0x3800 && address <= 0x39FF)
                nothing = 0; // GRAPHICS RAM
            else
                throw new Exception("Writing outside of available memory.");
        }

        public UInt16 Read16BitsFromAddress(UInt16 address)
        {
            if (address >= 0 && address <= 0x3F)
                return 0; // STIC REGISTERS
            else if (address >= 0x0100 && address <= 0x01EF)
                return _scratchpadRAM.Read(address - 0x0100);
            else if (address >= 0x01F0 && address <= 0x01FF)
                return 0; // PSG REGISTERS
            else if (address >= 0x0200 && address <= 0x035F)
                return _systemRAM.Read(address - 0x0200);
            else if (address >= 0x1000 && address <= 0x1FFF)
                return _executiveROM.Read(address - 0x1000);
            else if (address >= 0x3000 && address <= 0x37FF)
                return 0; // GRAPHICS ROM
            else if (address >= 0x3800 && address <= 0x39FF)
                return 0; // GRAPHICS RAM
            else
                return UInt16.MaxValue;
        }

        public void Write8BitsToAddress(UInt16 address, UInt16 value)
        {
            throw new NotImplementedException();
        }

        public byte Read8BitsFromAddress(UInt16 address)
        {
            throw new NotImplementedException();
        }
    }
}
