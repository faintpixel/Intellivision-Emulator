using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Intellivision.CPU;
using Intellivision.Memory;

namespace Intellivision.Tests.CPU
{
    [TestFixture]
    public class DirectAddressingModeFunctions
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MoveOut_ShouldCopyValue()
        {
            MasterComponent.Instance.CPU.Registers[1] = 5432;

            MasterComponent.Instance.CPU.MoveOut_MVO(1, 0x200);

            Assert.AreEqual(5432, MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x200));
        }

        [Test]
        public void MoveIn_ShouldCopyValue()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 5432);

            MasterComponent.Instance.CPU.MoveIn_MVI(1, 0x200);

            Assert.AreEqual(5432, MasterComponent.Instance.CPU.Registers[1]);
        }

        [Test]
        public void Add_ShouldPerformOperation()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 5);
            MasterComponent.Instance.CPU.Registers[1] = 3;

            MasterComponent.Instance.CPU.Add_ADD(1, 0x200);

            Assert.AreEqual(8, MasterComponent.Instance.CPU.Registers[1]);
        }

        [Test]
        public void Subtract_ShouldPerformOperation()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 5);
            MasterComponent.Instance.CPU.Registers[1] = 13;

            MasterComponent.Instance.CPU.Subtract_SUB(1, 0x200);

            Assert.AreEqual(8, MasterComponent.Instance.CPU.Registers[1]);
        }

        [Test]
        public void Compare_ShouldPerformOperation()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 13);
            MasterComponent.Instance.CPU.Registers[1] = 5;

            MasterComponent.Instance.CPU.Compare_CMP(1, 0x200);

            Assert.AreEqual(5, MasterComponent.Instance.CPU.Registers[1]);
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Sign);
        }

        [Test]
        public void And_ShouldPerformOperation()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 21); // ___________10101
            MasterComponent.Instance.CPU.Registers[1] = 27; // ___________11011

            MasterComponent.Instance.CPU.And_AND(1, 0x200);

            Assert.AreEqual(17, MasterComponent.Instance.CPU.Registers[1]); // ___________10001
        }

        [Test]
        public void Xor_ShouldPerformOperation()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x200, 2570); // 101000001010
            MasterComponent.Instance.CPU.Registers[1] = 3087; // 110000001111

            MasterComponent.Instance.CPU.Xor_XOR(1, 0x200);

            Assert.AreEqual(1541, MasterComponent.Instance.CPU.Registers[1]); // _11000000101
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Zero);
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Sign);
        }
    }
}
