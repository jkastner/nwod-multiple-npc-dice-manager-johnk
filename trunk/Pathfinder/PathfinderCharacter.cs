using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    public class PathfinderCharacter : CharacterSheet
    {
        public PathfinderCharacter(string name, List<Trait> curTraits):
            base(name, curTraits)
        {
        }

        private int _hitPoints;
        public int HitPoints
        {
            get { return _hitPoints; }
            set { _hitPoints = value; }
        }
        private int _maxHitPoints;
        public int MaxHitPoints
        {
            get { return _maxHitPoints; }
            set { _maxHitPoints = value; }
        }

        public override void PopulateCombatTraits()
        {
            HitPoints = NumericTraits.Where(x => x.TraitLabel.Equals("HP")).FirstOrDefault().TraitValue;
            MaxHitPoints = HitPoints;
        }

        public override void RollInitiative()
        {
            var matchingNumeric = Traits.Where(x => (x as PathfinderNumericTrait) != null && x.TraitLabel.Equals("Initiative")).FirstOrDefault() as PathfinderNumericTrait;
            if (matchingNumeric == null)
                CurInitiative = -1;
            else
            {
                PathfinderDicePool rollIni = new PathfinderDicePool(1, 20, matchingNumeric.TraitValue);
                rollIni.Roll();
                CurInitiative = rollIni.TotalValue;
            }
            
        }

        public override SolidColorBrush StatusColor
        {
            get
            {
                if (HitPoints == 0)
                    return new SolidColorBrush(Colors.Yellow);
                if((HitPoints<0)&&(HitPoints>-10))
                    return new SolidColorBrush(Colors.Orange);
                if (HitPoints <= -10)
                    return new SolidColorBrush(Colors.Red);
                return new SolidColorBrush(Colors.Black);
            }
        }

        public override string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(base.Status);
                sb.Append("HP: "+_hitPoints+"/"+MaxHitPoints);
                return sb.ToString();
            }
        }

        private string _rollResults = "";
        public override String RollResults
        {
            get
            {
                return _rollResults;
            }
            set
            {
                _rollResults = value;
            }
        }

        internal override CharacterSheet Copy(string newName)
        {
            List<Trait> allTraits = new List<Trait>();
            foreach(var cur in this.Traits)
            {
                allTraits.Add(cur.CopyTrait());
            }
            PathfinderCharacter copyChar = new PathfinderCharacter(newName, allTraits);
            return copyChar;
        }

        internal override String DoDamage(int count, String descriptor)
        {
            _hitPoints -= count;
            if (_hitPoints < 0)
            {
                SetIncapacitated(true);
            }
            else
            {
                SetIncapacitated(false);
            }
            return "";
        }

        internal override string NewRound()
        {
            if (HasTrait("Regeneration"))
            {
                if (HitPoints < MaxHitPoints)
                {
                    int regenValueMax = MaxHitPoints - HitPoints;
                    int regenValue = FindNumericTrait("Regeneration").TraitValue;
                    if (regenValueMax < regenValue)
                    {
                        regenValue = regenValueMax;
                    }
                    HitPoints += regenValue;
                    OnReportTextFromCharacterEvent(new ReportTextFromCharacterEvent(Name + " regenerated " + regenValue + " damage -- " + HitPoints + "/" + MaxHitPoints));
                }
            }
            base.NewRound();
            return String.Empty;
        }

        internal Damage AdjustDamageByResistances(Damage receivedDamage)
        {
            String descriptor = receivedDamage.DamageDescriptor;
            var stringTraits = (from trait in Traits
                                where trait as StringTrait != null
                                select trait as StringTrait);
            var immuneTrait = stringTraits.Where(x => x.TraitLabel.Equals("Immunity") && x.TraitContents.Equals(descriptor)).FirstOrDefault();
            if (immuneTrait != null)
            {
                return new PathfinderDamage(receivedDamage.DamageDescriptor, 0);
            }
            var resistTrait = NumericTraits.Where(x => x.TraitLabel.Equals("Resist") && x.TraitDescription.Equals(descriptor)).FirstOrDefault();
            if (resistTrait != null)
            {
                int count = receivedDamage.DamageValue - resistTrait.TraitValue;
                if (count < 0)
                    count = 0;
                return new PathfinderDamage(receivedDamage.DamageDescriptor, count);
            }
            var damageReductionTrait = NumericTraits.Where(x => x.TraitLabel.Equals("Damage Resistance"));
            {
                //TODO - I hate SRD damage reduction rules.

            }
            return receivedDamage;
        }


        internal override void ResetHealth()
        {
            _hitPoints = MaxHitPoints;
            SetIncapacitated(false);
        }

        public override String ChosenAttackValue
        {
            get
            {
                return "";
            }
        }

        internal override List<Damage> AttackTarget(int RollModifier)
        {
            _rollResults = "";
            List<Damage> damage = new List<Damage>();
            var pathfinderTarget = Target as PathfinderCharacter;
            int targetDefense = 0;
            var ChosenAttackTrait = FindNumericTrait(ChosenAttack) as AttackTrait;
            var defenseTrait = pathfinderTarget.FindNumericTrait(ChosenAttackTrait.DefenseTarget);
            if (defenseTrait != null)
            {
                targetDefense = defenseTrait.TraitValue;
            }
            List<PathfinderAttackTrait> allAttacks = new List<PathfinderAttackTrait>(){ChosenAttackTrait as PathfinderAttackTrait};
            foreach(var cur in OtherAttackTraits)
            {
                var matchingAttack = FindNumericTrait(cur);
                if(matchingAttack != null)
                {
                    var matchingPathfinderAttack = matchingAttack as PathfinderAttackTrait;
                    if(matchingPathfinderAttack!=null)
                    {
                        allAttacks.Add(matchingPathfinderAttack);
                    }
                }
            }
            foreach(var attack in allAttacks)
            {
                foreach(var curBase in attack.ToHitBonusList)
                {
                    var curBonus = curBase + RollModifier;
                    PathfinderDicePool curAttack = new PathfinderDicePool(1, 20, 0);
                    curAttack.Roll();
                    int hitValue = curAttack.TotalValue + curBonus;
                    RollResults = RollResults + "Rolled "+curAttack.TotalValue+"+"+curBonus+"="+hitValue
                        +" VS "+attack.DefenseTarget+" of "+targetDefense;
                    if (curAttack.TotalValue!=1&&(curAttack.TotalValue==20||curAttack.TotalValue+curBonus >= targetDefense))
                    {
                        RollResults = RollResults + "\tDamage: ";
                        for(int curIndex = 0;curIndex<attack.DamageDice.Count;curIndex++)
                        {
                            int damageMultiplier = 1;
                            if (curIndex == 0)
                            {
                                if (curAttack.TotalValue >= attack.LowestValueToCrit)
                                {
                                    curAttack.Roll();
                                    hitValue = curAttack.TotalValue + curBonus;
                                    RollResults = RollResults + "Crit confirm " + curAttack.TotalValue + "+" + curBonus + "=" + hitValue
                                        + " VS " + attack.DefenseTarget + " of " + targetDefense;
                                    if (curAttack.TotalValue!=1 && (curAttack.TotalValue==20|| hitValue >= targetDefense))
                                    {
                                        RollResults = RollResults + "--Crit confirmed--";
                                        damageMultiplier = attack.CritMultipier;
                                    }
                                    else
                                    {
                                        RollResults = RollResults + "--Crit fails--";
                                    }
                                }
                            }
                            var curDamage = attack.DamageDice[curIndex].CopyPool();
                            curDamage.DiceQuantity = curDamage.DiceQuantity * damageMultiplier;
                            curDamage.Modifier = curDamage.Modifier * damageMultiplier;
                            curDamage.Roll();
                            RollResults = RollResults + " " + curDamage.TotalValue + " " + attack.DamageDescriptors[curIndex];
                            var doneDamage = pathfinderTarget.AdjustDamageByResistances(new PathfinderDamage(attack.DamageDescriptors[curIndex],
                                curDamage.TotalValue));
                            if (doneDamage.DamageValue <= 0)
                            {
                                _rollResults = _rollResults + "\t" + Target.Name + " resisted all damage.";
                            }
                            else
                            {
                                Target.DoDamage(curDamage.TotalValue, attack.DamageDescriptors[curIndex]);
                                damage.Add(doneDamage);
                                _rollResults = _rollResults + "\t" + Target.Name + " has "+pathfinderTarget.HitPoints+"/"+pathfinderTarget.MaxHitPoints+" HP.";
                            }
                        }
                    }
                    _rollResults = _rollResults + "\n";
                    if (SingleAttackOnly)
                    {
                        break;
                    }
                }
                if (SingleAttackOnly)
                {
                    break;
                }
            }
            return damage;
        }

        internal override DicePool RollBasePool(List<Trait> dicePools, int modifier)
        {
            var rollabletraits = (from trait in dicePools
                                where trait as PathfinderNumericTrait != null
                                select trait as PathfinderNumericTrait);
            foreach(var cur in rollabletraits)
            {
                modifier += cur.TraitValue;
            }
            var pool = new PathfinderDicePool(1, 20, modifier);
            pool.Roll();
            return pool;
        }



        public bool SingleAttackOnly { get; set; }
    }
}
