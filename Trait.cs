using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public abstract class Trait
    {
        private String _traitLabel;
        public String TraitLabel
        {
            set { _traitLabel = value; }
            get { return _traitLabel; }
        }

        private int _traitValue;

        public int TraitValue
        {
            get { return _traitValue; }
            set { _traitValue = value; }
        }

        public Trait(int traitValue, String traitLabel)
        {
            _traitValue = traitValue;
            TraitLabel = traitLabel;
        }

        public override string ToString()
        {
            return TraitLabel + " -- " + TraitValue;
        }

        public abstract Trait CopyTrait();
    }
}
