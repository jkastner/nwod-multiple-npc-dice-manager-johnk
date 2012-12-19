using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public abstract class DicePool
    {
        protected static Random _theRandomGenerator = new Random();
        internal abstract void Roll();

        public abstract String ResultDescription
        {
            get;
            set;
        }

    }
}
