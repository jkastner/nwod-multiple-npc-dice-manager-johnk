using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    internal class NWoDCharacter : CharacterSheet
    {
        //private void RollPool(int numInPool, int maxSides, int minSuccess, int minAgain, int uberFail, int subtractsOn)
        [DataMember] private bool _checkedAgainstUnconsciousness;
        private List<HealthBox> _healthTrack = new List<HealthBox>();

        private bool _isVampire;
        private String _rollResults = "";
        [DataMember] private NWoDDicePool curPool = new NWoDDicePool(new NWoDTrait("DefaultPool", 5, 10, 0, 0, 8));

        public NWoDCharacter(string characterName, List<Trait> traits)
            : base(characterName, traits)
        {
        }

        [DataMember]
        public List<HealthBox> HealthTrack
        {
            get { return _healthTrack; }
            set { _healthTrack = value; }
        }

        [DataMember]
        public bool IsVampire
        {
            get { return _isVampire; }
            set
            {
                _isVampire = value;
                OnPropertyChanged("IsVampire");
            }
        }


        [DataMember]
        public int Armor { get; set; }


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
                        return new SolidColorBrush(Colors.Brown);
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

        public override string Status
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(base.Status);
                sb.Append("Health: ");
                sb.AppendLine(BuildHealthString());
                sb.AppendLine("Melee Def: " + CurrentMeleeDefense);
                return sb.ToString();
            }
        }

        internal override string HealthStatusLineDescription
        {
            get { return BuildHealthString(); }
        }

        public override string ChosenAttackValue
        {
            get
            {
                var sb = new StringBuilder();
                int fullValue = FindNumericTrait(ChosenAttack).TraitValue;
                sb.Append(ChosenAttack);
                for (int curIndex = 0; curIndex < OtherAttackTraits.Count(); curIndex++)
                {
                    fullValue += FindNumericTrait(OtherAttackTraits[curIndex]).TraitValue;
                }
                return fullValue.ToString();
            }
        }

        public override string RollResults
        {
            get { return _rollResults; }
            set { _rollResults = value; }
        }

        [DataMember]
        public int NormalMeleeDefense { get; set; }

        [DataMember]
        public int CurrentMeleeDefense { get; set; }

        internal override CharacterSheet Copy(string newName)
        {
            var copyTraits = new List<Trait>();

            foreach (Trait curTrait in Traits)
            {
                Trait copiedTrait = curTrait.CopyTrait();
                copyTraits.Add(copiedTrait);
            }
            CharacterSheet copy = new NWoDCharacter(newName, copyTraits);
            return copy;
        }

        public override void PopulateCombatTraits()
        {
            foreach (NumericIntTrait curTrait in NumericTraits)
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

        protected string BuildHealthString()
        {
            var sb = new StringBuilder();
            int count = 0;
            foreach (HealthBox curBox in HealthTrack)
            {
                count++;
                sb.Append(curBox.ToBoxString() + " ");
                if (count%5 == 0)
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

        internal override String DoDamage(int damage, String descriptor)
        {
            switch (descriptor)
            {
                case "Bashing":
                    DoBashing();
                    break;
                case "Lethal":
                    DoLethal();
                    break;
                case "Aggrivated":
                    DoAggrivated();
                    break;
            }
            return "";
        }


        internal void DoBashing()
        {
            var newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Bashing;
            AddDamageBox(newDamage);
        }

        internal String DoLethal()
        {
            var newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Lethal;
            AddDamageBox(newDamage);
            return "";
        }

        internal String DoAggrivated()
        {
            var newDamage = new HealthBox();
            newDamage.Box = HealthBox.DamageType.Aggrivated;
            AddDamageBox(newDamage);
            return "";
        }

        internal override void ResetHealth()
        {
            foreach (HealthBox curBox in HealthTrack)
            {
                curBox.Box = HealthBox.DamageType.Empty;
            }
            SetIncapacitated(false);
            _checkedAgainstUnconsciousness = false;
        }


        private void AddDamageBox(HealthBox newDamage)
        {
            if (HealthTrack.Last().Box == HealthBox.DamageType.Grievous)
                return;
            while (newDamage.Box < HealthTrack.Last().Box)
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
                    CheckForUnconsciousness(HealthTrack.Last().Box);
                }
                if (!IsIncapacitated && HealthTrack.Last().Box == HealthBox.DamageType.Bashing &&
                    !_checkedAgainstUnconsciousness)
                {
                    CheckToStayConscious();
                    _checkedAgainstUnconsciousness = true;
                }
                NotifyStatusChange();

                return;
            }
            else
            {
                HealthBox secondToLastBox = HealthTrack[HealthTrack.Count() - 2];
                HealthBox lastBox = HealthTrack.Last();
                var spilloverDamage = new HealthBox();
                spilloverDamage.Box = lastBox.Box + 1;
                secondToLastBox.Box = HealthBox.DamageType.Empty;
                HealthTrack.Remove(lastBox);
                AddDamageBox(spilloverDamage);
            }
        }

        protected virtual void CheckForUnconsciousness(HealthBox.DamageType damageType)
        {
            if (damageType > HealthBox.DamageType.Bashing)
            {
                SetIncapacitated(true);
            }
        }

        protected virtual void CheckToStayConscious()
        {
            if (!(HealthTrack.Last().Box == HealthBox.DamageType.Bashing))
            {
                return;
            }
            NumericIntTrait stamina = FindNumericTrait("Stamina");
            int staminaCheckNum = HealthTrack.Count() - 5;
            if (stamina != null)
            {
                staminaCheckNum = stamina.TraitValue;
            }
            var staminaCheck = new NWoDDicePool(new NWoDTrait("Stamina Check", staminaCheckNum, 10, 0, 0, 8));
            staminaCheck.Roll();
            String succeeded = "failed";
            if (staminaCheck.CurrentSuccesses > 0)
            {
                succeeded = "succeeded";
            }
            Report(Name + " " + succeeded + " rolled to stay conscious - "
                   + staminaCheck.ResultDescription);

            if (staminaCheck.CurrentSuccesses == 0)
            {
                SetIncapacitated(true);
            }
        }

        public override void RollInitiative()
        {
            int r = random.Next(10) + 1;
            CurInitiative = r + Initiative;
        }

        internal override List<Damage> AttackTarget(int modifier)
        {
            var damage = new List<Damage>();
            var nwodTarget = Target as NWoDCharacter;
            int targetDefense = 0;
            var ChosenAttackTrait = FindNumericTrait(ChosenAttack) as AttackTrait;
            if (ChosenAttackTrait.DefenseTarget.Contains("Melee Defense"))
            {
                targetDefense = nwodTarget.CurrentMeleeDefense;
            }
            else
            {
                targetDefense = Target.FindNumericTrait(ChosenAttackTrait.DefenseTarget).TraitValue;
            }
            var attackPool = (ChosenAttackTrait as NWoDAttackTrait).CopyTrait() as NumericIntTrait;
            foreach (String additionalAttackTrait in OtherAttackTraits)
            {
                attackPool.TraitValue += FindNumericTrait(additionalAttackTrait).TraitValue;
            }
            attackPool.TraitValue -= targetDefense;
            attackPool.TraitValue -= nwodTarget.Armor;
            attackPool.TraitValue += modifier;
            attackPool.TraitValue += WoundPenalties;
            FinalAttackPool = attackPool.TraitValue;
            int attackSuccesses = 0;
            var results = RollBasePool(new List<Trait> {attackPool}, 0) as NWoDDicePool;
            attackSuccesses = results.CurrentSuccesses;
            for (int curDamage = 0; curDamage < attackSuccesses; curDamage++)
            {
                switch (DamageType)
                {
                    case "Bashing":
                        damage.Add(new NWoDDamage(DamageType, 1));
                        Target.DoDamage(1, "Bashing");
                        break;
                    case "Aggrivated":
                        damage.Add(new NWoDDamage(DamageType, 1));
                        Target.DoDamage(1, "Aggrivated");
                        break;
                    default:
                    case "Lethal":
                        damage.Add(new NWoDDamage(DamageType, 1));
                        Target.DoDamage(1, "Lethal");
                        break;
                }
            }
            nwodTarget.WasAttacked(ChosenAttackTrait.DefenseTarget);
            RollResults = results.ResultDescription;
            return damage;
        }

        /// <summary>
        ///     Modifier is a flat value and must be negative to remove dice.
        /// </summary>
        internal override DicePool RollBasePool(List<Trait> dicePools, int modifier)
        {
            String curResults = "";

            Trait firstPool = dicePools.First().CopyTrait();
            var basepool = firstPool as INWoDTrait;

            for (int curIndex = 1; curIndex < dicePools.Count; curIndex++)
            {
                var nextTrait = dicePools[curIndex] as INWoDTrait;
                /*Johnathan K. 12-06-2012
                  * Presumed to take the 'non default' Modifier. */
                basepool.AddAndChangeFromDefaults(nextTrait);
            }
            basepool.TraitValue += modifier;
            var curPool = new NWoDDicePool(basepool);
            curPool.Roll();
            return curPool;
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

        

        public virtual Trait ResistanceTrait()
        {
            return null;
        }
        
        
    }
}