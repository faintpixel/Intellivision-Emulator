using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Intellivision.CPU;
using Intellivision.Memory;
using System.Collections;

namespace Intellivision.Tests.CPU
{
    [TestFixture]
    public class CP1610Tests
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

        [Test]
        public void GetRegisterBits_ShouldGetBits()
        {
            Cpu.Registers[0] = 1;
            BitArray bits = Cpu.GetRegisterBits(0);
            Assert.IsTrue(bits[0]);
            for (int i = 1; i < 16; i++)
                Assert.IsFalse(bits[i]);
        }

        [Test]
        public void ConvertBitArrayToUInt16_ShouldPerformConversion()
        {
            BitArray bits = new BitArray(16);
            bits[0] = true;
            bits[1] = true;
            bits[2] = false;
            bits[3] = true;
            bits[4] = false;
            bits[5] = false;
            bits[6] = false;
            bits[7] = false;

            bits[8] = false;
            bits[9] = false;
            bits[10] = false;
            bits[11] = false;
            bits[12] = false;
            bits[13] = false;
            bits[14] = false;
            bits[15] = false;

            Assert.AreEqual(11, Cpu.ConvertBitArrayToUInt16(bits));
        }

    }
}
