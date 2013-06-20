using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class PathfinderStringTrait: StringTrait, IPathfinderTrait
    {
        public PathfinderStringTrait(String traitlabel, String contents):
            base(traitlabel, contents)
        {
        }


        public override Trait CopyTrait()
        {
            return new PathfinderStringTrait(this.TraitLabel, this.TraitContents);
        }
        public override String TraitDescription
        {
            get
            {
                return TraitLabel+" - "+TraitContents;
            }
        }
        public override object BaseTraitContents
        {
            get
            {
                return TraitContents;
            }
            set
            {
                TraitContents = value.ToString();
            }
        }


    }
}
