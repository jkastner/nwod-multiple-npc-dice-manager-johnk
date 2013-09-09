using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class NWoDTrait : NumericIntTrait, INWoDTrait
    {
        public NWoDTrait(string traitLabel, int traitValue,
                         int explodesOn, int subtractsOn, int autoSuccesses, int succeedsOn)
            : base(traitLabel, traitValue)
        {
            ExplodesOn = explodesOn;
            SubtractsOn = subtractsOn;
            AutomaticSuccesses = autoSuccesses;
            SucceedsOn = succeedsOn;
        }

        public override string TraitDescription
        {
            get
            {
                return TraitLabel + ": " + TraitValue + " (S: " + SucceedsOn + " Exp: " + ExplodesOn + " Sub: " +
                       SubtractsOn + " Auto: " + AutomaticSuccesses + ")";
            }
        }

        [DataMember]
        public int ExplodesOn { get; set; }

        [DataMember]
        public int SubtractsOn { get; set; }

        [DataMember]
        public int SucceedsOn { get; set; }

        [DataMember]
        public int AutomaticSuccesses { get; set; }


        void INWoDTrait.AddAndChangeFromDefaults(INWoDTrait nextTrait)
        {
            TraitValue += nextTrait.TraitValue;
            if (nextTrait.ExplodesOn != 10)
                ExplodesOn = nextTrait.ExplodesOn;
            if (nextTrait.AutomaticSuccesses != 0)
                ExplodesOn = nextTrait.AutomaticSuccesses;
            if (nextTrait.SubtractsOn != 0)
                ExplodesOn = nextTrait.SubtractsOn;
        }

        public override Trait CopyTrait()
        {
            return new NWoDTrait(TraitLabel, TraitValue, ExplodesOn, SubtractsOn, AutomaticSuccesses, SucceedsOn);
        }
    }
}