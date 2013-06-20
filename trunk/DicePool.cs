using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof(NWoDDicePool))]
    [KnownType(typeof(PathfinderDicePool))]
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
