using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class NWoDDamage : Damage
    {
        public NWoDDamage(String descriptor, int value):
            base(descriptor, value)
        {
        }

        public override bool CanBeSummed()
        {
            return false;
        }


    }
}
