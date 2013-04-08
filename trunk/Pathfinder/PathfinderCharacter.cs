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
                return new SolidColorBrush(Colors.Blue);
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
            PathfinderCharacter copyChar = new PathfinderCharacter(this.Name, allTraits);
            return copyChar;
        }

        internal override String DoDamage(int count, String descriptor)
        {
            var stringTraits = (from trait in Traits
                                where trait as StringTrait != null
                                select trait as StringTrait);


            var immuneTrait = stringTraits.Where(x => x.TraitLabel.Equals("Immunity") && x.TraitContents.Equals(descriptor)).FirstOrDefault();
            if (immuneTrait != null)
            {
                return Name+" was immune to "+descriptor;
            }
            var resistTrait = NumericTraits.Where(x => x.TraitLabel.Equals("Resist") && x.TraitDescription.Equals(descriptor)).FirstOrDefault();
            if(resistTrait!=null)
            {
                count -= resistTrait.TraitValue;
            }
            var damageReductionTrait = NumericTraits.Where(x => x.TraitLabel.Equals("Damage Resistance"));
            {
                //TODO - I hate SRD damage reduction rules.

            }
            if (count < 0)
            {
                return Name + " resisted all damage"; ;
            }
            _hitPoints -= count;
            return "";

        }

        internal override void ResetHealth()
        {

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
                    PathfinderDicePool curAttack = new PathfinderDicePool(1, 20, curBonus);
                    curAttack.Roll();
                    RollResults = RollResults + "Rolled "+curAttack.TotalValue+" VS "+attack.DefenseTarget+" of "+targetDefense;
                    if (curAttack.TotalValue==20||curAttack.TotalValue >= targetDefense)
                    {
                        RollResults = RollResults + "\tDamage: ";
                        for(int curIndex = 0;curIndex<attack.DamageDice.Count;curIndex++)
                        {
                            var curDamage = attack.DamageDice[curIndex];
                            curDamage.Roll();
                            RollResults = RollResults + " " + curDamage.TotalValue + " " + attack.DamageDescriptors[curIndex];
                            Target.DoDamage(curDamage.TotalValue, attack.DamageDescriptors[curIndex]);
                        }
                    }
                }
            }
            return damage;
        }

        internal override DicePool RollBasePool(List<Trait> dicePools, int modifier)
        {
            return null;
        }

        public override bool IsIncapacitated { get; set; }
    }
}
