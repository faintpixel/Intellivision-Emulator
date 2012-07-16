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

        [SetUp]
        public void SetUp()
        {
            Memory = new MemoryMap();
            Cpu = new CP1610(ref Memory);
        }
    }
}
