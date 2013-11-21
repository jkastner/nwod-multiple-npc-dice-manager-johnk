using System;

namespace CombatAutomationTheater
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