using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    internal class PathfinderCharacter_HP : PathfinderCharacter
    {
        private int _hitPoints;

        public PathfinderCharacter_HP(string name, List<Trait> curTraits) :
            base(name, curTraits)
        {
        }

        [DataMember]
        public int CurrentHitPoints
        {
            get { return _hitPoints; }
            set { _hitPoints = value; }
        }

        [DataMember]
        public int MaxHitPoints { get; set; }

        public override SolidColorBrush StatusColor
        {
            get
            {
                if (CurrentHitPoints == 0)
                    return new SolidColorBrush(Colors.Yellow);
                if ((CurrentHitPoints < 0) && (CurrentHitPoints > -10))
                    return new SolidColorBrush(Colors.Orange);
                if (CurrentHitPoints <= -10)
                    return new SolidColorBrush(Colors.Red);
                return new SolidColorBrush(Colors.Black);
            }
        }

        public override string Status
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(base.Status);
                sb.Append("HP: " + _hitPoints + "/" + MaxHitPoints);
                return sb.ToString();
            }
        }

        internal override string HealthStatusLineDescription
        {
            get { return CurrentHitPoints + "/" + MaxHitPoints; }
        }

        public override void PopulateCombatTraits()
        {
            CurrentHitPoints = NumericTraits.Where(x => x.TraitLabel.Equals("HP")).FirstOrDefault().TraitValue;
            MaxHitPoints = CurrentHitPoints;
        }

        public override void HandleRegeneration(int regenValue)
        {
            if (CurrentHitPoints < MaxHitPoints)
            {
                int regenValueMax = MaxHitPoints - CurrentHitPoints;
                if (regenValueMax < regenValue)
                {
                    regenValue = regenValueMax;
                }
                CurrentHitPoints += regenValue;
                Report(Name + " regenerated " + regenValue + " damage -- " + CurrentHitPoints + "/" + MaxHitPoints);
            }
        }

        internal override void ResetHealth()
        {
            _hitPoints = MaxHitPoints;
            SetIncapacitated(false);
        }

        internal override CharacterSheet Copy(string newName)
        {
            var allTraits = new List<Trait>();
            foreach (Trait cur in Traits)
            {
                allTraits.Add(cur.CopyTrait());
            }
            var copyChar = new PathfinderCharacter_HP(newName, allTraits);
            return copyChar;
        }

        internal override String DoDamage(int count, String descriptor)
        {
            int startHitpoints = CurrentHitPoints;
            CurrentHitPoints -= count;
            if (CurrentHitPoints < 0)
            {
                SetIncapacitated(true);
            }
            else
            {
                SetIncapacitated(false);
            }
            Report(Name + " has " + CurrentHitPoints + "/" + MaxHitPoints + " HP");
            NotifyStatusChange();
            return "";
        }
    }
}