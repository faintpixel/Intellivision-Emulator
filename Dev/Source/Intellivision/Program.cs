using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Intellivision.CPU;
using Intellivision.Memory;

namespace Intellivision
{
    class Program
    {
        static bool _programRunning = true;

        static void Main(string[] args)
        {
            MemoryMap memory = new MemoryMap();
            CP1610 cpu = new CP1610(ref memory);
            cpu.Halted_HALT += new CP1610.OutputSignalEvent(cpu_Halted_HALT);

            // set the start position
            cpu.Registers[7] = 0x200;

            // load an imaginary program into memory
            //memory.Write16BitsToAddress(0x200, 0x0240);  // MVO R0 23
            //memory.Write16BitsToAddress(0x201, 23);  
            //memory.Write16BitsToAddress(0x202, 0xD); // INCR R5

            // load from file

            BinaryReader reader = new BinaryReader(File.Open("hi.bin", FileMode.Open));
            int pos = 0;
            UInt16 index = 0x200;

            int length = (int)reader.BaseStream.Length;
            try
            {
                while (pos < length)
                {
                    byte[] word = reader.ReadBytes(2);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(word);

                    UInt16 data = BitConverter.ToUInt16(word, 0);
                    //UInt16 data = reader.ReadUInt16();
                    Console.Write(data.ToString("X") + ":");
                    memory.Write16BitsToAddress(index, data);
                    pos += sizeof(UInt16);
                    index += 1;
                }
            }
            catch (Exception ex)
            {
            }
             

            // execute
            while (_programRunning)
            {
                try
                {
                    cpu.ExecuteInstruction();
                    //cpu.DEBUG_PRINT_REGISTERS_AS_HEX();
                    //cpu.DEBUG_PRINT_FLAGS();
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    cpu.Registers[7] += 1;
                    Console.ReadLine();
                }
            }

            Console.Write("Done.");
            Console.ReadLine();
        }

        static void cpu_Halted_HALT()
        {
            _programRunning = false;
        }
 
    }
}
