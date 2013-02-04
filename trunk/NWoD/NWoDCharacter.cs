﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace XMLCharSheets
{
    class NWoDCharacter : CharacterSheet
    {

        //private void RollPool(int numInPool, int maxSides, int minSuccess, int minAgain, int uberFail, int subtractsOn)
        private NWoDDicePool curPool = new NWoDDicePool(new NWoDTrait(5, "DefaultPool", 10, 0, 0, 8));
        private List<HealthBox> _healthTrack = new List<HealthBox>();
        public List<HealthBox> HealthTrack
        {
            get { return _healthTrack; }
            set { _healthTrack = value; }
        }

        private bool _isVampire = false;

        public bool IsVampire
        {
            get { return _isVampire; }
            set 
            {
                _isVampire = value;
                OnPropertyChanged("IsVampire");
            }
        }

        internal override CharacterSheet Copy(string newName)
        {
            List<Trait> copyTraits = new List<Trait>();

            foreach (Trait curTrait in this.Traits)
            {
                Trait copiedTrait = curTrait.CopyTrait();
                copyTraits.Add(copiedTrait);
            }
            CharacterSheet copy = new NWoDCharacter(newName, copyTraits);
            return copy;
        }



        private int _armor;

        public int Armor
        {
            get { return _armor; }
            set { _armor = value; }
        }
        


        public NWoDCharacter(string characterName, List<Trait> traits)
            : base(characterName, traits)
        {
        }

        public override SolidColorBrush StatusColor
        {
            get
            {
                switch (HealthTrack.Last().Box)
                {
                    case HealthBox.DamageType.Bashing:
                        return new SolidColorBrush(Colors.Yellow);
                    case HealthBox.DamageType.Lethal:
                        return new SolidColorBrush(Colors.Orange);
                    case HealthBox.DamageType.Aggrivated:
                        return new SolidColorBrush(Colors.Red);
                    case HealthBox.DamageType.Grievous:
                        return new SolidColorBrush(Colors.LightGray);
                    case HealthBox.DamageType.Empty:
                    default:
                        return new SolidColorBrush(Colors.White);

                }
            }
        }

        public int WoundPenalties 
        {
            get
            {
                HealthBox last = HealthTrack.Last();
                if (last.Box != HealthBox.DamageType.Empty)
                {
                    return -3;
                }
                HealthBox secondToLast = HealthTrack[HealthTrack.Count() - 2];
                if (secondToLast.Box != HealthBox.DamageType.Empty)
                {
                    return -2;
                }
                HealthBox thirdToLast = HealthTrack[HealthTrack.Count() - 3];
                if (thirdToLast.Box != HealthBox.DamageType.Empty)
                {
                    return -1;
                }
                return 0;
            }
        }

        public override void PopulateCombatTraits()
        {
            foreach (Trait curTrait in Traits)
            {
                switch (curTrait.TraitLabel)
                {
                    case NWoDConstants.HealthStatName:
                        InitializeHealthBoxes(curTrait.TraitValue);
                        break;
                    case NWoDConstants.InitiativeStatName:
                        Initiative = curTrait.TraitValue;
                        break;
                    case NWoDConstants.MeleeDefenseStatName:
                        NormalMeleeDefense = curTrait.TraitValue;
                        CurrentMeleeDefense = curTrait.TraitValue;
                        break;
                    case NWoDConstants.ArmorStatName:
                        Armor = curTrait.TraitValue;
                        break;
                }
            }

        }

        public override string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(base.Status);
                sb.Append("Health: ");
                sb.AppendLine(BuildHealthString());
                sb.AppendLine("Melee Def: " + CurrentMeleeDefense);
                return sb.ToString();
            }
        }

        protected string BuildHealthString()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (HealthBox curBox in HealthTrack)
            {
                count++;
                sb.Append(curBox.ToBoxString()+" ");
                if (count % 5 == 0)
                    sb.Append("   ");
            }
            return sb.ToString().Trim();
        }
        

        protected void InitializeHealthBoxes(int p)
        {
            HealthTrack.Clear();
            if (p < 3)
                p = 3;
            for (int curIndex = 0; curIndex < p; curIndex++)
            {
                HealthTrack.Add(new HealthBox());
            }
        }

        internal string Roll(NWoDTrait traitToRoll)
        {
            curPool = new NWoDDicePool(traitToRoll);
            curPool.Roll();
            _rollResults = curPool.ResultDescription;
            return curPool.ResultDescription;
        }

        private bool _isIncapacitated = false;
        public override bool IsIncapacitated
        {
            get 
            {
                return _isIncapacitated;
            }
            set
            {
                _isIncapacitated = value;
            }
        }


        internal override void DoBashing()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Bashing;
            AddDamageBox(newDamage);


        }
        internal override void DoLethal()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Lethal;
            AddDamageBox(newDamage);
        }
        internal override void DoAggrivated()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Aggrivated;
            AddDamageBox(newDamage);
        }

        internal override void ResetHealth()
        {
            foreach (HealthBox curBox in HealthTrack)
            {
                curBox.Box = HealthBox.DamageType.Empty;
            }
            IsIncapacitated = false;
            _checkedAgainstUnconsciousness = false;
            NotifyStatusChange();
        }


        private bool _checkedAgainstUnconsciousness = false;

        private void AddDamageBox(HealthBox newDamage)
        {
            if (HealthTrack.Last().Box == HealthBox.DamageType.Grievous)
                return;
            while(newDamage.Box < HealthTrack.Last().Box)
            {
                newDamage.Box++;
            }
            for (int curIndex = 0; curIndex < HealthTrack.Count(); curIndex++)
            {
                HealthBox curBox = HealthTrack[curIndex];
                if (curBox.Box <= newDamage.Box)
                {
                    HealthTrack.Insert(curIndex, newDamage);
                    break;
                }
            }
            if (HealthTrack.Last().Box == HealthBox.DamageType.Empty)
            {
                HealthTrack.Remove(HealthTrack.Last());
                if (!IsIncapacitated && HealthTrack.Last().Box > HealthBox.DamageType.Bashing)
                {
                    StatusEffects.Add(new StatusEffect("Incapacitated", 500));
                    IsIncapacitated = true;
                }
                if (!IsIncapacitated && HealthTrack.Last().Box == HealthBox.DamageType.Bashing && !_checkedAgainstUnconsciousness)
                {
                    CheckToStayConscious();
                    _checkedAgainstUnconsciousness = true;
                }
                NotifyStatusChange();

                return;
            }
            else
            {

                var secondToLastBox = HealthTrack[HealthTrack.Count() - 2];
                var lastBox = HealthTrack.Last();
                HealthBox spilloverDamage = new HealthBox();
                spilloverDamage.Box = lastBox.Box + 1;
                secondToLastBox.Box = HealthBox.DamageType.Empty;
                HealthTrack.Remove(lastBox);
                AddDamageBox(spilloverDamage);
            }

        }

        protected virtual void CheckToStayConscious()
        {
            if (!(HealthTrack.Last().Box == HealthBox.DamageType.Bashing))
            {
                return;
            }
            var stamina = FindTrait("Stamina");
            int staminaCheckNum = HealthTrack.Count()-5;
            if (stamina != null)
            {
                staminaCheckNum = stamina.TraitValue;
            }
            NWoDDicePool staminaCheck = new NWoDDicePool(new NWoDTrait(staminaCheckNum, "Stamina Check", 10, 0, 0, 8));
            staminaCheck.Roll();
            if (staminaCheck.CurrentSuccesses == 0)
            {
                StatusEffects.Add(new StatusEffect("Incapacitated", 500));
                IsIncapacitated = true;
            }

        }

        public override string ChosenAttackValue
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                int fullValue = FindTrait(ChosenAttack).TraitValue;
                sb.Append(ChosenAttack);
                for (int curIndex = 0; curIndex < OtherAttackTraits.Count(); curIndex++)
                {
                    fullValue += FindTrait(OtherAttackTraits[curIndex]).TraitValue;
                }
                return fullValue.ToString();
            }
        }

        public override void RollInitiative()
        {
            int r = random.Next(10) + 1;
            CurInitiative = r + Initiative;
        }
        String _rollResults = "";
        public override string RollResults
        {
            get { return _rollResults; }
            set { _rollResults = value; }
        }

        internal override void AttackTarget(int modifier)
        {
            var nwodTarget = Target as NWoDCharacter;
            int targetDefense = 0;
            var ChosenAttackTrait = FindTrait(ChosenAttack) as AttackTrait;
            if (ChosenAttackTrait.DefenseTarget.Contains("Melee Defense"))
            {
                targetDefense = nwodTarget.CurrentMeleeDefense;
            }
            else
            {
                targetDefense = Target.FindTrait(ChosenAttackTrait.DefenseTarget).TraitValue;
            }
            var attackPool = (ChosenAttackTrait as NWoDAttackTrait).CopyTrait();
            foreach (String additionalAttackTrait in OtherAttackTraits)
            {
                attackPool.TraitValue += FindTrait(additionalAttackTrait).TraitValue;
            }
            attackPool.TraitValue -= targetDefense;
            attackPool.TraitValue -= nwodTarget.Armor;
            attackPool.TraitValue += modifier;
            attackPool.TraitValue += WoundPenalties;
            FinalAttackPool = attackPool.TraitValue;
            int attackSuccesses = 0;
            var results = RollBasePool(new List<Trait>(){attackPool}, 0) as NWoDDicePool;
            attackSuccesses = results.CurrentSuccesses;
            for (int curDamage = 0; curDamage < attackSuccesses; curDamage++)
            {
                switch (DamageType)
                {
                    case "Bashing":
                        Target.DoBashing();
                        break;
                    case "Aggrivated":
                        Target.DoAggrivated();
                        break;
                    default:
                    case "Lethal":
                        Target.DoLethal();
                        break;
                }
            }
            nwodTarget.WasAttacked(ChosenAttackTrait.DefenseTarget);
            RollResults = results.ResultDescription;
        }

        /// <summary>
        /// Modifier is a flat value and must be negative to remove dice.
        /// </summary>
        internal override DicePool RollBasePool(List<Trait> dicePools, int modifier)
        {
            String curResults = "";

            var firstPool = dicePools.First().CopyTrait();
            var basepool = firstPool as INWoDTrait;

            for(int curIndex = 1;curIndex<dicePools.Count;curIndex++)
            {
                var nextTrait = dicePools[curIndex] as INWoDTrait;
                 /*Johnathan K. 12-06-2012
                  * Presumed to take the 'non default' modifier. */
                 basepool.AddAndChangeFromDefaults(nextTrait);
            }
            basepool.TraitValue += modifier;
            NWoDDicePool curPool = new NWoDDicePool(basepool);
            curPool.Roll();
            return curPool;

        }

        private int _normalMeleeDefense;
        public int NormalMeleeDefense
        {
            get { return _normalMeleeDefense; }
            set { _normalMeleeDefense = value; }
        }

        private int _currentMeleeDefense;
        public int CurrentMeleeDefense
        {
            get { return _currentMeleeDefense; }
            set { _currentMeleeDefense = value; }
        }

        internal void WasAttacked(string DefenseType)
        {
            if (DefenseType.Contains("Melee"))
            {
                if (--CurrentMeleeDefense < 0)
                    CurrentMeleeDefense = 0;
            }
        }

        internal override string NewRound()
        {
            base.NewRound();
            CurrentMeleeDefense = NormalMeleeDefense;
            return String.Empty;

        }



    }
}
