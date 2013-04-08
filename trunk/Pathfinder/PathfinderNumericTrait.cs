using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderNumericTrait : NumericTrait
    {
        //Depending on the trait, this could be the thing being resisted, the way to overcome the damage, or the type of damage.
        private String _descriptor;
        public String Descriptor
        {
            get { return _descriptor; }
            set { _descriptor = value; }
        }
        
        
        public PathfinderNumericTrait(String label, int value, String descriptor):
            base(label, value)
        {
            this._descriptor = descriptor;
        }

        public override Trait CopyTrait()
        {
            return new PathfinderNumericTrait(this.TraitLabel, this.TraitValue, this.Descriptor);
        }

        public override string TraitDescription
        {
            get 
            {
                if (String.IsNullOrWhiteSpace(Descriptor))
                {
                    return TraitLabel + " " + TraitValue;
                }
                else
                {
                    return TraitLabel + " " + TraitValue + " -- " + Descriptor;
                }
            }
        }
    }
}
