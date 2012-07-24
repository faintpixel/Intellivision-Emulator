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
            Console.WriteLine("\nWR a=0x" + address.ToString("X") + " v=0x" + value.ToString("X"));

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
            UInt16 returnValue = 0;

            if (address >= 0 && address <= 0x3F)
                returnValue = 0; // STIC REGISTERS
            else if (address >= 0x0100 && address <= 0x01EF)
                returnValue = _scratchpadRAM.Read(address - 0x0100);
            else if (address >= 0x01F0 && address <= 0x01FF)
                returnValue = 0; // PSG REGISTERS
            else if (address >= 0x0200 && address <= 0x035F)
                returnValue = _systemRAM.Read(address - 0x0200);
            else if (address >= 0x1000 && address <= 0x1FFF)
                returnValue = _executiveROM.Read(address - 0x1000);
            else if (address >= 0x3000 && address <= 0x37FF)
                returnValue = 0; // GRAPHICS ROM
            else if (address >= 0x3800 && address <= 0x39FF)
                returnValue = 0; // GRAPHICS RAM
            else
                returnValue = UInt16.MaxValue;

            Console.WriteLine("\nRD a=0x" + address.ToString("X") + " v=0x" + returnValue.ToString("X"));

            return returnValue;
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
