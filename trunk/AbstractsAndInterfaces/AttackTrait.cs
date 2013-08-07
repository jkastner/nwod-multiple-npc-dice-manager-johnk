using System;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (NWoDAttackTrait))]
    [KnownType(typeof (PathfinderAttackTrait))]
    public abstract class AttackTrait : NumericIntTrait
    {
        public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            base(label, value)
        {
            DefenseTarget = defenseTarget;
            DamageType = damageType;
        }

        [DataMember]
        public String DefenseTarget { get; set; }

        [DataMember]
        public String DamageType { get; set; }
    }
}