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
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void MoveOutIndirect_ShouldPerformOperation()
        {
            MasterComponent.Instance.CPU.Registers[5] = 0x102B;
            MasterComponent.Instance.CPU.Registers[6] = 0x02f1;
            MasterComponent.Instance.CPU.MoveOutIndirect_MVOat(5, 6);

            UInt16 value = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x02f1);
            Assert.AreEqual(0x102B, value);
            Assert.AreEqual(0x102B, MasterComponent.Instance.CPU.Registers[5], "Expected 0x102B was 0x" + MasterComponent.Instance.CPU.Registers[5].ToString("X"));
            Assert.AreEqual(0x02f2, MasterComponent.Instance.CPU.Registers[6], "Expected 0x02F2 was 0x" + MasterComponent.Instance.CPU.Registers[6].ToString("X"));
        }

        [Test]
        public void MoveOutIndirect_ShouldPerformOperation2()
        {
            MasterComponent.Instance.CPU.Registers[4] = 0x01F0;
            MasterComponent.Instance.CPU.Registers[5] = 0x0;
            MasterComponent.Instance.CPU.MoveOutIndirect_MVOat(5, 4);

            UInt16 value = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x01F0);
            Assert.AreEqual(0x0, value);
            Assert.AreEqual(0x0, MasterComponent.Instance.CPU.Registers[5], "Expected 0x0 was 0x" + MasterComponent.Instance.CPU.Registers[5].ToString("X"));
            Assert.AreEqual(0x01F1, MasterComponent.Instance.CPU.Registers[4], "Expected 0x01F1 was 0x" + MasterComponent.Instance.CPU.Registers[4].ToString("X"));
        }

    }
}
