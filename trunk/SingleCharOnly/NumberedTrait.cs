using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NumberedTrait
    {
        private int _traitValue;

        public int TraitValue
        {
            get { return _traitValue; }
            set { _traitValue = value; }
        }


        private Trait _associatedTrait;

        public String TraitLabel
        {
            get { return _associatedTrait.TraitName; }
        }

        public NumberedTrait(int traitValue, Trait theTrait)
        {
            _traitValue = traitValue;
            _associatedTrait = theTrait;
        }

        public override string ToString()
        {
            return TraitLabel + " -- " + TraitValue;
        }
    }
}
