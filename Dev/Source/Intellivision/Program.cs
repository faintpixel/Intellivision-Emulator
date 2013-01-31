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
            MasterComponent.Instance.Start();

            MasterComponent.Instance.CPU.Halted_HALT += new CP1610.OutputSignalEvent(cpu_Halted_HALT);
            MasterComponent.Instance.CPU.Log += new CP1610.LoggingEvent(cpu_Log);

            // load an imaginary program into memory
            //memory.Write16BitsToAddress(0x200, 0x0240);  // MVO R0 23
            //memory.Write16BitsToAddress(0x201, 23);  
            //memory.Write16BitsToAddress(0x202, 0xD); // INCR R5

            // load from file

            LoadRom("system.bin", 0x1000);
            LoadRom("hi.bin", 0x5000);
            
            Console.WriteLine("\n");
            Console.WriteLine("Roms loaded. Beginning execution.");
            Console.WriteLine();

            // execute
            while (_programRunning)
            {
                try
                {
                    MasterComponent.Instance.CPU.DEBUG_PRINT_JZINTV_STYLE_DEBUG_INFO();
                    MasterComponent.Instance.CPU.ExecuteInstruction();
                    //cpu.DEBUG_PRINT_REGISTERS_AS_HEX();
                    //cpu.DEBUG_PRINT_FLAGS();
                    Console.Write("> ");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MasterComponent.Instance.CPU.Registers[7] += 1;
                    Console.ReadLine();
                }
            }

            Console.WriteLine("------------REGISTER VALUES-------------");
            MasterComponent.Instance.CPU.DEBUG_PRINT_REGISTERS_AS_INT();

            Console.Write("Done.");
            Console.ReadLine();
        }

        static void cpu_Log(string message, LogType type)
        {
            Console.WriteLine(message);
        }

        static void cpu_Halted_HALT()
        {
            _programRunning = false;
        }

        static void LoadRom(string fileName, UInt16 memoryAddress)
        {
            BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open));
            int pos = 0;
            UInt16 index = memoryAddress;

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
                    MasterComponent.Instance.MemoryMap.Write16BitsToAddress(index, data);
                    pos += sizeof(UInt16);
                    index += 1;
                }

                Console.WriteLine("Loaded rom " + fileName + " into 0x" + memoryAddress.ToString("X"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading rom " + fileName + " into 0x" + memoryAddress.ToString("X"));
                Console.WriteLine(ex);
            }
            reader.Close();
        }
 
    }
}
