using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    public abstract class PathfinderCharacter : CharacterSheet
    {
        public PathfinderCharacter(string name, List<Trait> curTraits):
            base(name, curTraits)
        {
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

        internal override string NewRound()
        {
            if (HasTrait("Regeneration"))
            {
                int regenValue = FindNumericTrait("Regeneration").TraitValue;
                HandleRegeneration(regenValue);
            }
            base.NewRound();
            return String.Empty;
        }

        public abstract void HandleRegeneration(int regenValue);

        internal PathfinderDamage AdjustDamageByResistances(PathfinderDamage receivedDamage)
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
            var damageReductionTrait = NumericTraits.Where(x => x.TraitLabel.Equals("Damage Resistance")).FirstOrDefault();
            if (damageReductionTrait != null)
            {
                if (!damageReductionTrait.TraitDescription.Equals(receivedDamage.DamageDescriptor))
                {
                    int count = receivedDamage.DamageValue - damageReductionTrait.TraitValue;
                    if (count < 0)
                        count = 0;
                    return new PathfinderDamage(receivedDamage.DamageDescriptor, count);
                }
                //TODO - I hate SRD damage reduction rules.

            }
            return receivedDamage;
        }

        internal virtual List<PathfinderDamage> HandleAttackResults(PathfinderDicePool curDamage, int damageMultiplier, PathfinderCharacter pathfinderTarget, String damageDescriptor, bool wasCrit)
        {
            curDamage.DiceQuantity = curDamage.DiceQuantity * damageMultiplier;
            curDamage.Modifier = curDamage.Modifier * damageMultiplier;
            curDamage.Roll();
            Report(curDamage.TotalValue + " " + damageDescriptor);
            PathfinderDamage doneDamage = pathfinderTarget.AdjustDamageByResistances(new PathfinderDamage(damageDescriptor,
                curDamage.TotalValue));
            if (doneDamage.DamageValue <= 0)
            {
                Report("\t" + Target.Name + " resisted all damage");
                return null;
            }
            else
            {
                Target.DoDamage(doneDamage.DamageValue, damageDescriptor);
                Report("Target took " + doneDamage.DamageValue + " " + damageDescriptor);
                return new List<PathfinderDamage>(){doneDamage};
            }
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
                    Report(RollResults + "Rolled " + curAttack.TotalValue + "+" + curBonus + "=" + hitValue
                        +" VS "+attack.DefenseTarget+" of "+targetDefense);
                    if (curAttack.TotalValue!=1&&(curAttack.TotalValue==20||curAttack.TotalValue+curBonus >= targetDefense))
                    {
                        Report("\tDamage: ");
                        for(int curIndex = 0;curIndex<attack.DamageDice.Count;curIndex++)
                        {
                            int damageMultiplier = 1;
                            bool wasCrit = false;
                            //If the damage is 1d8 + 1d6, as with a flaming longsword, the flaming damage is not multiplied on a critical hit
                            if (curIndex == 0)
                            {
                                if (curAttack.TotalValue >= attack.LowestValueToCrit)
                                {
                                    curAttack.Roll();
                                    hitValue = curAttack.TotalValue + curBonus;
                                    Report("Crit confirm " + curAttack.TotalValue + "+" + curBonus + "=" + hitValue
                                        + " VS " + attack.DefenseTarget + " of " + targetDefense);
                                    if (curAttack.TotalValue!=1 && (curAttack.TotalValue==20|| hitValue >= targetDefense))
                                    {
                                        Report("--Crit confirmed--");
                                        damageMultiplier = attack.CritMultipier;
                                        wasCrit = true;
                                    }
                                    else
                                    {
                                        Report("--Crit fails--");
                                    }
                                }
                            }
                            Report("\n\t");
                            var curDamage = attack.DamageDice[curIndex].CopyPool();
                            var doneDamage = HandleAttackResults(curDamage,
                                damageMultiplier, pathfinderTarget, attack.DamageDescriptors[curIndex], wasCrit);
                            if (doneDamage != null)
                                damage.AddRange(doneDamage);

                        }
                    }
                    Report("\n");
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
