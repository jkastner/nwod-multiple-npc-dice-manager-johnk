using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NWoDTrait : Trait, INWoDTrait
    {
        public NWoDTrait(int traitValue, string traitLabel, 
            int explodesOn, int subtractsOn, int autoSuccesses, int succeedsOn)
            : base(traitValue, traitLabel)
        {
            ExplodesOn = explodesOn;
            SubtractsOn = subtractsOn;
            AutomaticSuccesses = autoSuccesses;
            SucceedsOn = succeedsOn;
        }

        public int ExplodesOn { get; set; }
        public int SubtractsOn { get; set; }
        public int SucceedsOn { get; set; }
        public int AutomaticSuccesses { get; set; }
        

        void INWoDTrait.AddAndChangeFromDefaults(INWoDTrait nextTrait)
        {
            TraitValue += nextTrait.TraitValue;
            if (nextTrait.ExplodesOn != 10)
                this.ExplodesOn = nextTrait.ExplodesOn;
            if (nextTrait.AutomaticSuccesses != 0)
                this.ExplodesOn = nextTrait.AutomaticSuccesses;
            if (nextTrait.SubtractsOn != 0)
                this.ExplodesOn = nextTrait.SubtractsOn;

        }
        public override Trait CopyTrait()
        {
            return new NWoDTrait(TraitValue, TraitLabel, ExplodesOn, SubtractsOn, AutomaticSuccesses, SucceedsOn);
        }

        public override string TraitDescription
        {
            get
            {
                return TraitLabel+": "+TraitValue+" (S: "+SucceedsOn+" Exp: "+ExplodesOn+" Sub: "+SubtractsOn+" Auto: "+AutomaticSuccesses+")";
            }
        }
    }
}
