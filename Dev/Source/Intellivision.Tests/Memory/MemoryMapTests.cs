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
        public MemoryMap memoryMap;
        public STIC.AY_3_8900 stic;
        public PSG.AY_3_891x psg;

        [SetUp]
        public void Setup()
        {
            stic = new STIC.AY_3_8900();
            psg = new PSG.AY_3_891x();
            memoryMap = new MemoryMap(ref stic, ref psg);
        }

        [Test]
        public void ReadingFromSystemRam_ShouldRead()
        {
            UInt16 result = memoryMap.Read16BitsFromAddress(0x0200);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void WritingToSystemRam_ShouldWrite()
        {
            memoryMap.Write16BitsToAddress(0x0200, 256);
            UInt16 result = memoryMap.Read16BitsFromAddress(0x0200);
            Assert.AreEqual(256, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldIgnoreUpperBits()
        {
            memoryMap.Write16BitsToAddress(0x102, 65280); // 1111111100000000
            UInt16 result = memoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldWriteLowerBits()
        {
            memoryMap.Write16BitsToAddress(0x102, 65535); // 1111111111111111
            UInt16 result = memoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(255, result);
        }

        [Test]
        public void WritingToScratchRam_ShouldWriteLowerBits2()
        {
            memoryMap.Write16BitsToAddress(0x102, 255); // 11111111
            UInt16 result = memoryMap.Read16BitsFromAddress(0x102);
            Assert.AreEqual(255, result);
        }

        [Test]
        public void WritingToExecutiveRom_ShouldWrite10Bits()
        {
            memoryMap.Write16BitsToAddress(0x1000, 1023); // 1111111111
            UInt16 result = memoryMap.Read16BitsFromAddress(0x1000);
            Assert.AreEqual(1023, result);
        }

        [Test]
        public void WritingToExecutiveRom_ShouldIgnoreOutsideBits()
        {
            memoryMap.Write16BitsToAddress(0x1000, 1024); // 1000000000
            UInt16 result = memoryMap.Read16BitsFromAddress(0x1000);
            Assert.AreEqual(0, result);
        }
    }
}
