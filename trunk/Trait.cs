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

        public Trait(String traitLabel)
        {
            TraitLabel = traitLabel;
        }

        public override string ToString()
        {
            return TraitDescription;
        }

        public abstract Trait CopyTrait();
        public abstract String TraitDescription
        {
            get;
        }

        public abstract object BaseTraitContents
        {
            get;
            set;
        }

    }
}
