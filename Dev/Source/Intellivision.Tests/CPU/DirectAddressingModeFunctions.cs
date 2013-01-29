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
        public MemoryMap Memory;
        public CP1610 Cpu;
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

        [Test]
        public void MoveOut_ShouldCopyValue()
        {
            Cpu.Registers[1] = 5432;

            Cpu.MoveOut_MVO(1, 0x200);

            Assert.AreEqual(5432, Memory.Read16BitsFromAddress(0x200));
        }

        [Test]
        public void MoveIn_ShouldCopyValue()
        {
            Memory.Write16BitsToAddress(0x200, 5432);

            Cpu.MoveIn_MVI(1, 0x200);

            Assert.AreEqual(5432, Cpu.Registers[1]);
        }

        [Test]
        public void Add_ShouldPerformOperation()
        {
            Memory.Write16BitsToAddress(0x200, 5);
            Cpu.Registers[1] = 3;

            Cpu.Add_ADD(1, 0x200);

            Assert.AreEqual(8, Cpu.Registers[1]);
        }

        [Test]
        public void Subtract_ShouldPerformOperation()
        {
            Memory.Write16BitsToAddress(0x200, 5);
            Cpu.Registers[1] = 13;

            Cpu.Subtract_SUB(1, 0x200);

            Assert.AreEqual(8, Cpu.Registers[1]);
        }

        [Test]
        public void Compare_ShouldPerformOperation()
        {
            Memory.Write16BitsToAddress(0x200, 13);
            Cpu.Registers[1] = 5;

            Cpu.Compare_CMP(1, 0x200);

            Assert.AreEqual(5, Cpu.Registers[1]);
            Assert.IsTrue(Cpu.Flags.Sign);
        }

        [Test]
        public void And_ShouldPerformOperation()
        {
            Memory.Write16BitsToAddress(0x200, 21); // ___________10101
            Cpu.Registers[1] = 27; // ___________11011

            Cpu.And_AND(1, 0x200);

            Assert.AreEqual(17, Cpu.Registers[1]); // ___________10001
        }

        [Test]
        public void Xor_ShouldPerformOperation()
        {
            Memory.Write16BitsToAddress(0x200, 2570); // 101000001010
            Cpu.Registers[1] = 3087; // 110000001111

            Cpu.Xor_XOR(1, 0x200);

            Assert.AreEqual(1541, Cpu.Registers[1]); // _11000000101
            Assert.IsFalse(Cpu.Flags.Zero);
            Assert.IsFalse(Cpu.Flags.Sign);
        }
    }
}
