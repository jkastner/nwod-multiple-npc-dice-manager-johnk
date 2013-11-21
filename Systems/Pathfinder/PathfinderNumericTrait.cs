using System;
using System.Runtime.Serialization;

namespace CombatAutomationTheater
{
    [DataContract(Namespace = "")]
    public class PathfinderNumericTrait : NumericIntTrait
    {
        //Depending on the trait, this could be the thing being resisted, the way to overcome the damage, or the type of damage.


        public PathfinderNumericTrait(String label, int value, String descriptor) :
            base(label, value)
        {
            Descriptor = descriptor;
        }

        [DataMember]
        public String Descriptor { get; set; }

        public override string TraitDescription
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Descriptor))
                {
                    return TraitLabel + " " + TraitValue;
                }
                else
                {
                    return TraitLabel + " " + TraitValue + " -- " + Descriptor;
                }
            }
        }

        public override Trait CopyTrait()
        {
            return new PathfinderNumericTrait(TraitLabel, TraitValue, Descriptor);
        }
    }
}