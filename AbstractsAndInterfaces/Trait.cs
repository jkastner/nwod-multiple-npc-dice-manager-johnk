using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CombatAutomationTheater
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (NWoDAttackTrait))]
    [KnownType(typeof (NumericIntTrait))]
    [KnownType(typeof (StringTrait))]
    [KnownType(typeof (PathfinderAttackTrait))]
    [KnownType(typeof (PathfinderStringTrait))]
    [KnownType(typeof (NWoDTrait))]
    public abstract class Trait : INotifyPropertyChanged
    {
        public Trait(String traitLabel)
        {
            TraitLabel = traitLabel;
        }

        [DataMember]
        public String TraitLabel { get; protected set; }

        public abstract String TraitDescription { get; }

        public abstract object BaseTraitContents { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return TraitDescription;
        }

        public abstract Trait CopyTrait();

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}