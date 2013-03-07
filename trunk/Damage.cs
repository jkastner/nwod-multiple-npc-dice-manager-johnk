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
        
    }
}
