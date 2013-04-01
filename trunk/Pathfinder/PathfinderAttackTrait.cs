using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderAttackTrait : AttackTrait, IPathfinderTrait
    {

        public PathfinderAttackTrait(int traitValue, String traitLabel, String defenseTarget, String damageType) :
            base(traitLabel, traitValue, defenseTarget, damageType)
        {

        }


        //traits.Add(new PathfinderAttackTrait(5, curQuery.Label, curQuery.Descriptor, curQuery.AttackBonus, curQuery.Damage, curQuery.DamageType, curQuery.Triggers, curQuery.TriggerEffect, curQuery.TargetDefense));
        public PathfinderAttackTrait(int attackValue, string label, string descriptor, string attackbonus, string damage, string damagetype, string triggers, string triggereffects, string targetDefense)
            //public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            : base(label, attackValue, targetDefense, damagetype)
        {
        
        }
        
        public override Trait CopyTrait()
        {
            return null;
        }
        public override String TraitDescription
        {
            get
            {
                return "";
            }
        }
    }
}
