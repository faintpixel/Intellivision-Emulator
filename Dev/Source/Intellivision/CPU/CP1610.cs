using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Intellivision.Memory;

namespace Intellivision.CPU
{
    public class CP1610
    {
        private MemoryMap _memoryMap;

        public Flags Flags = new Flags();

        public UInt16[] Registers = new UInt16[8];

        public UInt16 StackPointer
        {
            get
            {
                return Registers[6];
            }
            set
            {
                Registers[6] = value;
            }
        }

        public UInt16 ProgramCounter
        {
            get
            {
                return Registers[7];
            }
            set
            {
                Registers[7] = value;
            }
        }

        public bool AllowInterupts = true;
        public int Cycles = 0;

        public delegate void OutputSignalEvent();
        public event OutputSignalEvent BusAcknowledge_BUSAK;
        public event OutputSignalEvent ExternalBranchConditionAddress0_EBCA0;
        public event OutputSignalEvent ExternalBranchConditionAddress1_EBCA1;
        public event OutputSignalEvent ExternalBranchConditionAddress2_EBCA2;
        public event OutputSignalEvent ExternalBranchConditionAddress3_EBCA3;
        public event OutputSignalEvent TerminateCurrentInterruprt_TCI;
        public event OutputSignalEvent Halted_HALT;

        private List<int> _incrementingRegisters;

        public CP1610(ref MemoryMap memoryMap)
        {
            _memoryMap = memoryMap;
            _incrementingRegisters = new List<int>();
            _incrementingRegisters.Add(4);
            _incrementingRegisters.Add(5);
            _incrementingRegisters.Add(7);
        }

        public void ExecuteInstruction()
        {
            UInt16 commandAddress = Registers[7];
            UInt16 command = _memoryMap.Read16BitsFromAddress(commandAddress);

            Console.WriteLine("Found command 0x" + command.ToString("X")  + " at address 0x" + commandAddress.ToString("X"));

            if (command == 0x0000)
            {
                Log("HLT", LogType.CommandExecution);
                Halt_HLT();
            }
            else if (command == 0x0001)
            {
                Log("SDBD", LogType.CommandExecution);
                SetDoubleByteData_SDBD();
            }
            else if (command == 0x0002)
            {
                Log("EIS", LogType.CommandExecution);
                EnableInterruptSystem_EIS();
            }
            else if (command == 0x0003)
            {
                Log("DIS", LogType.CommandExecution);
                DisableInterruptSystem_DIS();
            }
            else if (command == 0x0004)
            {
                Registers[7] += 1;
                UInt16 command2 = _memoryMap.Read16BitsFromAddress(Registers[7]);
                UInt16 registerId = GetNumberFromBits(command2, 8, 2);
                UInt16 interuptFlags = GetNumberFromBits(command2, 0, 2);
                UInt16 addressPart1 = GetNumberFromBits(command2, 2, 6);

                Registers[7] += 1;
                UInt16 command3 = _memoryMap.Read16BitsFromAddress(Registers[7]);
                UInt16 addressPart2 = GetNumberFromBits(command3, 0, 10);

                UInt16 address = (UInt16)((addressPart1 << 10) + addressPart2);

                int? register = null;

                if (registerId == 0)
                    register = 4;
                else if (registerId == 1)
                    register = 5;
                else if (registerId == 2)
                    registerId = 6;

                bool? interupt = null;
                if (interuptFlags == 1)
                    interupt = true;
                else if (interuptFlags == 2)
                    interupt = false;
                //else if (interuptFlags == 3)
                //    UNDOCUMENTED - it's a mystery!

                Log("JUMP R" + register + ", 0x" + interuptFlags.ToString("x") + ", 0x" + address.ToString("x"), LogType.CommandExecution);
                Jump_JUMP(register, interupt, address);
            }
            else if (command == 0x0005)
            {
                Log("TCI", LogType.CommandExecution);
                TerminateCurrentInterrupt_TCI();
            }
            else if (command == 0x0006)
            {
                Log("CLRC", LogType.CommandExecution);
                ClearCarry_CLRC();
            }
            else if (command == 0x0007)
            {
                Log("SETC", LogType.CommandExecution);
                SetCarry_SETC();
            }
            else if (command >= 0x0008 && command <= 0x000F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Log("INCR R" + register, LogType.CommandExecution);
                IncrementRegister_INCR(register);
            }
            else if (command >= 0x0010 && command <= 0x0017)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Log("DECR R" + register, LogType.CommandExecution);
                DecrementRegister_DECR(register);
            }
            else if (command >= 0x0018 && command <= 0x001F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Log("COMR R" + register, LogType.CommandExecution);
                ComplementRegister_COMR(register);
            }
            else if (command >= 0x0020 && command <= 0x0027)
            {
                throw new NotImplementedException("NEGR not yet implemented.");
            }
            else if (command >= 0x0028 && command <= 0x002F)
            {
                throw new NotImplementedException("ADCR not yet implemented.");
            }
            else if (command >= 0x0030 && command <= 0x0033)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                Log("GSWD R" + register, LogType.CommandExecution);
                GetStatusWord_GSWD(register);
            }
            else if (command >= 0x0034 && command <= 0x0035)
                throw new NotImplementedException("NOP not implemented");
            else if (command >= 0x0036 && command <= 0x0037)
                throw new NotImplementedException("SIN not implemented");
            else if (command >= 0x0038 && command <= 0x003F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Log("RSWD R" + register, LogType.CommandExecution);
                ReturnStatusWord_RSWD(register);
            }
            else if (command >= 0x0040 && command <= 0x0047)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 swapBit = GetNumberFromBits(command, 2, 1);
                bool doubleSwap = swapBit == 1;
                Log("SWAP R" + register + ", " + (swapBit + 1), LogType.CommandExecution);
                SwapBytes_SWAP(doubleSwap, register);
            }
            else if (command >= 0x0048 && command <= 0x004F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 shiftAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("SLL R" + register + ", " + shiftAmount, LogType.CommandExecution);
                ShiftLogicalLeft_SLL(shiftAmount, register);
            }
            else if (command >= 0x0050 && command <= 0x0057)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 rotateAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("RLC R" + register + ", " + rotateAmount, LogType.CommandExecution);
                RotateLeftThroughCarry_RLC(rotateAmount, register);
            }
            else if (command >= 0x0058 && command <= 0x005F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 shiftAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("SLLC R" + register + ", " + shiftAmount, LogType.CommandExecution);
                ShiftLogicalLeftThroughCarry_SLLC(shiftAmount, register);
            }
            else if (command >= 0x0060 && command <= 0x0067)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 shiftAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("SLR R" + register + ", " + shiftAmount, LogType.CommandExecution);
                ShiftLogicalRight_SLR(shiftAmount, register);
            }
            else if (command >= 0x0068 && command <= 0x006F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 shiftAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("SAR R" + register + ", " + shiftAmount, LogType.CommandExecution);
                ShiftArithmeticRight_SAR(shiftAmount, register);
            }
            else if (command >= 0x0070 && command <= 0x0077)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 rotateAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("RRC R" + register + ", " + rotateAmount, LogType.CommandExecution);
                RotateRightThroughCarry_RRC(rotateAmount, register);
            }
            else if (command >= 0x0078 && command <= 0x007F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 2);
                UInt16 shiftAmount = (UInt16)(GetNumberFromBits(command, 2, 1) + 1);
                Log("SARC R" + register + ", " + shiftAmount, LogType.CommandExecution);
                ShiftArtithmeticRightThroughCarry_SARC(shiftAmount, register);
            }
            else if (command >= 0x0080 && command <= 0x00BF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("MOVR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                MoveRegister_MOVR(sourceRegister, destinationRegister); 
            }
            else if (command >= 0x00C0 && command <= 0x00FF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("ADDR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                AddRegisters_ADDR(sourceRegister, destinationRegister);
            }
            else if (command >= 0x0100 && command <= 0x013F)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("SUBR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                SubtractRegisters_SUBR(sourceRegister, destinationRegister);
            }
            else if (command >= 0x0140 && command <= 0x017F)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("CMPR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                throw new NotImplementedException("CMPR not implemented");
            }
            else if (command >= 0x0180 && command <= 0x01BF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("ANDR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                AndRegisters_ANDR(sourceRegister, destinationRegister);
            }
            else if (command >= 0x01C0 && command <= 0x01FF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 sourceRegister = GetNumberFromBits(command, 3, 3);
                Log("XORR R" + sourceRegister + ", R" + destinationRegister, LogType.CommandExecution);
                XorRegisters_XORR(sourceRegister, destinationRegister);
            }
            else if (command >= 0x0200 && command <= 0x023F)
            {
                throw new NotImplementedException("Branch not implemented");
            }
            else if (command >= 0x0240 && command <= 0x0247)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("MVO R" + register + " \n " + address, LogType.CommandExecution);
                MoveOut_MVO(register, address);
            }
            else if (command >= 0x0248 && command <= 0x026F)
            {
                UInt16 sourceRegister = GetNumberFromBits(command, 0, 3);
                UInt16 destinationAddressRegister = GetNumberFromBits(command, 3, 3);
                Log("MVO@ R" + destinationAddressRegister + ", R" + sourceRegister, LogType.CommandExecution);
                MoveOutIndirect_MVOat(sourceRegister, destinationAddressRegister);
            }
            else if (command >= 0x0270 && command <= 0x027F)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Log("MVOI R" + register, LogType.CommandExecution);
                throw new NotImplementedException("MVOI not implemented");
            }
            else if (command >= 0x0280 && command <= 0x0287)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("MVI R" + register + " \n " + address, LogType.CommandExecution);
                MoveIn_MVI(register, address);
            }
            else if (command >= 0x0288 && command <= 0x02AF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("MVI@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                MoveInIndirect_MVIat(destinationRegister, addressRegister);
            }
            else if (command >= 0x02B0 && command <= 0x02BF)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 value = _memoryMap.Read16BitsFromAddress(Registers[7]);

                if (Flags.DoubleByteData)
                    value = _memoryMap.Read16BitsFromAddress(Registers[7]);
                else
                    value = _memoryMap.Read16BitsFromAddress(Registers[7]); // FIX - this should be 8 bits i think

                Log("MVII 0x" + value.ToString("x") + ", R" + register, LogType.CommandExecution);
                MoveInImmediate_MVII(register, value);
            }
            else if (command >= 0x02C0 && command <= 0x02C7)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("ADD R" + register + " \n " + address, LogType.CommandExecution);
                Add_ADD(register, address);
            }
            else if (command >= 0x02C8 && command <= 0x02EF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("ADD@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                AddIndirect_ADDat();
            }
            else if (command >= 0x02F0 && command <= 0x02FF)
            {
                throw new NotImplementedException("ADDI not implemented");
            }
            else if (command >= 0x0300 && command <= 0x0307)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("SUB R" + register + " \n " + address, LogType.CommandExecution);
                Subtract_SUB(register, address);
            }
            else if (command >= 0x0308 && command <= 0x032F)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("SUB@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                SubtractIndirect_SUBat();
            }
            else if (command >= 0x0330 && command <= 0x033F)
            {
                throw new NotImplementedException("SUBI not implemented");
            }
            else if (command >= 0x0340 && command <= 0x0347)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("CMP R" + register + " \n " + address, LogType.CommandExecution);
                Compare_CMP(register, address);
            }
            else if (command >= 0x0348 && command <= 0x036F)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("CMP@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                CompareIndirect_CMPat();
            }
            else if (command >= 0x0370 && command <= 0x037F)
            {
                throw new NotImplementedException("CMPI not implemented");
            }
            else if (command >= 0x0380 && command <= 0x0387)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("AND R" + register + " \n " + address, LogType.CommandExecution);
                And_AND(register, address);
            }
            else if (command >= 0x0388 && command <= 0x03AF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("AND@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                AndIndirect_ANDat();
            }
            else if (command >= 0x03B0 && command <= 0x03BF)
            {
                throw new NotImplementedException("ANDI not implemented");
            }
            else if (command >= 0x03C0 && command <= 0x03C7)
            {
                UInt16 register = GetNumberFromBits(command, 0, 3);
                Registers[7] += 1;
                UInt16 address = _memoryMap.Read16BitsFromAddress(Registers[7]);
                Log("XOR R" + register + " \n " + address, LogType.CommandExecution);
                Xor_XOR(register, address);
            }
            else if (command >= 0x03C8 && command <= 0x03EF)
            {
                UInt16 destinationRegister = GetNumberFromBits(command, 0, 3);
                UInt16 addressRegister = GetNumberFromBits(command, 3, 3);
                Log("XOR@ R" + addressRegister + ", R" + destinationRegister, LogType.CommandExecution);
                XorIndirect_XORat();
            }
            else if (command >= 0x03F0 && command <= 0x03FF)
            {
                throw new NotImplementedException("XORI not implemented");
            }
            else
            {
                throw new Exception("Unknown command " + command);
            }

            Registers[7] += 1;
        }

        private void Log(string message, LogType type)
        {
            Console.WriteLine(message);
        }

        #region Debug helpers

        public void DEBUG_PRINT_REGISTERS()
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                BitArray x = new BitArray(BitConverter.GetBytes(Registers[i]));
                for (int y = 0; y < x.Length; y++)
                {
                    if (x[y])
                        Console.Write("1");
                    else
                        Console.Write("0");
                    if ((y + 1) % 4 == 0 && y + 1 != x.Length)
                        Console.Write(":");
                }

                
                Console.Write(" | ");
            }
            Console.WriteLine();
        }

        public void DEBUG_PRINT_REGISTERS_AS_HEX()
        {
            for (int i = 0; i < Registers.Length; i++)
                Console.Write(i + ": 0x" + Registers[i].ToString("X") + " ");
            Console.WriteLine();
        }

        public void DEBUG_PRINT_REGISTERS_AS_INT()
        {
            for (int i = 0; i < Registers.Length; i++)
                Console.Write(i + ": " + Registers[i].ToString() + " ");
            Console.WriteLine();
        }

        public void DEBUG_PRINT_FLAGS()
        {
            Flags.DEBUG_PRINT();
        }

        #endregion

        #region Implied Addressing Mode Functions

        public void Halt_HLT()
        {
            AllowInterupts = false;
            Cycles = int.MaxValue * -1; 
            if (Halted_HALT != null)
                Halted_HALT();            
        }

        public void ClearCarry_CLRC()
        {
            AllowInterupts = false;
            Flags.Carry = false;
            Cycles -= 4;
        }

        public void DisableInterruptSystem_DIS()
        {
            AllowInterupts = false;
            Flags.InterruptEnable = false;
            Cycles -= 4;
        }

        public void EnableInterruptSystem_EIS()
        {
            AllowInterupts = false;
            Flags.InterruptEnable = true;
            Cycles -= 4;
        }

        public void SetDoubleByteData_SDBD()
        {
            AllowInterupts = false;
            Flags.DoubleByteData = true;
            Cycles -= 4;
        }

        public void SetCarry_SETC()
        {
            AllowInterupts = false;
            Flags.Carry = true;
            Cycles -= 4;
        }

        public void TerminateCurrentInterrupt_TCI()
        {
            AllowInterupts = false;
            Cycles -= 4;
            if (TerminateCurrentInterruprt_TCI != null)
                TerminateCurrentInterruprt_TCI();

        }

        #endregion

        #region Register Addressing Mode Functions

        public void GetStatusWord_GSWD(int registerNumber)
        {
            if (registerNumber > 3 || registerNumber < 0)
                throw new Exception("Invalid register for operation");

            Cycles -= 6;
            AllowInterupts = true;

            BitArray result = new BitArray(16);
            for(int i = 0; i < result.Length; i++)
                result[i] = false;

            result[15] = Flags.Sign;
            result[14] = Flags.Zero;
            result[13] = Flags.Overflow;
            result[12] = Flags.Carry;

            result[7] = Flags.Sign;
            result[6] = Flags.Zero;
            result[5] = Flags.Overflow;
            result[4] = Flags.Carry;

            Registers[registerNumber] = ConvertBitArrayToUInt16(result);
        }             

        public void ReturnStatusWord_RSWD(int registerNumber)
        {
            if (registerNumber > 7 || registerNumber < 0)
                throw new Exception("Invalid register for operation");

            AllowInterupts = true;
            Cycles -= 6;

            BitArray registerBits = GetRegisterBits(registerNumber);
            Flags.Sign = registerBits[7];
            Flags.Zero = registerBits[6];
            Flags.Overflow = registerBits[5];
            Flags.Carry = registerBits[4];            
        }

        public void SwapBytes_SWAP(bool doubleSwap, int registerNumber)
        {
            if (registerNumber < 0 || registerNumber > 3)
                throw new Exception("Invalid register for operation");

            AllowInterupts = false;

            BitArray registerBits = GetRegisterBits(registerNumber);

            if (doubleSwap)
            {
                Cycles -= 8;
                
                for (int i = 0; i < 8; i++)
                    registerBits[i + 8] = registerBits[i];               
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    bool overwrittenBit = registerBits[i + 8];
                    registerBits[i + 8] = registerBits[i];
                    registerBits[i] = overwrittenBit;
                }

                Cycles -= 6;
            }

            Flags.Sign = registerBits[7];
            UInt16 result = ConvertBitArrayToUInt16(registerBits);
            Registers[registerNumber] = ConvertBitArrayToUInt16(registerBits);

            if (result == 0)
                Flags.Zero = true;
            else
                Flags.Zero = false;            
        }

        public void ShiftLogicalLeft_SLL(int amountToShift, int registerNumber)
        {
            if (amountToShift < 1 || amountToShift > 2)
                throw new Exception("Amount to shift must be 1 or 2");
            if (registerNumber < 0 || registerNumber > 3)
                throw new Exception("Invalid register for operation");

            AllowInterupts = false;

            if (amountToShift == 1)
                Cycles -= 6;
            else
                Cycles -= 8;

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] << amountToShift);

            SetSignFlagFromRegister(registerNumber);
            SetZeroFlagFromRegister(registerNumber);
        }

        public void RotateLeftThroughCarry_RLC(int amountToRotate, int registerNumber)
        {
            BitArray originalBits = GetRegisterBits(registerNumber);
            Registers[registerNumber] = (UInt16)(Registers[registerNumber] << amountToRotate);
            BitArray newBits = GetRegisterBits(registerNumber);

            if (amountToRotate == 1)
            {
                newBits[0] = Flags.Carry;
                Flags.Carry = originalBits[15];                
                Cycles -= 6;
            }
            else
            {
                newBits[0] = Flags.Overflow;
                newBits[1] = Flags.Carry;
                Flags.Carry = originalBits[15];
                Flags.Overflow = originalBits[14]; 
                Cycles -= 8;
            }

            Registers[registerNumber] = ConvertBitArrayToUInt16(newBits);
            Flags.Sign = newBits[15];
            SetZeroFlagFromRegister(registerNumber);
        }

        public void ShiftLogicalLeftThroughCarry_SLLC(int amountToShift, int registerNumber)
        {
            BitArray originalBits = GetRegisterBits(registerNumber);

            Flags.Carry = originalBits[15];            

            if (amountToShift == 1)
            {
                Cycles -= 6;
            }
            else
            {
                Cycles -= 8;
                Flags.Overflow = originalBits[14];
            }

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] << amountToShift);

            SetSignFlagFromRegister(registerNumber);
            SetZeroFlagFromRegister(registerNumber);
        }

        public void ShiftLogicalRight_SLR(int amountToShift, int registerNumber)
        {
            if (amountToShift < 1 || amountToShift > 2)
                throw new Exception("Amount to shift must be 1 or 2");
            if (registerNumber < 0 || registerNumber > 3)
                throw new Exception("Invalid register for operation");

            AllowInterupts = false;

            if (amountToShift == 1)
                Cycles -= 6;
            else
                Cycles -= 8;

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] >> amountToShift);
            
            Flags.Zero = Registers[registerNumber] == 0;

            BitArray registerBits = GetRegisterBits(registerNumber);
            Flags.Sign = registerBits[7]; 
        }

        public void ShiftArithmeticRight_SAR(int amountToShift, int registerNumber)
        {
            BitArray originalBits = GetRegisterBits(registerNumber);

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] >> amountToShift);
            BitArray newBits = GetRegisterBits(registerNumber);
            newBits[15] = originalBits[15];

            if (amountToShift == 2)
            {
                newBits[14] = originalBits[15];
                Flags.Sign = originalBits[9];
                Cycles -= 8;
            }
            else
            {
                Flags.Sign = originalBits[8];
                Cycles -= 6;
            }

            Registers[registerNumber] = ConvertBitArrayToUInt16(newBits);

            SetZeroFlagFromRegister(registerNumber);

        }

        public void ShiftArtithmeticRightThroughCarry_SARC(int amountToShift, int registerNumber)
        {
            BitArray originalBits = GetRegisterBits(registerNumber);

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] >> amountToShift);
            BitArray updatedBits = GetRegisterBits(registerNumber);

            if (amountToShift == 1)
                Cycles -= 6;
            else
            {
                Cycles -= 8;
                Flags.Overflow = originalBits[1];
                updatedBits[14] = originalBits[15];
            }

            updatedBits[15] = originalBits[15];

            Registers[registerNumber] = ConvertBitArrayToUInt16(updatedBits);
            
            Flags.Carry = originalBits[0];

            
            Flags.Sign = updatedBits[7];
            SetZeroFlagFromRegister(registerNumber);
        }

        public void RotateRightThroughCarry_RRC(int amountToRotate, int registerNumber)
        {
            BitArray originalBits = GetRegisterBits(registerNumber);

            Registers[registerNumber] = (UInt16)(Registers[registerNumber] >> amountToRotate);

            BitArray updatedBits = GetRegisterBits(registerNumber);

            if (amountToRotate == 1)
            {
                Cycles -= 6;
                updatedBits[15] = Flags.Carry;
            }
            else
            {
                Cycles -= 8;
                updatedBits[15] = Flags.Overflow;
                updatedBits[14] = Flags.Carry;
                Flags.Overflow = originalBits[1];
            }
            Registers[registerNumber] = ConvertBitArrayToUInt16(updatedBits);

            Flags.Carry = originalBits[0];
            Flags.Sign = updatedBits[7];
            SetZeroFlagFromRegister(registerNumber);
        }

        public void MoveRegister_MOVR(int sourceRegister, int destinationRegister)
        {
            if (destinationRegister == 6 || destinationRegister == 7)
                Cycles -= 7;
            else
                Cycles -= 6;

            Registers[destinationRegister] = Registers[sourceRegister];

            SetSignFlagFromRegister(destinationRegister);
            SetZeroFlagFromRegister(destinationRegister);
        }

        public void AddRegisters_ADDR(int sourceRegister, int destinationRegister)
        {
            Registers[destinationRegister] = PerformAddAndSetFlags(Registers[sourceRegister], Registers[destinationRegister]);            
        }

        private UInt16 PerformAddAndSetFlags(UInt16 sourceValue, UInt16 destinationValue)
        {
            UInt16 result;

            BitArray originalSourceBits = ConvertUInt16ToBitArray(sourceValue);
            BitArray originalDestinationBits = ConvertUInt16ToBitArray(destinationValue);

            result = (UInt16)(sourceValue + destinationValue);

            BitArray resultBits = ConvertUInt16ToBitArray(result);

            Flags.Sign = resultBits[15];
            Flags.Zero = result == 0;

            if (resultBits[15] == true && (originalDestinationBits[15] == false && originalSourceBits[15] == false))
                Flags.Overflow = true;
            else if (resultBits[15] == false && (originalDestinationBits[15] == true && originalSourceBits[15] == true))
                Flags.Overflow = true;
            else
                Flags.Overflow = false;

            if ((UInt32)(sourceValue + destinationValue) > UInt16.MaxValue)
                Flags.Carry = true;
            else
                Flags.Carry = false;

            return result;
        }

        public void SubtractRegisters_SUBR(int sourceRegister, int destinationRegister)
        {
            Registers[destinationRegister] = PerformSubtractAndSetFlags(Registers[sourceRegister], Registers[destinationRegister]);                 
        }

        private UInt16 PerformSubtractAndSetFlags(UInt16 sourceValue, UInt16 destinationValue)
        {
            UInt16 result;

            BitArray sourceBits = ConvertUInt16ToBitArray(sourceValue);
            BitArray destinationBits = ConvertUInt16ToBitArray(destinationValue);

            if (destinationValue >= sourceValue)
                Flags.Carry = true;
            else
                Flags.Carry = false;

            result = (UInt16)(destinationValue - sourceValue);

            BitArray resultBits = ConvertUInt16ToBitArray(result);
            if (resultBits[15] == false && (sourceBits[15] == false && destinationBits[15] == true))
                Flags.Overflow = true;
            else if (resultBits[15] == true && (sourceBits[15] == true && destinationBits[15] == false))
                Flags.Overflow = true;
            else
                Flags.Overflow = false;

            Flags.Zero = result == 0;
            Flags.Sign = resultBits[15];

            return result;
        }

        public void IncrementRegister_INCR(int registerNumber)
        {
            Registers[registerNumber] += 1;

            SetSignFlagFromRegister(registerNumber);
            SetZeroFlagFromRegister(registerNumber);
        }

        public void DecrementRegister_DECR(int registerNumber)
        {
            Registers[registerNumber] -= 1;

            SetSignFlagFromRegister(registerNumber);
            SetZeroFlagFromRegister(registerNumber);
        }

        public void ComplementRegister_COMR(int registerNumber)
        {
            Cycles -= 6;

            BitArray bits = GetRegisterBits(registerNumber);
            for (int i = 0; i < bits.Length; i++)
                bits[i] = !bits[i];

            Registers[registerNumber] = ConvertBitArrayToUInt16(bits);

            SetZeroFlagFromRegister(registerNumber);
            SetSignFlagFromRegister(registerNumber);
        }

        public void AndRegisters_ANDR(int sourceRegister, int destinationRegister)
        {
            Registers[destinationRegister] = PerformANDAndSetFlags(Registers[sourceRegister], Registers[destinationRegister]);
        }

        private UInt16 PerformANDAndSetFlags(UInt16 sourceValue, UInt16 destinationValue)
        {
            UInt16 result = (UInt16)(sourceValue & destinationValue);
            BitArray bits = ConvertUInt16ToBitArray(result);

            Flags.Zero = result == 0;
            Flags.Sign = bits[15];

            return result;
        }

        public void XorRegisters_XORR(int sourceRegister, int destinationRegister)
        {
            Registers[destinationRegister] = PerformXorAndSetFlags(Registers[sourceRegister], Registers[destinationRegister]);
        }

        private UInt16 PerformXorAndSetFlags(UInt16 sourceValue, UInt16 destinationValue)
        {
            UInt16 result = (UInt16)(sourceValue ^ destinationValue);

            BitArray bits = ConvertUInt16ToBitArray(result);

            Flags.Sign = bits[15];
            Flags.Zero = result == 0;

            return result;
        }



        #endregion

        #region Direct Addressing Mode Functions

        public void MoveOut_MVO(int sourceRegister, UInt16 destinationAddress)
        {
            _memoryMap.Write16BitsToAddress(destinationAddress, Registers[sourceRegister]);
        }

        public void MoveIn_MVI(int destinationRegister, UInt16 sourceAddress)
        {
            Registers[destinationRegister] = _memoryMap.Read16BitsFromAddress(sourceAddress);
        }

        public void Add_ADD(int destinationRegister, UInt16 sourceAddress)
        {
            UInt16 valueAtAddress = _memoryMap.Read16BitsFromAddress(sourceAddress);
            Registers[destinationRegister] = PerformAddAndSetFlags(valueAtAddress, Registers[destinationRegister]); 
        }

        public void Subtract_SUB(int destinationRegister, UInt16 sourceAddress)
        {
            UInt16 sourceValue = _memoryMap.Read16BitsFromAddress(sourceAddress);
            Registers[destinationRegister] = PerformSubtractAndSetFlags(sourceValue, Registers[destinationRegister]);     
        }

        public void Compare_CMP(int destinationRegister, UInt16 sourceAddress)
        {
            UInt16 sourceValue = _memoryMap.Read16BitsFromAddress(sourceAddress);
            UInt16 destinationValue = Registers[destinationRegister];

            BitArray sourceBits = ConvertUInt16ToBitArray(sourceValue);
            BitArray destinationBits = ConvertUInt16ToBitArray(destinationValue);

            if (destinationValue >= sourceValue)
                Flags.Carry = true;
            else
                Flags.Carry = false;

            UInt16 result = (UInt16)(destinationValue - sourceValue);

            BitArray resultBits = ConvertUInt16ToBitArray(result);
            if (resultBits[15] == false && (sourceBits[15] == false && destinationBits[15] == true))
                Flags.Overflow = true;
            else if (resultBits[15] == true && (sourceBits[15] == true && destinationBits[15] == false))
                Flags.Overflow = true;
            else
                Flags.Overflow = false;

            Flags.Zero = result == 0;
            Flags.Sign = resultBits[15];
        }

        public void And_AND(int destinationRegister, UInt16 sourceAddress)
        {
            UInt16 sourceValue = _memoryMap.Read16BitsFromAddress(sourceAddress);
            Registers[destinationRegister] = PerformANDAndSetFlags(sourceValue, Registers[destinationRegister]);
        }

        public void Xor_XOR(int destinationRegister, UInt16 sourceAddress)
        {
            UInt16 sourceValue = _memoryMap.Read16BitsFromAddress(sourceAddress);
            Registers[destinationRegister] = PerformXorAndSetFlags(sourceValue, Registers[destinationRegister]);
        }

        #endregion

        #region Indirect Addressing Mode Functions

        public void Jump_JUMP(int? returnAddressRegister, bool? interuptFlag, UInt16 address)
        {
            if (returnAddressRegister != null)
                Registers[returnAddressRegister.Value] = Registers[7]; // this should be the address right after the jump instruction

            if (interuptFlag != null)
                Flags.InterruptEnable = interuptFlag.Value;

            Registers[7] = (ushort)(address - 1); // minus one so the first command at the address will be executed

            if (returnAddressRegister != null)
                IncrementRegisterForIndirectMode(returnAddressRegister.Value);
        }

        public void MoveInIndirect_MVIat(int destinationRegister, int addressRegister)
        {
            // Q - Does MVI@ set the zero/sign flag like INCR?
            // Q - MVI@ uses 2 registers... if I use R4 for one and R5 for the other, do they both increment?
            // The answers to the above questions will affect all other indirect methods as well..

            if (destinationRegister == 6)
                Registers[6] -= 1;
            
            UInt16 address = Registers[addressRegister];
            UInt16 value;

            if (Flags.DoubleByteData)
                value = _memoryMap.Read16BitsFromAddress(address);
            else
                value = _memoryMap.Read8BitsFromAddress(address);

            Registers[destinationRegister] = value;

            IncrementRegistersForIndirectMode(destinationRegister, addressRegister);            
        }

        public void MoveInImmediate_MVII(int destinationRegister, UInt16 value)
        {
            // Q - Does MVI@ set the zero/sign flag like INCR?
            // Q - MVI@ uses 2 registers... if I use R4 for one and R5 for the other, do they both increment?
            // The answers to the above questions will affect all other indirect methods as well..

            if (destinationRegister == 6)
                Registers[6] -= 1;

            Registers[destinationRegister] = value;

            IncrementRegisterForIndirectMode(destinationRegister);   
        }

        private void IncrementRegistersForIndirectMode(int writeRegister, int readRegister)
        {
            if (writeRegister == 6)
                Registers[6] += 1;
            if (_incrementingRegisters.Contains(writeRegister))
                Registers[writeRegister] += 1;
            if (_incrementingRegisters.Contains(readRegister))
                Registers[readRegister] += 1;
        }

        private void IncrementRegisterForIndirectMode(int register)
        {
            if (_incrementingRegisters.Contains(register))
                Registers[register] += 1;
        }

        public void MoveOutIndirect_MVOat(int sourceRegister, int destinationAddressRegister)
        {
            // Q - For MVO@, assuming that DoubleByteData flag makes it either write 8bits of 16
            // Q - How does MVO@ work with R6? Neither register gets written to, so R6 will never increment?

            //if (sourceRegister == 6)
            //    Registers[6] -= 1;

            //UInt16 address = Registers[addressRegister];
            //UInt16 value = Registers[sourceRegister];

            //if (Flags.DoubleByteData)
            //    _memoryMap.Write16BitsToAddress(address, value);
            //else
            //    _memoryMap.Write8BitsToAddress(address, value);

            //IncrementRegistersForIndirectMode(sourceRegister, addressRegister);    
        }

        public void AddIndirect_ADDat()
        {
            throw new NotImplementedException();
        }

        public void SubtractIndirect_SUBat()
        {
            throw new NotImplementedException();
        }

        public void CompareIndirect_CMPat()
        {
            throw new NotImplementedException();
        }

        public void AndIndirect_ANDat()
        {
            throw new NotImplementedException();
        }

        public void XorIndirect_XORat()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helpers

        public BitArray GetRegisterBits(int registerNumber)
        {
            BitArray bits = new BitArray(BitConverter.GetBytes(Registers[registerNumber]));

            return bits;
        }

        public BitArray ConvertUInt16ToBitArray(UInt16 value)
        {            
            byte[] bytes = BitConverter.GetBytes(value);

            BitArray bits = new BitArray(bytes);

            //ReverseBitArray(ref bits);

            return bits;
        }

        private void DEBUG_PRINT_BIT_ARRAY(BitArray bits)
        {
            foreach (bool bit in bits)
                if (bit == true)
                    Console.Write("1");
                else
                    Console.Write("0");
        }

        //private void ReverseBitArray(ref BitArray array)
        //{
        //    int length = array.Length;
        //    int mid = (length / 2);

        //    for (int i = 0; i < mid; i++)
        //    {
        //        bool bit = array[i];
        //        array[i] = array[length - i - 1];
        //        array[length - i - 1] = bit;
        //    }
        //}

        public UInt16 GetNumberFromBits(UInt16 number, int startIndex, int length)
        {
            UInt16 result;

            BitArray bits = ConvertUInt16ToBitArray(number);
            BitArray bitsOfInterest = new BitArray(length);
            int a = 0;
            for (int i = startIndex; i < startIndex + length; i++)
            {
                bitsOfInterest[a] = bits[i];
                a += 1;
            }

            result = ConvertBitArrayToUInt16(bitsOfInterest);

            return result;
        }

        public UInt16 ConvertBitArrayToUInt16(BitArray bitArray)
        {
            Int32[] result = new Int32[1];

            //ReverseBitArray(ref bitArray);
            bitArray.CopyTo(result, 0);

            return (UInt16)result[0];
        }

        private void SetSignFlagFromRegister(int n)
        {
            BitArray registerBits = GetRegisterBits(n);
            Flags.Sign = registerBits[15];
        }

        public void SetZeroFlagFromRegister(int n)
        {
            Flags.Zero = Registers[n] == 0;
        }

        #endregion
    }
}
