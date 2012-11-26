using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NumberedTrait : Trait
    {
        public NumberedTrait(int traitValue, String traitLabel) :
            base(traitValue, traitLabel)
        {
        }

        public override Trait CopyTrait()
        {
            return new NumberedTrait(TraitValue, TraitLabel);
        }
    }

}
