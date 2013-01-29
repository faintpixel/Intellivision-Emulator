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
    public class IndirectAddressingModeFunctions
    {
        public CP1610 Cpu;
        public MemoryMap Memory;
        public STIC.AY_3_8900 Stic;
        public PSG.AY_3_891x Psg;

        [SetUp]
        public void SetUp()
        {
            Stic = new STIC.AY_3_8900();
            Psg = new PSG.AY_3_891x();
            Memory = new MemoryMap(ref Stic, ref Psg);
            Cpu = new CP1610(ref Memory);
        }

        [Test]
        public void MoveOutIndirect_ShouldPerformOperation()
        {
            Cpu.Registers[5] = 0x102B;
            Cpu.Registers[6] = 0x02f1;
            Cpu.MoveOutIndirect_MVOat(5, 6);

            UInt16 value = Memory.Read16BitsFromAddress(0x02f1);
            Assert.AreEqual(0x102B, value);
            Assert.AreEqual(0x102B, Cpu.Registers[5], "Expected 0x102B was 0x" + Cpu.Registers[5].ToString("X"));
            Assert.AreEqual(0x02f2, Cpu.Registers[6], "Expected 0x02F2 was 0x" + Cpu.Registers[6].ToString("X"));
        }

        [Test]
        public void MoveOutIndirect_ShouldPerformOperation2()
        {
            Cpu.Registers[4] = 0x01F0;
            Cpu.Registers[5] = 0x0;
            Cpu.MoveOutIndirect_MVOat(5, 4);

            UInt16 value = Memory.Read16BitsFromAddress(0x01F0);
            Assert.AreEqual(0x0, value);
            Assert.AreEqual(0x0, Cpu.Registers[5], "Expected 0x0 was 0x" + Cpu.Registers[5].ToString("X"));
            Assert.AreEqual(0x01F1, Cpu.Registers[4], "Expected 0x01F1 was 0x" + Cpu.Registers[4].ToString("X"));
        }

    }
}
