using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    class NWoDTrait : Trait, INWoDTrait
    {
        public NWoDTrait(int traitValue, string traitLabel, 
            int explodesOn, int subtractsOn, int autoSuccesses)
            : base(traitValue, traitLabel)
        {
            ExplodesOn = explodesOn;
            SubtractsOn = subtractsOn;
            AutomaticSuccesses = autoSuccesses;
        }

        public int ExplodesOn { get; set; }
        public int SubtractsOn { get; set; }
        public int AutomaticSuccesses { get; set; }
    }
}
