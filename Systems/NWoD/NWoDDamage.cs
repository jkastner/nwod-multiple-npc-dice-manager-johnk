using System;

namespace CombatAutomationTheater
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