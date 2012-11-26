using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace XMLCharSheets
{
    class NWoDCharacter : CharacterSheet
    {

        NWoDRollDice curPool = new NWoDRollDice(5);
        private List<HealthBox> _healthTrack = new List<HealthBox>();
        public List<HealthBox> HealthTrack
        {
            get { return _healthTrack; }
            set { _healthTrack = value; }
        }

        private bool _isVampire = false;

        public bool IsVampire
        {
            get { return _isVampire = false; }
            set { _isVampire = value; }
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
                        return new SolidColorBrush(Colors.DarkGray);
                    case HealthBox.DamageType.Empty:
                    default:
                        return new SolidColorBrush(Colors.Black);

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
        
        protected override void PopulateCombatTraits()
        {
            foreach (Trait curTrait in Traits)
            {
                switch (curTrait.TraitLabel)
                {
                    case "Health":
                        InitializeHealthBoxes(curTrait.TraitValue);
                        break;
                    case "Initiative":
                        Initiative = curTrait.TraitValue;
                        break;
                    case "Melee Defense":
                        NormalMeleeDefense = curTrait.TraitValue;
                        CurrentMeleeDefense = curTrait.TraitValue;
                        break;
                }
            }

        }

        public override string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Status:\n");
                sb.Append("Health: ");
                sb.Append(BuildHealthString());
                return sb.ToString();
            }
            set
            {
                //Do Nothing

            }
        }

        private string BuildHealthString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (HealthBox curBox in HealthTrack)
            {
                sb.Append(curBox.ToBoxString()+" ");
            }
            return sb.ToString().Trim();
        }
        

        private void InitializeHealthBoxes(int p)
        {
            if (p < 3)
                p = 3;
            for (int curIndex = 0; curIndex < p; curIndex++)
            {
                HealthTrack.Add(new HealthBox());
            }
        }

        internal override string Roll(int totalDice)
        {
            totalDice += WoundPenalties;
            curPool = new NWoDRollDice(totalDice);
            curPool.Roll();
            String result = " {" + curPool.NumberOfDice + "}: " + curPool.CurrentSuccesses + " successes.\nResults: " + curPool.ResultDescription;
            _rollResults = result;
            return result;
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
            ChangeHealthProperties();
            
        }

        private void ChangeHealthProperties()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusColor");
        }

        private void AddDamageBox(HealthBox newDamage)
        {
            if (HealthTrack.Last().Box == HealthBox.DamageType.Grievous)
                return;
            for(int curIndex = 0;curIndex< HealthTrack.Count();curIndex++)
            {
                HealthBox curBox = HealthTrack[curIndex];
                if (curBox.Box < newDamage.Box)
                {
                    HealthTrack.Insert(curIndex, newDamage);
                    break;
                }
            }
            if (HealthTrack.Last().Box == HealthBox.DamageType.Empty)
            {
                HealthTrack.Remove(HealthTrack.Last());
                ChangeHealthProperties();
            }
            else
            {

                var secondToLastBox = HealthTrack[HealthTrack.Count() - 2];
                var lastBox = HealthTrack.Last();
                HealthBox spilloverDamage = new HealthBox();
                spilloverDamage.Box = lastBox.Box + 1;
                HealthTrack.Last().Box = HealthBox.DamageType.Empty;
                AddDamageBox(spilloverDamage);
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
        }

        internal override void AttackTarget()
        {
            var nwodTarget = Target as NWoDCharacter;
            int targetDefense = 0;
            var ChosenAttackTrait = FindTrait(ChosenAttack) as AttackTrait;
            if (ChosenAttackTrait.DefenseTarget.Contains("Melee"))
            {
                targetDefense = nwodTarget.CurrentMeleeDefense;
            }
            else
            {
                targetDefense = Target.FindTrait(ChosenAttackTrait.DefenseTarget).TraitValue;
            }
            var attackPool = ChosenAttackTrait.TraitValue;
            attackPool -= targetDefense;
            int attackSuccesses = 0;
            if (attackPool > 0)
            {
                Roll(attackPool);
                attackSuccesses = curPool.CurrentSuccesses;
            }
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

        internal override void NewRound()
        {
            CurrentMeleeDefense = NormalMeleeDefense;
        }


    }
}
