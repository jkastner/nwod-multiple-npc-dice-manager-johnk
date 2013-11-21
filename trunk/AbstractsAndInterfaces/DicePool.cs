using System;
using System.Runtime.Serialization;

namespace CombatAutomationTheater
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (NWoDDicePool))]
    [KnownType(typeof (PathfinderDicePool))]
    public abstract class DicePool
    {
        protected static Random _theRandomGenerator = new Random();

        public abstract String ResultDescription { get; set; }
        internal abstract void Roll();
    }
}