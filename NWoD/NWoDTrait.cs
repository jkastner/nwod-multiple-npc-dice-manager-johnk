using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NWoDTrait : Trait, INWoDTrait
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

        internal void AddAndChangeFromDefaults(NWoDTrait nextTrait)
        {
            TraitValue += nextTrait.TraitValue;
            if (nextTrait.ExplodesOn != 10)
                this.ExplodesOn = nextTrait.ExplodesOn;
            if (nextTrait.AutomaticSuccesses != 0)
                this.ExplodesOn = nextTrait.AutomaticSuccesses;
            if (nextTrait.SubtractsOn != 0)
                this.ExplodesOn = nextTrait.SubtractsOn;

        }
    }
}
