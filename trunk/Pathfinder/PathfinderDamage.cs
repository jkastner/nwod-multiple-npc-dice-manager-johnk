using System;

namespace XMLCharSheets
{
    public class PathfinderDamage : Damage
    {
        public PathfinderDamage(String descriptor, int value) :
            base(descriptor, value)
        {
        }

        public override bool CanBeSummed()
        {
            return true;
        }
    }
}