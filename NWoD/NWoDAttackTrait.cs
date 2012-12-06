using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NWoDAttackTrait : AttackTrait, INWoDTrait
    {
        public NWoDAttackTrait(int value, string label, string defenseTarget, string damageType,
            int explodesOn, int subtractsOn, int autoSuccesses)
            : base(value, label, defenseTarget, damageType)
        {
            ExplodesOn = explodesOn;
            SubtractsOn = subtractsOn;
            autoSuccesses = AutomaticSuccesses;
        }

        public int ExplodesOn { get; set; }
        public int SubtractsOn { get; set; }
        public int AutomaticSuccesses { get; set; }

        public override Trait CopyTrait()
        {
            NWoDAttackTrait copy = new NWoDAttackTrait(TraitValue, TraitLabel, DefenseTarget, DamageType, ExplodesOn, SubtractsOn, AutomaticSuccesses);
            return copy;
        }
    }
}
