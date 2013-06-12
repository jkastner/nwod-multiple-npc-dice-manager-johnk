using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    class PathfinderCharacter_WoundsVitality : PathfinderCharacter
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

        private int _maxVitalityPoints;

        public int MaxVitalityPoints
        {
            get { return _maxVitalityPoints; }
            set { _maxVitalityPoints = value; }
        }

        private int _currentVitalityPoints;

        public int CurrentVitalityPoints
        {
            get { return _currentVitalityPoints; }
            set { _currentVitalityPoints = value; }
        }

        public override SolidColorBrush StatusColor
        {
            get
            {
                //Target has taken some vitality, but no wounds.
                if (MaxVitalityPoints > 10 && CurrentVitalityPoints <= 10 && CurrentWoundPoints==MaxWoundPoints)
                    return new SolidColorBrush(Colors.Yellow);
                if ((CurrentWoundPoints < MaxWoundPoints)&&CurrentWoundPoints>WoundThreshhold)
                    return new SolidColorBrush(Colors.Orange);
                if (CurrentWoundPoints < WoundThreshhold && CurrentWoundPoints > 0)
                    return new SolidColorBrush(Colors.Red);
                if (CurrentWoundPoints <= 0)
                    return new SolidColorBrush(Colors.DimGray);
                return new SolidColorBrush(Colors.Black);
            }
        }

        internal override void ResetHealth()
        {
            CurrentWoundPoints = MaxWoundPoints;
            CurrentVitalityPoints = MaxVitalityPoints;
            NotifyStatusChange();
        }

        public override void HandleRegeneration(int regenValue)
        {
            if (CurrentVitalityPoints < MaxVitalityPoints)
            {
                int regenValueMax = MaxVitalityPoints - CurrentVitalityPoints;
                if (regenValueMax < regenValue)
                {
                    regenValue = regenValueMax;
                }
                CurrentVitalityPoints += regenValue;
                Report(Name + " regenerated " + regenValue + " vitality damage -- " + HealthStatusLineDescription);
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
            get { return CurrentVitalityPoints+"/"+MaxVitalityPoints+" Vitality "+ CurrentWoundPoints+"/"+MaxWoundPoints+" Wounds"; }
        }

        public int WoundThreshhold { get; set; }

        public PathfinderCharacter_WoundsVitality(string name, List<Trait> curTraits) :
            base(name, curTraits)
        {
        }

        public override void PopulateCombatTraits()
        {
            WoundThreshhold = NumericTraits.Where(x => x.TraitLabel.Equals("Constitution")).FirstOrDefault().TraitValue;
            CurrentWoundPoints = WoundThreshhold * 2;
            MaxWoundPoints = CurrentWoundPoints;
            
            CurrentVitalityPoints = NumericTraits.Where(x => x.TraitLabel.Equals("Vitality")).FirstOrDefault().TraitValue;
            MaxVitalityPoints = CurrentVitalityPoints;
        }
        
        internal override String DoDamage(int count, String descriptor)
        {
            int startWounds = CurrentWoundPoints;
            int startVitality = CurrentVitalityPoints;
            switch (descriptor)
            {
                case "Wounds":
                    CurrentWoundPoints -= count;
                    break;
                case "Vitality":
                    CurrentVitalityPoints -= count;
                    if (CurrentVitalityPoints < 0)
                    {
                        int overflowDamage = Math.Abs(CurrentVitalityPoints);
                        CurrentVitalityPoints = 0;
                        CurrentWoundPoints -= overflowDamage;
                    }
                    break;
                default:
                    throw new Exception("Unknown damage type.");
            }
            int vitalityDifference = startVitality - CurrentVitalityPoints;
            int woundsDifference = startWounds - CurrentWoundPoints;
            if (CurrentWoundPoints <= WoundThreshhold)
            {
                AssignStatus("Staggered from wounds", 500);
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
            PathfinderCharacter_WoundsVitality copyChar = new PathfinderCharacter_WoundsVitality(newName, allTraits);
            return copyChar;
        }

        internal override List<PathfinderDamage> HandleAttackResults(PathfinderDicePool curDamage, int damageMultiplier, PathfinderCharacter pathfinderTarget, string damageDescriptor, bool wasCrit)
        {
            var targetVitWounds = pathfinderTarget as PathfinderCharacter_WoundsVitality;
            List<PathfinderDamage> damageList = new List<PathfinderDamage>();
            curDamage.DiceQuantity = curDamage.DiceQuantity * damageMultiplier;
            curDamage.Modifier = curDamage.Modifier * damageMultiplier;
            curDamage.Roll();
            Report("Damage roll -- "+curDamage.TotalValue + " " + damageDescriptor);
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
                int vitalityDamage = 0;
                int woundDamage = 0;
                vitalityDamage = doneDamage.DamageValue;
                int curVit = targetVitWounds.CurrentVitalityPoints;
                curVit -= vitalityDamage;
                if (curVit < 0)
                {
                    woundDamage = Math.Abs(curVit);
                    vitalityDamage = targetVitWounds.CurrentVitalityPoints;
                }
                if (wasCrit)
                {
                    woundDamage += damageMultiplier;
                }
                String finalDamage = "";
                if(vitalityDamage>0)
                {
                    var vitDmg = new PathfinderDamage(damageDescriptor + " Vitality", vitalityDamage);
                    Target.DoDamage(vitDmg.DamageValue, "Vitality");
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
