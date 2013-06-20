using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    internal class NWoDVampire : NWoDCharacter
    {
        public NWoDVampire(string characterName, List<Trait> traits)
            : base(characterName, traits)
        {
            IsVampire = true;
        }

        private int _currentVitae;
        [DataMember]
        public int CurrentVitae
        {
            get { return _currentVitae; }
            set
            { 
                _currentVitae = value;
                OnPropertyChanged("CurrentVitae");
            }
        }
        private int _maxVitae;
        [DataMember]
        public int MaxVitae
        {
            get { return _maxVitae; }
            set
            {
                _maxVitae = value;
                OnPropertyChanged("MaxVitae");
            }
        }

        private int _bloodAttackBonus = 0;
        [DataMember]
        public int BloodAttackBonus
        {
            get { return _bloodAttackBonus; }
            set { _bloodAttackBonus = value; }
        }

        private int _bloodThisRound;
        [DataMember]
        public int BloodThisRound
        {
            get { return _bloodThisRound; }
            set { _bloodThisRound = value; }
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
            foreach (NumericIntTrait curTrait in NumericTraits)
            {
                switch (curTrait.TraitLabel)
                {
                    case NWoDConstants.VitaeStatName:
                        MaxVitae = curTrait.TraitValue;
                        CurrentVitae = curTrait.TraitValue;
                        break;
                }
            }
            if (MaxVitae == 0)
            {
                MaxVitae = 10;
                //CurrentVitae = 10;
            }
        }

        public override string Status
        {
            get
            {
                String normal = base.Status;
                normal = normal + "\nVitae: " + CurrentVitae+"/"+MaxVitae;
                normal = normal + "\nVitae this turn: "+BloodThisRound;
                normal = normal + "\nBlood buff: " + BloodAttackBonus;
                return normal;
            }
        }

        internal override List<Damage> AttackTarget(int modifier)
        {
            var ChosenAttackTrait = FindNumericTrait(ChosenAttack) as AttackTrait;
            ChosenAttackTrait.TraitValue += BloodAttackBonus;
            List<Damage> results = base.AttackTarget(modifier);
            FinalAttackPool += BloodAttackBonus;
            ChosenAttackTrait.TraitValue -= BloodAttackBonus;
            return results;
        }

        internal override string NewRound()
        {
            base.NewRound();
            BloodAttackBonus = 0;
            BloodThisRound = 0;
            return String.Empty;
        }


        internal void BloodBuff()
        {
            BloodThisRound++;
            BloodAttackBonus += 2;
            CurrentVitae--;
            NotifyStatusChange();
        }

        public bool HasHealableWounds()
        {
            bool hasBashing = HealthTrack.Where(x => x.Box == HealthBox.DamageType.Bashing).Any();
            bool hasLethal = HealthTrack.Where(x => x.Box == HealthBox.DamageType.Lethal).Any();
            return hasBashing || hasLethal;
        }

        internal void BloodHeal()
        {
            BloodThisRound++;
            CurrentVitae--;
            int totalBashing = HealthTrack.Where(x => x.Box == HealthBox.DamageType.Bashing).Count();
            if (totalBashing >= 2)
            {
                RemoveDamage(HealthBox.DamageType.Bashing);
                RemoveDamage(HealthBox.DamageType.Bashing);
            }
            else
            {
                RemoveDamage(HealthBox.DamageType.Lethal);
            }
            NotifyStatusChange();
        }

        private void RemoveDamage(HealthBox.DamageType damageType)
        {
            HealthBox matchingBox = HealthTrack.Where(x => x.Box == damageType).FirstOrDefault();
            if (matchingBox != null)
            {
                HealthTrack.Remove(matchingBox);
                HealthTrack.Add(new HealthBox());
            }
        }

        protected override void CheckToStayConscious()
        {
            return;
        }


        internal void ResetVitae()
        {
            CurrentVitae = MaxVitae;
            NotifyStatusChange();
        }
    }
}
