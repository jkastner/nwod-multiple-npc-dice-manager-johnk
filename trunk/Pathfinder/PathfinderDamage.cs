using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderDamage : Damage
    {
        public PathfinderDamage(String descriptor, int value):
            base(descriptor, value)
        {

        }
        public override  bool CanBeSummed()
        {
            return true;
        }
    }
}
