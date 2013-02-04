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

        [Test]
        public void MoveInImmediate_DoubleByteData_Autoincrement()
        {
            //http://wiki.intellivision.us/index.php?title=Double_Byte_Data_Flag
            MasterComponent.Instance.CPU.Flags.DoubleByteData = true;
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x1234, 0x5534);
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x1235, 0x5674);
            MasterComponent.Instance.CPU.MoveInImmediate_MVII(0, 0x1234);

            Assert.AreEqual(0x7434, MasterComponent.Instance.CPU.Registers[0]);
        }

        [Test]
        public void MoveInImmediate_DoubleByteData_NoAutoincrement()
        {
            //http://wiki.intellivision.us/index.php?title=Double_Byte_Data_Flag
            MasterComponent.Instance.CPU.Flags.DoubleByteData = true;
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x1234, 0x1134);
            MasterComponent.Instance.CPU.MoveInImmediate_MVII(3, 0x1234);

            Assert.AreEqual(0x3434, MasterComponent.Instance.CPU.Registers[3]);
        }
    }
}
