using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public abstract class NumericTrait : Trait
    {

        private int _traitValue;

        public int TraitValue
        {
            get { return _traitValue; }
            set { _traitValue = value; }
        }

        public NumericTrait(String traitLabel, int traitValue):
            base(traitLabel)
        {
            _traitValue = traitValue;
            TraitLabel = traitLabel;
        }

        public override object BaseTraitContents
        {
            get
            {
                return TraitValue;
            }
            set
            {
                if (value == null)
                    return;
                int newval = TraitValue;
                if (int.TryParse(value.ToString(), out newval))
                {
                    TraitValue = newval;
                }

            }
        }

        internal virtual void ApplyModifier(int modifier)
        {
            TraitValue += modifier;
        }
    }
}
