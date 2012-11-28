using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    internal class NWoDVampire : NWoDCharacter
    {
        public NWoDVampire(string characterName, List<Trait> traits)
            : base(characterName, traits)
        {
            IsVampire = true;
        }

        private int _vitae;

        public int CurrentVitae
        {
            get { return _vitae; }
            set
            { 
                _vitae = value;
                OnPropertyChanged("CurrentVitae");
            }
        }
        private int _maxVitae;

        public int MaxVitae
        {
            get { return _maxVitae; }
            set
            {
                _maxVitae = value;
                OnPropertyChanged("MaxVitae");
            }
        }

        internal override CharacterSheet Copy(String newName)
        {
            List<Trait> copyTraits = new List<Trait>();

            foreach (Trait curTrait in this.Traits)
            {
                Trait copiedTrait = curTrait.CopyTrait();
                copyTraits.Add(copiedTrait);
            }
            CharacterSheet copy = new NWoDVampire(newName, copyTraits);
            return copy;
        }


        public override void PopulateCombatTraits()
        {
            base.PopulateCombatTraits();
            foreach (Trait curTrait in Traits)
            {
                switch (curTrait.TraitLabel)
                {
                    case NWoDConstants.VitaeStatName:
                        InitializeHealthBoxes(curTrait.TraitValue);
                        break;
                }
            }
            if (MaxVitae == 0)
            {
                MaxVitae = 10;
                CurrentVitae = 10;
            }
        }

        public override string Status
        {
            get
            {
                String normal = base.Status;
                normal = normal + "\nVitae:" + CurrentVitae+"/"+MaxVitae;
                StringBuilder sb = new StringBuilder();
                sb.Append("Status:\n");
                sb.Append("Health: ");
                sb.Append(BuildHealthString());
                return sb.ToString();
            }
            set
            {
            }
        }
    }
}
