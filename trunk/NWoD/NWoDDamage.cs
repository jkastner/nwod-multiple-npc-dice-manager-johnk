using System;

namespace XMLCharSheets
{
    public class NWoDDamage : Damage
    {
        public NWoDDamage(String descriptor, int value) :
            base(descriptor, value)
        {
        }

        public override bool CanBeSummed()
        {
            return false;
        }
    }
}