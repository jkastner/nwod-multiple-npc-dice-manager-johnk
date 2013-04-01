using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public abstract class AttackTrait : NumericTrait
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


        public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            base(label, value)
        {
            this.DefenseTarget = defenseTarget;
            this.DamageType = damageType;
        }

    }
}
