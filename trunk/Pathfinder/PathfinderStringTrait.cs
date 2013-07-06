using System;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class PathfinderStringTrait : StringTrait, IPathfinderTrait
    {
        public PathfinderStringTrait(String traitlabel, String contents) :
            base(traitlabel, contents)
        {
        }


        public override String TraitDescription
        {
            get { return TraitLabel + " - " + TraitContents; }
        }

        public override object BaseTraitContents
        {
            get { return TraitContents; }
            set { TraitContents = value.ToString(); }
        }

        public override Trait CopyTrait()
        {
            return new PathfinderStringTrait(TraitLabel, TraitContents);
        }
    }
}