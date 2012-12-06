using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public abstract class AttackTrait : Trait
    {
        private String _defenseTarget;
        public String DefenseTarget
        {
            get { return _defenseTarget; }
            set { _defenseTarget = value; }
        }

        private String _damageType;
        public String DamageType
        {
            get { return _damageType; }
            set { _damageType = value; }
        }
        
        
        public AttackTrait(int value, string label, String defenseTarget, String damageType):
            base(value, label)
        {
            this.DefenseTarget = defenseTarget;
            this.DamageType = damageType;
        }

    }
}
