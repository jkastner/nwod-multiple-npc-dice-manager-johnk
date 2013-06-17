using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    class PathfinderCharacter_WoundsVigor : PathfinderCharacter
    {
        //http://paizo.com/pathfinderRPG/prd/ultimateCombat/variants/woundsAndVigor.html
        
        private int _currentWoundPoints;
        public int CurrentWoundPoints
        {
            get { return _currentWoundPoints; }
            set { _currentWoundPoints = value; }
        }
        private int _maxWoundPoints;
        public int MaxWoundPoints
        {
            get { return _maxWoundPoints; }
            set { _maxWoundPoints = value; }
        }

        private int _maxVigorPoints;

        public int MaxVigorPoints
        {
            get { return _maxVigorPoints; }
            set { _maxVigorPoints = value; }
        }

        private int _currentVigorPoints;

        public int CurrentVigorPoints
        {
            get { return _currentVigorPoints; }
            set { _currentVigorPoints = value; }
        }

        public override SolidColorBrush StatusColor
        {
            get
            {
                //Target has taken some Vigor, but no wounds.
                if (MaxVigorPoints > 10 && CurrentVigorPoints <= 10 && CurrentWoundPoints==MaxWoundPoints)
                    return new SolidColorBrush(Colors.Yellow);
                if ((CurrentWoundPoints < MaxWoundPoints)&&CurrentWoundPoints>WoundThreshhold)
                    return new SolidColorBrush(Colors.Orange);
                if (CurrentWoundPoints <= WoundThreshhold && CurrentWoundPoints > 0)
                    return new SolidColorBrush(Colors.Red);
                if (CurrentWoundPoints <= 0)
                    return new SolidColorBrush(Colors.DimGray);
                return new SolidColorBrush(Colors.Black);
            }
        }

        internal override void ResetHealth()
        {
            CurrentWoundPoints = MaxWoundPoints;
            CurrentVigorPoints = MaxVigorPoints;
            _isStaggered = false;
            NotifyStatusChange();
        }

        public override void HandleRegeneration(int regenValue)
        {
            if (CurrentVigorPoints < MaxVigorPoints)
            {
                int regenValueMax = MaxVigorPoints - CurrentVigorPoints;
                if (regenValueMax < regenValue)
                {
                    regenValue = regenValueMax;
                }
                CurrentVigorPoints += regenValue;
                Report(Name + " regenerated " + regenValue + " Vigor damage -- " + HealthStatusLineDescription);
            }
        }

        public override string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(base.Status);
                sb.Append("Health: " + HealthStatusLineDescription);
                return sb.ToString();
            }
        }

        internal override string HealthStatusLineDescription
        {
            get { return CurrentVigorPoints+"/"+MaxVigorPoints+" Vigor "+ CurrentWoundPoints+"/"+MaxWoundPoints+" Wounds"; }
        }

        public int WoundThreshhold { get; set; }

        public PathfinderCharacter_WoundsVigor(string name, List<Trait> curTraits) :
            base(name, curTraits)
        {
        }

        public override void PopulateCombatTraits()
        {
            WoundThreshhold = NumericTraits.Where(x => x.TraitLabel.Equals("Constitution")).FirstOrDefault().TraitValue;
            CurrentWoundPoints = WoundThreshhold * 2;
            MaxWoundPoints = CurrentWoundPoints;
            
            CurrentVigorPoints = NumericTraits.Where(x => x.TraitLabel.Equals("Vigor")).FirstOrDefault().TraitValue;
            MaxVigorPoints = CurrentVigorPoints;
        }

        bool _isStaggered = false;
        internal override String DoDamage(int count, String descriptor)
        {
            int startWounds = CurrentWoundPoints;
            int startVigor = CurrentVigorPoints;
            switch (descriptor)
            {
                case "Wounds":
                    CurrentWoundPoints -= count;
                    break;
                case "Vigor":
                    CurrentVigorPoints -= count;
                    if (CurrentVigorPoints < 0)
                    {
                        int overflowDamage = Math.Abs(CurrentVigorPoints);
                        CurrentVigorPoints = 0;
                        CurrentWoundPoints -= overflowDamage;
                    }
                    break;
                default:
                    DoDamage(count, "Vigor");
                    return String.Empty;
            }
            int VigorDifference = startVigor - CurrentVigorPoints;
            int woundsDifference = startWounds - CurrentWoundPoints;
            if (CurrentWoundPoints <= 0 && !IsIncapacitated)
            {
                Report(Name + " has been killed.\n");
                SetIncapacitated(true);
            }
            if (!_isStaggered && CurrentWoundPoints <= WoundThreshhold)
            {
                AssignStatus("Staggered from wounds", 500);
                _isStaggered = true;
            }
            
            NotifyStatusChange();
            return String.Empty;
        }

        internal override string NewRound()
        {
            String start = base.NewRound();
            if (CurrentWoundPoints < WoundThreshhold&&!IsIncapacitated)
            {
                var con = NumericTraits.Where(x => x.TraitLabel.Equals("Constitution")).FirstOrDefault().TraitValue;
                var conBonus = (con -10)/2;
                PathfinderDicePool conCheck = new PathfinderDicePool(1, 20, conBonus);
                conCheck.Roll();
                Report("\n"+Name+" made a wound check VS DC 10 -- "+conCheck.ResultDescription);
                
                if (conCheck.TotalValue < 10)
                {
                    Report(", fails and is unconscious.");
                    AssignStatus("Unconscious from wounds", 500);
                    SetIncapacitated(true);
                }
            }
            return start;
        }

        internal override CharacterSheet Copy(string newName)
        {
            List<Trait> allTraits = new List<Trait>();
            foreach (var cur in this.Traits)
            {
                allTraits.Add(cur.CopyTrait());
            }
            PathfinderCharacter_WoundsVigor copyChar = new PathfinderCharacter_WoundsVigor(newName, allTraits);
            return copyChar;
        }

        internal override List<PathfinderDamage> HandleAttackResults(PathfinderDicePool curDamage, int damageMultiplier, PathfinderCharacter pathfinderTarget, string damageDescriptor, bool wasCrit)
        {
            var targetVitWounds = pathfinderTarget as PathfinderCharacter_WoundsVigor;
            List<PathfinderDamage> damageList = new List<PathfinderDamage>();
            curDamage.DiceQuantity = curDamage.DiceQuantity * damageMultiplier;
            curDamage.Modifier = curDamage.Modifier * damageMultiplier;
            curDamage.Roll();
            Report("Damage roll -- " + curDamage.PoolDescription + " = " + curDamage.TotalValue + " " + damageDescriptor);
            PathfinderDamage doneDamage = pathfinderTarget.AdjustDamageByResistances(new PathfinderDamage(damageDescriptor,
                curDamage.TotalValue));
            if (doneDamage.DamageValue <= 0)
            {
                Report("\t" + Target.Name + " resisted all damage");
                return null;
            }
            else
            {
                //Split it up into 'Descriptor' and 'Descriptor - Wounds'
                int VigorDamage = 0;
                int woundDamage = 0;
                VigorDamage = doneDamage.DamageValue;
                int curVit = targetVitWounds.CurrentVigorPoints;
                curVit -= VigorDamage;
                if (curVit < 0)
                {
                    woundDamage = Math.Abs(curVit);
                    VigorDamage = targetVitWounds.CurrentVigorPoints;
                }
                if (wasCrit)
                {
                    woundDamage += damageMultiplier;
                }
                String finalDamage = "";
                if(VigorDamage>0)
                {
                    var vitDmg = new PathfinderDamage(damageDescriptor + " Vigor", VigorDamage);
                    Target.DoDamage(vitDmg.DamageValue, "Vigor");
                    finalDamage = vitDmg.DamageValue + " "+vitDmg.DamageDescriptor;
                    damageList.Add(vitDmg);
                }
                if(woundDamage>0)
                {
                    var wndDmg = new PathfinderDamage(damageDescriptor + " Wounds", woundDamage);
                    Target.DoDamage(wndDmg.DamageValue, "Wounds");
                    finalDamage = finalDamage + " "+ wndDmg.DamageValue + " " + wndDmg.DamageDescriptor;
                    damageList.Add(wndDmg);
                }
                Report("\n\tTarget took " + finalDamage);
                return damageList;
            }
        }

    }
}
