using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NUnit.Framework;
using Intellivision.CPU;
using Intellivision.Memory;

namespace Intellivision.Tests.CPU
{
    [TestFixture]
    public class RegisterAddressingModeFunctions
    {
        public CP1610 Cpu;
        public MemoryMap Memory;
        public STIC.AY_3_8900 Stic;
        public PSG.AY_3_891x Psg;

        [SetUp]
        public void Setup()
        {
            Stic = new STIC.AY_3_8900();
            Psg = new PSG.AY_3_891x();
            Memory = new MemoryMap(ref Stic, ref Psg);
            Cpu = new CP1610(ref Memory);
        }

        #region GSWD - GetStatusWord

        [Test]
        public void GetStatusWord_ShouldSetRegister()
        {
            Cpu.Flags.Sign = true;
            Cpu.Flags.Carry = true;
            Cpu.Flags.Overflow = true;
            Cpu.Flags.Zero = true;

            Cpu.GetStatusWord_GSWD(0);
            Assert.AreEqual(61680, Cpu.Registers[0]); // 1111000011110000
        }

        [Test]
        public void GetStatusWord_ShouldSetRegister2()
        {
            Cpu.Flags.Sign = true;
            Cpu.Flags.Zero = false;
            Cpu.Flags.Overflow = true;
            Cpu.Flags.Carry = false;

            Cpu.GetStatusWord_GSWD(1);
            Assert.AreEqual(41120, Cpu.Registers[1]); // 1010000010100000
        }

        #endregion

        #region RSWD - ReturnStatusWord

        [Test]
        public void ReturnStatusWord_ShouldSetRegister()
        {
            Cpu.Registers[1] = 240; // ________11110000

            Cpu.ReturnStatusWord_RSWD(1);

            Assert.IsTrue(Cpu.Flags.Sign);
            Assert.IsTrue(Cpu.Flags.Zero);
            Assert.IsTrue(Cpu.Flags.Overflow);
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void ReturnStatusWord_ShouldSetRegister2()
        {
            Cpu.Registers[1] = 0;

            Cpu.ReturnStatusWord_RSWD(1);

            Assert.IsFalse(Cpu.Flags.Sign);
            Assert.IsFalse(Cpu.Flags.Zero);
            Assert.IsFalse(Cpu.Flags.Overflow);
            Assert.IsFalse(Cpu.Flags.Carry);
        }

        #endregion

        #region SWAP - SwapBytes

        [Test]
        public void SwapBytes_ShouldPerformDoubleSwap()
        {
            Cpu.Registers[2] = 255; // ________11111111

            Cpu.SwapBytes_SWAP(true, 2);

            Assert.IsTrue(Cpu.Flags.Sign);
            Assert.IsFalse(Cpu.Flags.Zero);
            Assert.AreEqual(65535, Cpu.Registers[2]); // 1111111111111111
        }

        [Test]
        public void SwapBytes_ShouldPerformSingleSwap()
        {
            Cpu.Registers[2] = 255; // ________11111111

            Cpu.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(65280, Cpu.Registers[2]); // 1111111100000000
            Assert.IsFalse(Cpu.Flags.Sign, "Sign flag");
            Assert.IsFalse(Cpu.Flags.Zero, "Zero flag");            
        }

        [Test]
        public void SwapBytes_ShouldPerformSingleSwap2()
        {
            Cpu.Registers[2] = 52415; // 1100110010111111

            Cpu.SwapBytes_SWAP(false, 2);

            Assert.IsTrue(Cpu.Flags.Sign);
            Assert.IsFalse(Cpu.Flags.Zero);
            Assert.AreEqual(49100, Cpu.Registers[2]); // 1011111111001100
        }

        [Test]
        public void SwapBytes_ShouldUpdateSignFlag()
        {
            Cpu.Registers[2] = 255; // ________11111111

            Cpu.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(65280, Cpu.Registers[2]); // 1111111100000000
            Assert.IsFalse(Cpu.Flags.Sign, "Sign flag");
            Assert.IsFalse(Cpu.Flags.Zero, "Zero flag"); 
        }

        [Test]
        public void SwapBytes_ShouldUpdateSignFlag2()
        {
            Cpu.Registers[2] = 65280; // 1111111100000000 

            Cpu.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(255, Cpu.Registers[2]); // ________11111111
            Assert.IsTrue(Cpu.Flags.Sign, "Sign flag");
            Assert.IsFalse(Cpu.Flags.Zero, "Zero flag");
        }

        [Test]
        public void SwapBytes_ShouldSetZeroFlag()
        {
            Cpu.Registers[2] = 0; 

            Cpu.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(0, Cpu.Registers[2]); 
            Assert.IsFalse(Cpu.Flags.Sign, "Sign flag");
            Assert.IsTrue(Cpu.Flags.Zero, "Zero flag");    
        }

        #endregion

        #region SLL - ShiftLogicalLeft

        [Test]
        public void ShiftLogicalLeft_ShouldPerformSingleShift()
        {
            Cpu.Registers[0] = 10; // ____________1010

            Cpu.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(20, Cpu.Registers[0]); // __________10100
        }

        [Test]
        public void ShiftLogicalLeft_ShouldPerformDoubleShift()
        {
            Cpu.Registers[0] = 10; // ____________1010

            Cpu.ShiftLogicalLeft_SLL(2, 0);

            Assert.AreEqual(40, Cpu.Registers[0]); // _________101000    
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetSignFlag()
        {
            Cpu.Registers[0] = 48896; // 1011111100000000

            Cpu.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(32256, Cpu.Registers[0]); // 0111111000000000

            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetSignFlag2()
        {
            Cpu.Registers[0] = 65280; // 1111111100000000

            Cpu.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(65024, Cpu.Registers[0]); // 1111111000000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetZeroFlag()
        {
            Cpu.Registers[0] = 65280; // 1111111100000000

            Cpu.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(65024, Cpu.Registers[0]); // 1111111000000000            
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetZeroFlag2()
        {
            Cpu.Registers[0] = 0;

            Cpu.ShiftLogicalLeft_SLL(2, 0);

            Assert.AreEqual(0, Cpu.Registers[0]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        #endregion

        #region RLC - RotateLeftThroughCarry

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformSingleRotate()
        {
            Cpu.Flags.Carry = true;
            Cpu.Registers[1] = 41120; // 1010000010100000
                                
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16705, Cpu.Registers[1]); // _100000101000001
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformSingleRotate2()
        {
            Cpu.Flags.Carry = false;
            Cpu.Registers[1] = 41120; // 1010000010100000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16704, Cpu.Registers[1]); // _100000101000000
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate()
        {
            Cpu.Flags.Carry = true;
            Cpu.Flags.Overflow = true;

            Cpu.Registers[1] = 41120; // 1010000010100000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33411, Cpu.Registers[1]); // 1000001010000011
            Assert.IsTrue(Cpu.Flags.Carry, "carry");
            Assert.IsFalse(Cpu.Flags.Overflow, "overflow");
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate2()
        {
            Cpu.Flags.Carry = false;
            Cpu.Flags.Overflow = false;

            Cpu.Registers[1] = 41120; // 1010000010100000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33408, Cpu.Registers[1]); // 1000001010000000
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate3()
        {
            Cpu.Flags.Carry = false;
            Cpu.Flags.Overflow = true;

            Cpu.Registers[1] = 41120; // 1010000010100000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33409, Cpu.Registers[1]); // 1000001010000001
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate4()
        {
            Cpu.Flags.Carry = true;
            Cpu.Flags.Overflow = false;

            Cpu.Registers[1] = 41120; // 1010000010100000
                         
            Cpu.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33410, Cpu.Registers[1]); // 1000001010000010
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetZeroFlag()
        {
            Cpu.Flags.Carry = true;
            Cpu.Registers[1] = 41120; // 1010000010100000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16705, Cpu.Registers[1]); // _100000101000001
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 32768; // 1000000000000000
            //                    
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(0, Cpu.Registers[1]); 
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 41120; // 1010000010100000
                             
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16704, Cpu.Registers[1]); // _100000101000000
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 57504; // 1110000010100000
                  
            Cpu.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(49472, Cpu.Registers[1]); // 1100000101000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        #endregion

        #region SLLC - ShiftLogicalLeftThroughCarry

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldPerformSingleShift()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, Cpu.Registers[1]); // _101111000010100
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldPerformDoubleShift()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, Cpu.Registers[1]); // 1011110000101000
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetOverflowFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, Cpu.Registers[1]); // 1011110000101000
            Assert.IsFalse(Cpu.Flags.Overflow);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetOverflowFlag2()
        {
            Cpu.Registers[1] = 61194; // 1110111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, Cpu.Registers[1]); // 1011110000101000
            Assert.IsTrue(Cpu.Flags.Overflow);
        }
        
        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetCarryFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, Cpu.Registers[1]); // _101111000010100
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetCarryFlag2()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, Cpu.Registers[1]); // __101111000010100
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, Cpu.Registers[1]); // __101111000010100
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 0;

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(0, Cpu.Registers[1]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, Cpu.Registers[1]); // __101111000010100
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 61194; // 1110111100001010

            Cpu.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(56852, Cpu.Registers[1]); // 1101111000010100
            Assert.IsTrue(Cpu.Flags.Sign);
        }



        #endregion

        #region SLR - ShiftLogicalRight

        [Test]
        public void ShiftLogicalRight_ShouldPerformSingleShift()
        {
            Cpu.Registers[0] = 13; // ____________1101

            Cpu.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(6, Cpu.Registers[0]); // _____________110
        }

        [Test]
        public void ShiftLogicalRight_ShouldPerformDoubleShift()
        {
            Cpu.Registers[0] = 65531; // 1111111111111011

            Cpu.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(16382, Cpu.Registers[0]); // 0011111111111110
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetZeroFlag()
        {
            Cpu.Registers[0] = 65531; // 1111111111111011

            Cpu.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(16382, Cpu.Registers[0]); // 0011111111111110
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetZeroFlag2()
        {
            Cpu.Registers[0] = 0;

            Cpu.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(0, Cpu.Registers[0]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetSignFlag()
        {
            Cpu.Registers[0] = 256; // _______100000000

            Cpu.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(128, Cpu.Registers[0]); // ________10000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetSignFlag2()
        {
            Cpu.Registers[0] = 4; // 100

            Cpu.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(2, Cpu.Registers[0]); // 10
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        #endregion

        #region SAR - ShiftArithmeticRight

        [Test]
        public void ShiftArithmeticRight_ShouldPerformSingleShift()
        {
            Cpu.Registers[1] = 43690; // 1010101010101010

            Cpu.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, Cpu.Registers[1]); // 1101010101010101
        }

        [Test]
        public void ShiftArithmeticRight_ShouldPerformDoubleShift()
        {
            Cpu.Registers[1] = 43690; // 1010101010101010

            Cpu.ShiftArithmeticRight_SAR(2, 1);

            Assert.AreEqual(60074, Cpu.Registers[1]); // 1110101010101010
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 43690; // 1010101010101010

            Cpu.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, Cpu.Registers[1]); // 1101010101010101
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 1; // _______________1

            Cpu.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(0, Cpu.Registers[1]); 
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 43690; // 10101010 10101010

            Cpu.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, Cpu.Registers[1]); // 1101010101010101
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 43946; // 1010101110101010

            Cpu.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54741, Cpu.Registers[1]); // 1101010111010101
            Assert.IsTrue(Cpu.Flags.Sign);
        }
        
        #endregion

        #region SARC - ShiftArithmeticRightThroughCarry

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformSingleShift()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(1, 1);

            Assert.AreEqual(55173, Cpu.Registers[1]); // 1101011110000101
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformSingleShift2()
        {
            Cpu.Registers[1] = 12042; // __10111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(1, 1);

            Assert.AreEqual(6021, Cpu.Registers[1]); // ___1011110000101
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformDoubleShift()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, Cpu.Registers[1]); // 1110101111000010
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformDoubleShift2()
        {
            Cpu.Registers[1] = 28426; // _110111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(7106, Cpu.Registers[1]); // ___1101111000010
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, Cpu.Registers[1]); // 1110101111000010
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 0; 

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(0, Cpu.Registers[1]); 
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, Cpu.Registers[1]); // 1110 1011 1100 0010
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 44299; // 1010110100001011

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, Cpu.Registers[1]); // 1110101101000010
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetCarryFlag()
        {
            Cpu.Registers[1] = 44299; // 1010110100001011

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, Cpu.Registers[1]); // 1110101101000010
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetCarryFlag2()
        {
            Cpu.Registers[1] = 44298; // 1010110100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, Cpu.Registers[1]); // 1110101101000010
            Assert.IsFalse(Cpu.Flags.Carry);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetOverflowFlag()
        {
            Cpu.Registers[1] = 44298; // 1010110100001010

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, Cpu.Registers[1]); // 1110101101000010
            Assert.IsFalse(Cpu.Flags.Carry);
            Assert.IsTrue(Cpu.Flags.Overflow);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetOverflowFlag2()
        {
            Cpu.Registers[1] = 44296; // 1010110100001000

            Cpu.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, Cpu.Registers[1]); // 1110101101000010
            Assert.IsFalse(Cpu.Flags.Carry);
            Assert.IsFalse(Cpu.Flags.Overflow);
        }

        #endregion

        #region RRC - PARTIALLY DONE ------------------------------------------

        [Test]
        public void RotateRightThroughCarry_ShouldPerformSingleOperation()
        {
            Cpu.Flags.Carry = true;
            Cpu.Registers[1] = 44810; // 1010111100001010

            Cpu.RotateRightThroughCarry_RRC(1, 1);

            Assert.AreEqual(55173, Cpu.Registers[1]); // 1101011110000101
            Assert.IsFalse(Cpu.Flags.Carry);
        }

        #endregion

        #region MOVR - MoveRegister

        [Test]
        public void MoveRegister_ShouldPerformMove()
        {
            Cpu.Registers[1] = 7;
            Cpu.Registers[2] = 10;

            Cpu.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(7, Cpu.Registers[2]);
        }

        [Test]
        public void MoveRegister_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 7;
            Cpu.Registers[2] = 0;

            Cpu.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(7, Cpu.Registers[2]);
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void MoveRegister_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 0;
            Cpu.Registers[2] = 10;

            Cpu.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(0, Cpu.Registers[2]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void MoveRegister_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 60;
            Cpu.Registers[2] = 10;

            Cpu.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(60, Cpu.Registers[2]);
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void MoveRegister_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 32768; // 1000000000000000
            Cpu.Registers[2] = 10;

            Cpu.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(32768, Cpu.Registers[2]); // 1000000000000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        #endregion

        #region ADDR - AddRegisters

        [Test]
        public void AddRegisters_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 500;
            Cpu.Registers[2] = 250;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, Cpu.Registers[2]);
        }

        [Test]
        public void AddRegisters_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 500;
            Cpu.Registers[2] = 250;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, Cpu.Registers[2]);
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void AddRegisters_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 65530; // 1111111111111010
            Cpu.Registers[2] = 6;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(0, Cpu.Registers[2]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void AddRegisters_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 500;
            Cpu.Registers[2] = 250;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, Cpu.Registers[2]); // ______1011101110
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void AddRegisters_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 16384;
            Cpu.Registers[2] = 16384;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(32768, Cpu.Registers[2]); // 1000000000000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void AddRegisters_ShouldSetOverflowFlag()
        {
            Cpu.Registers[1] = 16384;
            Cpu.Registers[2] = 16384;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(32768, Cpu.Registers[2]); // 1000000000000000
            Assert.IsTrue(Cpu.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetOverflowFlag2()
        {
            Cpu.Registers[1] = 2;
            Cpu.Registers[2] = 3;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(5, Cpu.Registers[2]);  
            Assert.IsFalse(Cpu.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetCarryFlag()
        {
            Cpu.Registers[1] = 32770;
            Cpu.Registers[2] = 32770;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(4, Cpu.Registers[2]);  
            Assert.IsTrue(Cpu.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetCarryFlag2()
        {
            Cpu.Registers[1] = 3;
            Cpu.Registers[2] = 2;

            Cpu.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(5, Cpu.Registers[2]); 
            Assert.IsFalse(Cpu.Flags.Overflow);
        }

        #endregion

        #region SUBR - PARTIALLY DONE ------------------------------------------

        [Test]
        public void SubtractRegisters_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 4;
            Cpu.Registers[2] = 3;

            Cpu.SubtractRegisters_SUBR(2, 1);

            Assert.AreEqual(1, Cpu.Registers[1]);
        }

        #endregion

        #region INCR - IncrementRegister

        [Test]
        public void IncrementRegister_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 7;
            Cpu.IncrementRegister_INCR(1);
            Assert.AreEqual(8, Cpu.Registers[1]);
        }

        [Test]
        public void IncrementRegister_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 3453;
            Cpu.IncrementRegister_INCR(1);
            Assert.AreEqual(3454, Cpu.Registers[1]);
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void IncrementRegister_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 65535; // 1111111111111111
            Cpu.IncrementRegister_INCR(1);
            Assert.AreEqual(0, Cpu.Registers[1]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void IncrementRegister_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 32767; // _111111111111111
            Cpu.IncrementRegister_INCR(1);
            Assert.AreEqual(32768, Cpu.Registers[1]); // 1000000000000000
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void IncrementRegister_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 3; // ______________11
            Cpu.IncrementRegister_INCR(1);
            Assert.AreEqual(4, Cpu.Registers[1]); // _________________100
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        #endregion

        #region DECR - DecrementRegister

        [Test]
        public void DecrementRegister_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 7;
            Cpu.DecrementRegister_DECR(1);
            Assert.AreEqual(6, Cpu.Registers[1]);
        }

        [Test]
        public void DecrementRegister_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 3434;
            Cpu.DecrementRegister_DECR(1);
            Assert.AreEqual(3433, Cpu.Registers[1]);
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void DecrementRegister_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 1;
            Cpu.DecrementRegister_DECR(1);
            Assert.AreEqual(0, Cpu.Registers[1]);
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void DecrementRegister_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 0;
            Cpu.DecrementRegister_DECR(1);
            Assert.AreEqual(65535, Cpu.Registers[1]); // 1111111111111111
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void DecrementRegister_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 12;
            Cpu.DecrementRegister_DECR(1);
            Assert.AreEqual(11, Cpu.Registers[1]);
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        #endregion

        #region COMR - ComplementRegister

        [Test]
        public void ComplementRegister_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 9; // ____________1001

            Cpu.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, Cpu.Registers[1]); // 1111111111110110
        }

        [Test]
        public void ComplementRegister_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 9; // ____________1001

            Cpu.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, Cpu.Registers[1]); // 1111111111110110
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void ComplementRegister_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 65535; // 1111111111111111

            Cpu.ComplementRegister_COMR(1);

            Assert.AreEqual(0, Cpu.Registers[1]);  
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void ComplementRegister_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 9; // ____________1001

            Cpu.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, Cpu.Registers[1]); // 1111111111110110
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void ComplementRegister_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 65535; // 1111111111111111

            Cpu.ComplementRegister_COMR(1);

            Assert.AreEqual(0, Cpu.Registers[1]);
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        #endregion

        #region ANDR - AndRegisters

        [Test]
        public void AndRegisters_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 21; // ___________10101
            Cpu.Registers[2] = 27; // ___________11011

            Cpu.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(17, Cpu.Registers[2]); // ___________10001
        }

        [Test]
        public void AndRegisters_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 21; // ___________10101
            Cpu.Registers[2] = 27; // ___________11011

            Cpu.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(17, Cpu.Registers[2]); // ___________10001
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void AndRegisters_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 21; // ___________10101
            Cpu.Registers[2] = 10; // ____________1010

            Cpu.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(0, Cpu.Registers[2]); 
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void AndRegisters_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 65535; // 1111111111111111
            Cpu.Registers[2] = 32767; // _111111111111111

            Cpu.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(32767, Cpu.Registers[2]); // _111111111111111
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void AndRegisters_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 65535; // 1111111111111111
            Cpu.Registers[2] = 65534; // 1111111111111110

            Cpu.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(65534, Cpu.Registers[2]); // 1111111111111110
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        #endregion

        #region XORR - XorRegisters

        [Test]
        public void XorRegisters_ShouldPerformOperation()
        {
            Cpu.Registers[1] = 2570; // 101000001010
            Cpu.Registers[2] = 3087; // 110000001111

            Cpu.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, Cpu.Registers[2]); // _11000000101
            Assert.IsFalse(Cpu.Flags.Zero);
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        [Test]
        public void XorRegisters_ShouldSetZeroFlag()
        {
            Cpu.Registers[1] = 2570; // 101000001010
            Cpu.Registers[2] = 3087; // 110000001111

            Cpu.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, Cpu.Registers[2]); // _11000000101
            Assert.IsFalse(Cpu.Flags.Zero);
        }

        [Test]
        public void XorRegisters_ShouldSetZeroFlag2()
        {
            Cpu.Registers[1] = 2570; // 101000001010
            Cpu.Registers[2] = 2570; // 101000001010

            Cpu.XorRegisters_XORR(1, 2);

            Assert.AreEqual(0, Cpu.Registers[2]); 
            Assert.IsTrue(Cpu.Flags.Zero);
        }

        [Test]
        public void XorRegisters_ShouldSetSignFlag()
        {
            Cpu.Registers[1] = 32768;  // 1000000000000000
            Cpu.Registers[2] = 2; // ______________10

            Cpu.XorRegisters_XORR(1, 2);

            Assert.AreEqual(32770, Cpu.Registers[2]); // 1000000000000010
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void XorRegisters_ShouldSetSignFlag2()
        {
            Cpu.Registers[1] = 2570; // 101000001010
            Cpu.Registers[2] = 3087; // 110000001111

            Cpu.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, Cpu.Registers[2]); // _11000000101
            Assert.IsFalse(Cpu.Flags.Sign);
        }

        #endregion
    }
}
