using System;

namespace XMLCharSheets
{
    public abstract class Damage
    {
        public Damage(String descriptor, int value)
        {
            DamageDescriptor = descriptor;
            DamageValue = value;
        }

        public String DamageDescriptor { get; set; }


        public int DamageValue { get; set; }

        //Some damage types, like Bashing or Aggrivated in New World of Darkness, can't be summed.
        //In D&D, it's useful to know the final sum regardless of type.
        public abstract bool CanBeSummed();
    }
}