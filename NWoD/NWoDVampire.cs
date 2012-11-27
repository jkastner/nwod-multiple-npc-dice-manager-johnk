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

        protected override void PopulateCombatTraits()
        {
            base.PopulateCombatTraits();
            foreach (Trait curTrait in Traits)
            {
                switch (curTrait.TraitLabel)
                {
                    case "Vitae":
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
