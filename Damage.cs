using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public abstract class Damage
    {
        private String _damageDescriptor;

        public String DamageDescriptor
        {
            get { return _damageDescriptor; }
            set { _damageDescriptor = value; }
        }


        private int _damageValue;

        public int DamageValue
        {
            get { return _damageValue; }
            set { _damageValue = value; }
        }

        public Damage(String descriptor, int value)
        {
            DamageDescriptor = descriptor;
            DamageValue = value;

        }

        //Some damage types, like Bashing or Aggrivated in New World of Darkness, can't be summed.
        //In D&D, it's useful to know the final sum regardless of type.
        public abstract bool CanBeSummed();

    }
}
