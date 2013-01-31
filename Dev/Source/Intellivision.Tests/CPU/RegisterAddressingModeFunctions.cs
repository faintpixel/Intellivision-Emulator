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

        [SetUp]
        public void Setup()
        {
        }

        #region GSWD - GetStatusWord

        [Test]
        public void GetStatusWord_ShouldSetRegister()
        {
            MasterComponent.Instance.CPU.Flags.Sign = true;
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Flags.Overflow = true;
            MasterComponent.Instance.CPU.Flags.Zero = true;

            MasterComponent.Instance.CPU.GetStatusWord_GSWD(0);
            Assert.AreEqual(61680, MasterComponent.Instance.CPU.Registers[0]); // 1111000011110000
        }

        [Test]
        public void GetStatusWord_ShouldSetRegister2()
        {
            MasterComponent.Instance.CPU.Flags.Sign = true;
            MasterComponent.Instance.CPU.Flags.Zero = false;
            MasterComponent.Instance.CPU.Flags.Overflow = true;
            MasterComponent.Instance.CPU.Flags.Carry = false;

            MasterComponent.Instance.CPU.GetStatusWord_GSWD(1);
            Assert.AreEqual(41120, MasterComponent.Instance.CPU.Registers[1]); // 1010000010100000
        }

        #endregion

        #region RSWD - ReturnStatusWord

        [Test]
        public void ReturnStatusWord_ShouldSetRegister()
        {
            MasterComponent.Instance.CPU.Registers[1] = 240; // ________11110000

            MasterComponent.Instance.CPU.ReturnStatusWord_RSWD(1);

            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Overflow);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ReturnStatusWord_ShouldSetRegister2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 0;

            MasterComponent.Instance.CPU.ReturnStatusWord_RSWD(1);

            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
        }

        #endregion

        #region SWAP - SwapBytes

        [Test]
        public void SwapBytes_ShouldPerformDoubleSwap()
        {
            MasterComponent.Instance.CPU.Registers[2] = 255; // ________11111111

            MasterComponent.Instance.CPU.SwapBytes_SWAP(true, 2);

            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.AreEqual(65535, MasterComponent.Instance.CPU.Registers[2]); // 1111111111111111
        }

        [Test]
        public void SwapBytes_ShouldPerformSingleSwap()
        {
            MasterComponent.Instance.CPU.Registers[2] = 255; // ________11111111

            MasterComponent.Instance.CPU.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(65280, MasterComponent.Instance.CPU.Registers[2]); // 1111111100000000
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign, "Sign flag");
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero, "Zero flag");            
        }

        [Test]
        public void SwapBytes_ShouldPerformSingleSwap2()
        {
            MasterComponent.Instance.CPU.Registers[2] = 52415; // 1100110010111111

            MasterComponent.Instance.CPU.SwapBytes_SWAP(false, 2);

            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.AreEqual(49100, MasterComponent.Instance.CPU.Registers[2]); // 1011111111001100
        }

        [Test]
        public void SwapBytes_ShouldUpdateSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[2] = 255; // ________11111111

            MasterComponent.Instance.CPU.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(65280, MasterComponent.Instance.CPU.Registers[2]); // 1111111100000000
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign, "Sign flag");
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero, "Zero flag"); 
        }

        [Test]
        public void SwapBytes_ShouldUpdateSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[2] = 65280; // 1111111100000000 

            MasterComponent.Instance.CPU.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(255, MasterComponent.Instance.CPU.Registers[2]); // ________11111111
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign, "Sign flag");
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero, "Zero flag");
        }

        [Test]
        public void SwapBytes_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[2] = 0; 

            MasterComponent.Instance.CPU.SwapBytes_SWAP(false, 2);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[2]); 
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign, "Sign flag");
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero, "Zero flag");    
        }

        #endregion

        #region SLL - ShiftLogicalLeft

        [Test]
        public void ShiftLogicalLeft_ShouldPerformSingleShift()
        {
            MasterComponent.Instance.CPU.Registers[0] = 10; // ____________1010

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(20, MasterComponent.Instance.CPU.Registers[0]); // __________10100
        }

        [Test]
        public void ShiftLogicalLeft_ShouldPerformDoubleShift()
        {
            MasterComponent.Instance.CPU.Registers[0] = 10; // ____________1010

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(2, 0);

            Assert.AreEqual(40, MasterComponent.Instance.CPU.Registers[0]); // _________101000    
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[0] = 48896; // 1011111100000000

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(32256, MasterComponent.Instance.CPU.Registers[0]); // 0111111000000000

            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[0] = 65280; // 1111111100000000

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(65024, MasterComponent.Instance.CPU.Registers[0]); // 1111111000000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[0] = 65280; // 1111111100000000

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(1, 0);

            Assert.AreEqual(65024, MasterComponent.Instance.CPU.Registers[0]); // 1111111000000000            
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeft_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[0] = 0;

            MasterComponent.Instance.CPU.ShiftLogicalLeft_SLL(2, 0);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[0]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        #endregion

        #region RLC - RotateLeftThroughCarry

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformSingleRotate()
        {
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
                                
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16705, MasterComponent.Instance.CPU.Registers[1]); // _100000101000001
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformSingleRotate2()
        {
            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16704, MasterComponent.Instance.CPU.Registers[1]); // _100000101000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate()
        {
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Flags.Overflow = true;

            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33411, MasterComponent.Instance.CPU.Registers[1]); // 1000001010000011
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry, "carry");
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow, "overflow");
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate2()
        {
            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.Flags.Overflow = false;

            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33408, MasterComponent.Instance.CPU.Registers[1]); // 1000001010000000
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate3()
        {
            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.Flags.Overflow = true;

            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33409, MasterComponent.Instance.CPU.Registers[1]); // 1000001010000001
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldPerformDoubleRotate4()
        {
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Flags.Overflow = false;

            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
                         
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(2, 1);

            Assert.AreEqual(33410, MasterComponent.Instance.CPU.Registers[1]); // 1000001010000010
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16705, MasterComponent.Instance.CPU.Registers[1]); // _100000101000001
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 32768; // 1000000000000000
            //                    
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]); 
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 41120; // 1010000010100000
                             
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(16704, MasterComponent.Instance.CPU.Registers[1]); // _100000101000000
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void RotateLeftThroughCarry_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 57504; // 1110000010100000
                  
            MasterComponent.Instance.CPU.RotateLeftThroughCarry_RLC(1, 1);

            Assert.AreEqual(49472, MasterComponent.Instance.CPU.Registers[1]); // 1100000101000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region SLLC - ShiftLogicalLeftThroughCarry

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldPerformSingleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, MasterComponent.Instance.CPU.Registers[1]); // _101111000010100
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldPerformDoubleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, MasterComponent.Instance.CPU.Registers[1]); // 1011110000101000
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetOverflowFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, MasterComponent.Instance.CPU.Registers[1]); // 1011110000101000
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetOverflowFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 61194; // 1110111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(2, 1);

            Assert.AreEqual(48168, MasterComponent.Instance.CPU.Registers[1]); // 1011110000101000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Overflow);
        }
        
        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetCarryFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, MasterComponent.Instance.CPU.Registers[1]); // _101111000010100
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetCarryFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, MasterComponent.Instance.CPU.Registers[1]); // __101111000010100
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, MasterComponent.Instance.CPU.Registers[1]); // __101111000010100
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 0;

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(24084, MasterComponent.Instance.CPU.Registers[1]); // __101111000010100
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalLeftThroughCarry_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 61194; // 1110111100001010

            MasterComponent.Instance.CPU.ShiftLogicalLeftThroughCarry_SLLC(1, 1);

            Assert.AreEqual(56852, MasterComponent.Instance.CPU.Registers[1]); // 1101111000010100
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }



        #endregion

        #region SLR - ShiftLogicalRight

        [Test]
        public void ShiftLogicalRight_ShouldPerformSingleShift()
        {
            MasterComponent.Instance.CPU.Registers[0] = 13; // ____________1101

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(6, MasterComponent.Instance.CPU.Registers[0]); // _____________110
        }

        [Test]
        public void ShiftLogicalRight_ShouldPerformDoubleShift()
        {
            MasterComponent.Instance.CPU.Registers[0] = 65531; // 1111111111111011

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(16382, MasterComponent.Instance.CPU.Registers[0]); // 0011111111111110
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[0] = 65531; // 1111111111111011

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(16382, MasterComponent.Instance.CPU.Registers[0]); // 0011111111111110
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[0] = 0;

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(2, 0);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[0]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[0] = 256; // _______100000000

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(128, MasterComponent.Instance.CPU.Registers[0]); // ________10000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftLogicalRight_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[0] = 4; // 100

            MasterComponent.Instance.CPU.ShiftLogicalRight_SLR(1, 0);

            Assert.AreEqual(2, MasterComponent.Instance.CPU.Registers[0]); // 10
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region SAR - ShiftArithmeticRight

        [Test]
        public void ShiftArithmeticRight_ShouldPerformSingleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 43690; // 1010101010101010

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, MasterComponent.Instance.CPU.Registers[1]); // 1101010101010101
        }

        [Test]
        public void ShiftArithmeticRight_ShouldPerformDoubleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 43690; // 1010101010101010

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(2, 1);

            Assert.AreEqual(60074, MasterComponent.Instance.CPU.Registers[1]); // 1110101010101010
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 43690; // 1010101010101010

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, MasterComponent.Instance.CPU.Registers[1]); // 1101010101010101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 1; // _______________1

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]); 
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 43690; // 10101010 10101010

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54613, MasterComponent.Instance.CPU.Registers[1]); // 1101010101010101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRight_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 43946; // 1010101110101010

            MasterComponent.Instance.CPU.ShiftArithmeticRight_SAR(1, 1);

            Assert.AreEqual(54741, MasterComponent.Instance.CPU.Registers[1]); // 1101010111010101
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }
        
        #endregion

        #region SARC - ShiftArithmeticRightThroughCarry

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformSingleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(1, 1);

            Assert.AreEqual(55173, MasterComponent.Instance.CPU.Registers[1]); // 1101011110000101
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformSingleShift2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 12042; // __10111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(1, 1);

            Assert.AreEqual(6021, MasterComponent.Instance.CPU.Registers[1]); // ___1011110000101
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformDoubleShift()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, MasterComponent.Instance.CPU.Registers[1]); // 1110101111000010
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldPerformDoubleShift2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 28426; // _110111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(7106, MasterComponent.Instance.CPU.Registers[1]); // ___1101111000010
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, MasterComponent.Instance.CPU.Registers[1]); // 1110101111000010
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 0; 

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]); 
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60354, MasterComponent.Instance.CPU.Registers[1]); // 1110 1011 1100 0010
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44299; // 1010110100001011

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, MasterComponent.Instance.CPU.Registers[1]); // 1110101101000010
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetCarryFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44299; // 1010110100001011

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, MasterComponent.Instance.CPU.Registers[1]); // 1110101101000010
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetCarryFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44298; // 1010110100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, MasterComponent.Instance.CPU.Registers[1]); // 1110101101000010
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetOverflowFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44298; // 1010110100001010

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, MasterComponent.Instance.CPU.Registers[1]); // 1110101101000010
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        [Test]
        public void ShiftArithmeticRightThroughCarry_ShouldSetOverflowFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 44296; // 1010110100001000

            MasterComponent.Instance.CPU.ShiftArtithmeticRightThroughCarry_SARC(2, 1);

            Assert.AreEqual(60226, MasterComponent.Instance.CPU.Registers[1]); // 1110101101000010
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        #endregion

        #region RRC - PARTIALLY DONE ------------------------------------------

        [Test]
        public void RotateRightThroughCarry_ShouldPerformSingleOperation()
        {
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.Registers[1] = 44810; // 1010111100001010

            MasterComponent.Instance.CPU.RotateRightThroughCarry_RRC(1, 1);

            Assert.AreEqual(55173, MasterComponent.Instance.CPU.Registers[1]); // 1101011110000101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
        }

        #endregion

        #region MOVR - MoveRegister

        [Test]
        public void MoveRegister_ShouldPerformMove()
        {
            MasterComponent.Instance.CPU.Registers[1] = 7;
            MasterComponent.Instance.CPU.Registers[2] = 10;

            MasterComponent.Instance.CPU.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(7, MasterComponent.Instance.CPU.Registers[2]);
        }

        [Test]
        public void MoveRegister_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 7;
            MasterComponent.Instance.CPU.Registers[2] = 0;

            MasterComponent.Instance.CPU.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(7, MasterComponent.Instance.CPU.Registers[2]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void MoveRegister_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 0;
            MasterComponent.Instance.CPU.Registers[2] = 10;

            MasterComponent.Instance.CPU.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[2]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void MoveRegister_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 60;
            MasterComponent.Instance.CPU.Registers[2] = 10;

            MasterComponent.Instance.CPU.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(60, MasterComponent.Instance.CPU.Registers[2]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void MoveRegister_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 32768; // 1000000000000000
            MasterComponent.Instance.CPU.Registers[2] = 10;

            MasterComponent.Instance.CPU.MoveRegister_MOVR(1, 2);

            Assert.AreEqual(32768, MasterComponent.Instance.CPU.Registers[2]); // 1000000000000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region ADDR - AddRegisters

        [Test]
        public void AddRegisters_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 500;
            MasterComponent.Instance.CPU.Registers[2] = 250;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, MasterComponent.Instance.CPU.Registers[2]);
        }

        [Test]
        public void AddRegisters_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 500;
            MasterComponent.Instance.CPU.Registers[2] = 250;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, MasterComponent.Instance.CPU.Registers[2]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void AddRegisters_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65530; // 1111111111111010
            MasterComponent.Instance.CPU.Registers[2] = 6;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[2]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void AddRegisters_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 500;
            MasterComponent.Instance.CPU.Registers[2] = 250;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(750, MasterComponent.Instance.CPU.Registers[2]); // ______1011101110
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void AddRegisters_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 16384;
            MasterComponent.Instance.CPU.Registers[2] = 16384;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(32768, MasterComponent.Instance.CPU.Registers[2]); // 1000000000000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void AddRegisters_ShouldSetOverflowFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 16384;
            MasterComponent.Instance.CPU.Registers[2] = 16384;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(32768, MasterComponent.Instance.CPU.Registers[2]); // 1000000000000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetOverflowFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 2;
            MasterComponent.Instance.CPU.Registers[2] = 3;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(5, MasterComponent.Instance.CPU.Registers[2]);  
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetCarryFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 32770;
            MasterComponent.Instance.CPU.Registers[2] = 32770;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(4, MasterComponent.Instance.CPU.Registers[2]);  
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        [Test]
        public void AddRegisters_ShouldSetCarryFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 3;
            MasterComponent.Instance.CPU.Registers[2] = 2;

            MasterComponent.Instance.CPU.AddRegisters_ADDR(1, 2);

            Assert.AreEqual(5, MasterComponent.Instance.CPU.Registers[2]); 
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Overflow);
        }

        #endregion

        #region SUBR - PARTIALLY DONE ------------------------------------------

        [Test]
        public void SubtractRegisters_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 4;
            MasterComponent.Instance.CPU.Registers[2] = 3;

            MasterComponent.Instance.CPU.SubtractRegisters_SUBR(2, 1);

            Assert.AreEqual(1, MasterComponent.Instance.CPU.Registers[1]);
        }

        #endregion

        #region INCR - IncrementRegister

        [Test]
        public void IncrementRegister_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 7;
            MasterComponent.Instance.CPU.IncrementRegister_INCR(1);
            Assert.AreEqual(8, MasterComponent.Instance.CPU.Registers[1]);
        }

        [Test]
        public void IncrementRegister_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 3453;
            MasterComponent.Instance.CPU.IncrementRegister_INCR(1);
            Assert.AreEqual(3454, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void IncrementRegister_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65535; // 1111111111111111
            MasterComponent.Instance.CPU.IncrementRegister_INCR(1);
            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void IncrementRegister_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 32767; // _111111111111111
            MasterComponent.Instance.CPU.IncrementRegister_INCR(1);
            Assert.AreEqual(32768, MasterComponent.Instance.CPU.Registers[1]); // 1000000000000000
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void IncrementRegister_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 3; // ______________11
            MasterComponent.Instance.CPU.IncrementRegister_INCR(1);
            Assert.AreEqual(4, MasterComponent.Instance.CPU.Registers[1]); // _________________100
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region DECR - DecrementRegister

        [Test]
        public void DecrementRegister_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 7;
            MasterComponent.Instance.CPU.DecrementRegister_DECR(1);
            Assert.AreEqual(6, MasterComponent.Instance.CPU.Registers[1]);
        }

        [Test]
        public void DecrementRegister_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 3434;
            MasterComponent.Instance.CPU.DecrementRegister_DECR(1);
            Assert.AreEqual(3433, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void DecrementRegister_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 1;
            MasterComponent.Instance.CPU.DecrementRegister_DECR(1);
            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void DecrementRegister_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 0;
            MasterComponent.Instance.CPU.DecrementRegister_DECR(1);
            Assert.AreEqual(65535, MasterComponent.Instance.CPU.Registers[1]); // 1111111111111111
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void DecrementRegister_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 12;
            MasterComponent.Instance.CPU.DecrementRegister_DECR(1);
            Assert.AreEqual(11, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region COMR - ComplementRegister

        [Test]
        public void ComplementRegister_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 9; // ____________1001

            MasterComponent.Instance.CPU.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, MasterComponent.Instance.CPU.Registers[1]); // 1111111111110110
        }

        [Test]
        public void ComplementRegister_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 9; // ____________1001

            MasterComponent.Instance.CPU.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, MasterComponent.Instance.CPU.Registers[1]); // 1111111111110110
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ComplementRegister_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65535; // 1111111111111111

            MasterComponent.Instance.CPU.ComplementRegister_COMR(1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]);  
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void ComplementRegister_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 9; // ____________1001

            MasterComponent.Instance.CPU.ComplementRegister_COMR(1);

            Assert.AreEqual(65526, MasterComponent.Instance.CPU.Registers[1]); // 1111111111110110
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void ComplementRegister_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65535; // 1111111111111111

            MasterComponent.Instance.CPU.ComplementRegister_COMR(1);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region ANDR - AndRegisters

        [Test]
        public void AndRegisters_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 21; // ___________10101
            MasterComponent.Instance.CPU.Registers[2] = 27; // ___________11011

            MasterComponent.Instance.CPU.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(17, MasterComponent.Instance.CPU.Registers[2]); // ___________10001
        }

        [Test]
        public void AndRegisters_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 21; // ___________10101
            MasterComponent.Instance.CPU.Registers[2] = 27; // ___________11011

            MasterComponent.Instance.CPU.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(17, MasterComponent.Instance.CPU.Registers[2]); // ___________10001
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void AndRegisters_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 21; // ___________10101
            MasterComponent.Instance.CPU.Registers[2] = 10; // ____________1010

            MasterComponent.Instance.CPU.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[2]); 
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void AndRegisters_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65535; // 1111111111111111
            MasterComponent.Instance.CPU.Registers[2] = 32767; // _111111111111111

            MasterComponent.Instance.CPU.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(32767, MasterComponent.Instance.CPU.Registers[2]); // _111111111111111
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void AndRegisters_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 65535; // 1111111111111111
            MasterComponent.Instance.CPU.Registers[2] = 65534; // 1111111111111110

            MasterComponent.Instance.CPU.AndRegisters_ANDR(1, 2);

            Assert.AreEqual(65534, MasterComponent.Instance.CPU.Registers[2]); // 1111111111111110
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion

        #region XORR - XorRegisters

        [Test]
        public void XorRegisters_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[1] = 2570; // 101000001010
            MasterComponent.Instance.CPU.Registers[2] = 3087; // 110000001111

            MasterComponent.Instance.CPU.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, MasterComponent.Instance.CPU.Registers[2]); // _11000000101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void XorRegisters_ShouldSetZeroFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 2570; // 101000001010
            MasterComponent.Instance.CPU.Registers[2] = 3087; // 110000001111

            MasterComponent.Instance.CPU.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, MasterComponent.Instance.CPU.Registers[2]); // _11000000101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void XorRegisters_ShouldSetZeroFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 2570; // 101000001010
            MasterComponent.Instance.CPU.Registers[2] = 2570; // 101000001010

            MasterComponent.Instance.CPU.XorRegisters_XORR(1, 2);

            Assert.AreEqual(0, MasterComponent.Instance.CPU.Registers[2]); 
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Zero);
        }

        [Test]
        public void XorRegisters_ShouldSetSignFlag()
        {
            MasterComponent.Instance.CPU.Registers[1] = 32768;  // 1000000000000000
            MasterComponent.Instance.CPU.Registers[2] = 2; // ______________10

            MasterComponent.Instance.CPU.XorRegisters_XORR(1, 2);

            Assert.AreEqual(32770, MasterComponent.Instance.CPU.Registers[2]); // 1000000000000010
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void XorRegisters_ShouldSetSignFlag2()
        {
            MasterComponent.Instance.CPU.Registers[1] = 2570; // 101000001010
            MasterComponent.Instance.CPU.Registers[2] = 3087; // 110000001111

            MasterComponent.Instance.CPU.XorRegisters_XORR(1, 2);

            Assert.AreEqual(1541, MasterComponent.Instance.CPU.Registers[2]); // _11000000101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }

        #endregion
    }
}
