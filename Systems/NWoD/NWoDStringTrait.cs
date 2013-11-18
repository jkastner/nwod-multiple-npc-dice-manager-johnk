using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NWoDStringTrait : StringTrait
    {
        public NWoDStringTrait(String label, String contents):
            base(label, contents)
        {
            
        }

        public override object BaseTraitContents
        {
            get { return TraitContents; }
            set { TraitContents = value.ToString(); }
        }

        public override Trait CopyTrait()
        {
            return new NWoDStringTrait(TraitLabel, TraitContents);
        }
        public override String TraitDescription
        {
            get { return TraitLabel + " - " + TraitContents; }
        }
    }
}
