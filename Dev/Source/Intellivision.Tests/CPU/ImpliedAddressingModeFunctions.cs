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
        private int _haltEventsRaised;
        private int _terminateCurrentInterruptEventsRaised;

        [SetUp]
        public void Setup()
        {
            MasterComponent.Instance.CPU.Halted_HALT += new CP1610.OutputSignalEvent(Cpu_Halted_HALT);
            MasterComponent.Instance.CPU.TerminateCurrentInterruprt_TCI += new CP1610.OutputSignalEvent(Cpu_TerminateCurrentInterruprt_TCI);
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
            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.ClearCarry_CLRC();
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);

            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.ClearCarry_CLRC();
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void ClearCarry_ShouldClearFlagIfFalse()
        {
            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.ClearCarry_CLRC();
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void DisableInterruptSystem_ShouldClearFlag()
        {
            MasterComponent.Instance.CPU.Flags.InterruptEnable = true;
            MasterComponent.Instance.CPU.DisableInterruptSystem_DIS();
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.InterruptEnable);

            MasterComponent.Instance.CPU.Flags.InterruptEnable = false;
            MasterComponent.Instance.CPU.DisableInterruptSystem_DIS();
            Assert.IsFalse(MasterComponent.Instance.CPU.Flags.InterruptEnable);
        }

        [Test]
        public void EnableInterruptSystem_ShouldSetFlag()
        {
            MasterComponent.Instance.CPU.Flags.InterruptEnable = false;
            MasterComponent.Instance.CPU.EnableInterruptSystem_EIS();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.InterruptEnable);

            MasterComponent.Instance.CPU.Flags.InterruptEnable = true;
            MasterComponent.Instance.CPU.EnableInterruptSystem_EIS();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.InterruptEnable);
        }

        [Test]
        public void Halt_ShouldRaiseEvent()
        {
            MasterComponent.Instance.CPU.Halt_HLT();
            Assert.AreEqual(1, _haltEventsRaised);
        }

        [Test]
        public void SetDoubleByteData_ShouldSetFlag()
        {
            MasterComponent.Instance.CPU.Flags.DoubleByteData = false;
            MasterComponent.Instance.CPU.SetDoubleByteData_SDBD();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.DoubleByteData);

            MasterComponent.Instance.CPU.Flags.DoubleByteData = true;
            MasterComponent.Instance.CPU.SetDoubleByteData_SDBD();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.DoubleByteData);
        }

        [Test]
        public void SetCarry_ShouldSetFlag()
        {
            MasterComponent.Instance.CPU.Flags.Carry = false;
            MasterComponent.Instance.CPU.SetCarry_SETC();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);

            MasterComponent.Instance.CPU.Flags.Carry = true;
            MasterComponent.Instance.CPU.SetCarry_SETC();
            Assert.IsTrue(MasterComponent.Instance.CPU.Flags.Carry);
        }

        [Test]
        public void TerminateCurrentInterrupt_TCI_ShouldRaiseEvent()
        {
            MasterComponent.Instance.CPU.TerminateCurrentInterrupt_TCI();
            Assert.AreEqual(1, _terminateCurrentInterruptEventsRaised);
        }
    }
}
