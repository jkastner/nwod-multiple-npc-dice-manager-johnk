using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof(NWoDAttackTrait))]
    [KnownType(typeof(PathfinderAttackTrait))]
    public abstract class AttackTrait : NumericIntTrait
    {
        private String _defenseTarget;
        [DataMember]
        public String DefenseTarget
        {
            get { return _defenseTarget; }
            set { _defenseTarget = value; }
        }

        private String _damageType;
        [DataMember]
        public String DamageType
        {
            get { return _damageType; }
            set { _damageType = value; }
        }


        public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            base(label, value)
        {
            this.DefenseTarget = defenseTarget;
            this.DamageType = damageType;
        }

    }
}
