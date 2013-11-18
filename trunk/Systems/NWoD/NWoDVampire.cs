using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    internal class NWoDVampire : NWoDCharacter
    {
        private int _currentVitae;

        private int _maxVitae;

        public NWoDVampire(string characterName, List<Trait> traits)
            : base(characterName, traits)
        {
            IsVampire = true;
        }

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

        [DataMember]
        public int BloodAttackBonus { get; set; }

        [DataMember]
        public int BloodThisRound { get; set; }

        public override string Status
        {
            get
            {
                String normal = base.Status;
                normal = normal + "\nVitae: " + CurrentVitae + "/" + MaxVitae;
                normal = normal + "\nVitae this turn: " + BloodThisRound;
                normal = normal + "\nBlood buff: " + BloodAttackBonus;
                return normal;
            }
        }


        internal override CharacterSheet Copy(String newName)
        {
            var copyTraits = new List<Trait>();

            foreach (Trait curTrait in Traits)
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
            var VitaeTrait = FindNumericTrait(NWoDConstants.VitaeStatName);
            if (VitaeTrait != null)
            {
                MaxVitae = VitaeTrait.TraitValue;
                CurrentVitae = VitaeTrait.TraitValue;
            }
            else
            {
                ThrowAttributeNotFound(NWoDConstants.VitaeStatName);
            }
            if (MaxVitae == 0)
            {
                MaxVitae = 10;
                //CurrentVitae = 10;
            }
        }
        protected void ThrowAttributeNotFound(string missingAttribute)
        {
            throw new Exception("Trait " + missingAttribute + " not found for " + Name);
        }


        protected override void CheckForUnconsciousness(HealthBox.DamageType damageType)
        {
            if (damageType > HealthBox.DamageType.Lethal)
            {
                SetIncapacitated(true);
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
            HealthBox matchingBox = HealthTrack.FirstOrDefault(x => x.Box == damageType);
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


        public override Trait ResistanceTrait()
        {
            return FindNumericTrait("Blood Potency");
        }
        
    }
}