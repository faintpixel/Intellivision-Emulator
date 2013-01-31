using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Intellivision
{
    public class MasterComponent 
    {

        #region singleton setup

        private static MasterComponent _instance;

        public static MasterComponent Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MasterComponent();
                return _instance;
            }
        }
      
        #endregion

        public CPU.CP1610 CPU;
        public PSG.AY_3_891x PSG;
        public Memory.MemoryMap MemoryMap;
        public STIC.AY_3_8900 STIC;

        private MasterComponent()
        {
            CPU = new CPU.CP1610();
            PSG = new PSG.AY_3_891x();
            MemoryMap = new Memory.MemoryMap();
            STIC = new STIC.AY_3_8900();
        }

        public void Start()
        {
            CPU.Registers[7] = 0x1000; // set the start position
        }
    }
}
