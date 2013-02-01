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
        private GraphicsRAM _graphicsRAM = new GraphicsRAM();
        private GraphicsROM _graphicsROM = new GraphicsROM();
        private Cartridge.CartridgeROM _cartridge = new Cartridge.CartridgeROM();

        public MemoryMap()
        {
        }

        public void Write16BitsToAddress(UInt16 address, UInt16 value)
        {
            Console.WriteLine("   WR a=0x" + address.ToString("X") + " v=0x" + value.ToString("X"));

            if (address >= 0 && address <= 0x7F) // note for some reason the STIC only actually uses the range from 0 to 0x3F
                MasterComponent.Instance.STIC.Write(address, value); // STIC REGISTERS
            else if (address >= 0x0100 && address <= 0x01EF)
                _scratchpadRAM.Write(address - 0x100, value);
            else if (address >= 0x01F0 && address <= 0x01FF)
                MasterComponent.Instance.PSG.Write(address - 0x01F0, value); // not totally sure this needs to be subtracting
            else if (address >= 0x0200 && address <= 0x035F)
                _systemRAM.Write(address - 0x0200, value);
            else if (address >= 0x1000 && address <= 0x1FFF)
                _executiveROM.Write(address - 0x1000, value);
            else if (address >= 0x3000 && address <= 0x37FF)
                _graphicsROM.Write(address - 0x3000, value);
            else if (address >= 0x3800 && address <= 0x39FF)
                _graphicsRAM.Write(address - 0x3800, value);
            else
                _cartridge.Write(address - 0x3A00, value);
        }

        public UInt16 Read16BitsFromAddress(UInt16 address)
        {
            UInt16 returnValue = 0;

            if (address >= 0 && address <= 0x7F) // note for some reason the STIC only actually uses the range from 0 to 0x3F
                returnValue = MasterComponent.Instance.STIC.Read(address); // STIC REGISTERS
            else if (address >= 0x0100 && address <= 0x01EF)
                returnValue = _scratchpadRAM.Read(address - 0x0100);
            else if (address >= 0x01F0 && address <= 0x01FF)
                returnValue = MasterComponent.Instance.PSG.Read(address - 0x01F0); // not totally sure this needs to be subtracting
            else if (address >= 0x0200 && address <= 0x035F)
                returnValue = _systemRAM.Read(address - 0x0200);
            else if (address >= 0x1000 && address <= 0x1FFF)
                returnValue = _executiveROM.Read(address - 0x1000);
            else if (address >= 0x3000 && address <= 0x37FF)
                returnValue = _graphicsROM.Read(address - 0x3000);
            else if (address >= 0x3800 && address <= 0x39FF)
                returnValue = _graphicsRAM.Read(address - 0x3800); 
            else
                returnValue = _cartridge.Read(address - 0x3A00); 

            Console.WriteLine("   RD a=0x" + address.ToString("X") + " v=0x" + returnValue.ToString("X") + ", " + Convert.ToString(returnValue, 2).PadLeft(8, '0'));

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
