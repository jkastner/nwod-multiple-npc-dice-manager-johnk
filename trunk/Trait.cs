using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class Trait
    {
        private String _traitName;

        public String TraitName
        {
            get { return _traitName; }
            set { _traitName = value; }
        }

        public Trait(string traitName)
        {
            _traitName = traitName;
        }

        public override bool Equals(object obj)
        {
            Trait other = obj as Trait;
            if (other == null)
                return false;
            return other.TraitName.Equals(TraitName);
        }
    }
}
