using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof(PathfinderNumericTrait))]
    [KnownType(typeof(NWoDTrait))]
    public abstract class NumericIntTrait : Trait
    {

        private int _traitValue;

        [DataMember]
        public int TraitValue
        {
            get { return _traitValue; }
            set 
            { 
                _traitValue = value;
                OnPropertyChanged("TraitDescription");
                OnPropertyChanged("TraitValue");
            
            }
        }

        public NumericIntTrait(String traitLabel, int traitValue):
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
