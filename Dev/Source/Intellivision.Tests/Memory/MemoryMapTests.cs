using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Intellivision.Memory;

namespace Intellivision.Tests.Memory
{
    [TestFixture]
    public class MemoryMapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReadingFromSystemRam_ShouldRead()
        {
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x0200);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void WritingToSystemRam_ShouldWrite()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x0200, 256);
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x0200);
            Assert.AreEqual(256, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldIgnoreUpperBits()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x102, 65280); // 1111111100000000
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldWriteLowerBits()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x102, 65535); // 1111111111111111
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(255, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldWriteLowerBits2()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x102, 255); // 11111111
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(255, result);
        }

        [Test]
        public void WritingToExecutiveRom_ShouldWrite10Bits()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x1000, 1023); // 1111111111
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x1000);
            Assert.AreEqual(1023, result);
        }

        [Test]
        public void WritingToExecutiveRom_ShouldIgnoreOutsideBits()
        {
            MasterComponent.Instance.MemoryMap.Write16BitsToAddress(0x1000, 1024); // 1000000000
            UInt16 result = MasterComponent.Instance.MemoryMap.Read16BitsFromAddress(0x1000);
            Assert.AreEqual(0, result);
        }
    }
}
