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
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Test()
        {
            MasterComponent.Instance.CPU.Registers[4] = 0;

            MasterComponent.Instance.CPU.MoveInImmediate_MVII(4, 0x01f0);
            Assert.AreEqual(0x01F0, MasterComponent.Instance.CPU.Registers[4]);
        }
    }
}
