using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public abstract class StringTrait : Trait
    {
        private String _traitContents;

        public String TraitContents
        {
            get { return _traitContents; }
            set { _traitContents = value; }
        }

        public StringTrait(String label, String contents) :
            base(label)
        {
            this.TraitContents = contents;
        }

        

    }
}
