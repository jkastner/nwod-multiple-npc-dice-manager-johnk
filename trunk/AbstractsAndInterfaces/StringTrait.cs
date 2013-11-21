using System;
using System.Runtime.Serialization;

namespace CombatAutomationTheater
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (PathfinderStringTrait))]
    public abstract class StringTrait : Trait
    {
        private String _traitContents;

        public StringTrait(String label, String contents) :
            base(label)
        {
            TraitContents = contents;
        }

        [DataMember]
        public String TraitContents
        {
            get { return _traitContents; }
            set
            {
                _traitContents = value;
                OnPropertyChanged("TraitContents");
                OnPropertyChanged("TraitDescription");
            }
        }
    }
}