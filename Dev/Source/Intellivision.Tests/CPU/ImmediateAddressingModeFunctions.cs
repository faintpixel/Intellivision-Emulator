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
    public class ImmediateAddressingModeFunctions
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
        public void Test()
        {
            Cpu.Registers[4] = 0;

            Cpu.MoveInImmediate_MVII(4, 0x01f0);
            Assert.AreEqual(0x01F0, Cpu.Registers[4]);
        }
    }
}
