using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NUnit.Framework;
using Intellivision.CPU;
using Intellivision.Memory;

namespace Intellivision.Tests.CPU
{
    [TestFixture]
    public class ImpliedAddressingModeFunctions
    {
        public CP1610 Cpu;
        public MemoryMap Memory;
        public STIC.AY_3_8900 Stic;
        public PSG.AY_3_891x Psg;
        private int _haltEventsRaised;
        private int _terminateCurrentInterruptEventsRaised;

        [SetUp]
        public void Setup()
        {
            Stic = new STIC.AY_3_8900();
            Psg = new PSG.AY_3_891x();
            Memory = new MemoryMap(ref Stic, ref Psg);
            Cpu = new CP1610(ref Memory);
            Cpu.Halted_HALT += new CP1610.OutputSignalEvent(Cpu_Halted_HALT);
            Cpu.TerminateCurrentInterruprt_TCI += new CP1610.OutputSignalEvent(Cpu_TerminateCurrentInterruprt_TCI);
            _haltEventsRaised = 0;
            _terminateCurrentInterruptEventsRaised = 0;
        }

        void Cpu_TerminateCurrentInterruprt_TCI()
        {
            _terminateCurrentInterruptEventsRaised += 1;
        }

        void Cpu_Halted_HALT()
        {
            _haltEventsRaised += 1;
        }

        [Test]
        public void ClearCarry_ShouldClearFlagIfTrue()
        {
            Cpu.Flags.Carry = true;
            Cpu.ClearCarry_CLRC();
            Assert.IsFalse(Cpu.Flags.Carry);

            Cpu.Flags.Carry = false;
            Cpu.ClearCarry_CLRC();
            Assert.IsFalse(Cpu.Flags.Carry);
        }

        [Test]
        public void ClearCarry_ShouldClearFlagIfFalse()
        {
            Cpu.Flags.Carry = false;
            Cpu.ClearCarry_CLRC();
            Assert.IsFalse(Cpu.Flags.Carry);
        }

        [Test]
        public void DisableInterruptSystem_ShouldClearFlag()
        {
            Cpu.Flags.InterruptEnable = true;
            Cpu.DisableInterruptSystem_DIS();
            Assert.IsFalse(Cpu.Flags.InterruptEnable);

            Cpu.Flags.InterruptEnable = false;
            Cpu.DisableInterruptSystem_DIS();
            Assert.IsFalse(Cpu.Flags.InterruptEnable);
        }

        [Test]
        public void EnableInterruptSystem_ShouldSetFlag()
        {
            Cpu.Flags.InterruptEnable = false;
            Cpu.EnableInterruptSystem_EIS();
            Assert.IsTrue(Cpu.Flags.InterruptEnable);

            Cpu.Flags.InterruptEnable = true;
            Cpu.EnableInterruptSystem_EIS();
            Assert.IsTrue(Cpu.Flags.InterruptEnable);
        }

        [Test]
        public void Halt_ShouldRaiseEvent()
        {
            Cpu.Halt_HLT();
            Assert.AreEqual(1, _haltEventsRaised);
        }

        [Test]
        public void SetDoubleByteData_ShouldSetFlag()
        {
            Cpu.Flags.DoubleByteData = false;
            Cpu.SetDoubleByteData_SDBD();
            Assert.IsTrue(Cpu.Flags.DoubleByteData);

            Cpu.Flags.DoubleByteData = true;
            Cpu.SetDoubleByteData_SDBD();
            Assert.IsTrue(Cpu.Flags.DoubleByteData);
        }

        [Test]
        public void SetCarry_ShouldSetFlag()
        {
            Cpu.Flags.Carry = false;
            Cpu.SetCarry_SETC();
            Assert.IsTrue(Cpu.Flags.Carry);

            Cpu.Flags.Carry = true;
            Cpu.SetCarry_SETC();
            Assert.IsTrue(Cpu.Flags.Carry);
        }

        [Test]
        public void TerminateCurrentInterrupt_TCI_ShouldRaiseEvent()
        {
            Cpu.TerminateCurrentInterrupt_TCI();
            Assert.AreEqual(1, _terminateCurrentInterruptEventsRaised);
        }
    }
}
