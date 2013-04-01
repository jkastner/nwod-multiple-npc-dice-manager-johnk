using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderTrait: StringTrait, IPathfinderTrait
    {
        private object curQuery;
        private string p1;
        private string p2;
        private string p3;
        private string p4;


        public PathfinderTrait(String traitValue, String traitLabel):
            base(traitValue, traitLabel)
        {
        }

        //traits.Add(new PathfinderTrait(curQuery.Label, curQuery.Value, curQuery.OvercomeBy, curQuery.Descriptor));
        public PathfinderTrait(string p1, string p2, string p3, string p4)
            :base(p1, p2)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }


        public override Trait CopyTrait()
        {
            return null;
        }
        public override String TraitDescription
        {
            get
            {
                return "";
            }
        }
    }
}
