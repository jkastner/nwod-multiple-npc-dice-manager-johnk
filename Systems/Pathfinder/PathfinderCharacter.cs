using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (PathfinderCharacter_HP))]
    [KnownType(typeof (PathfinderCharacter_WoundsVigor))]
    public abstract class PathfinderCharacter : CharacterSheet
    {
        private string _rollResults = "";

        public PathfinderCharacter(string name, List<Trait> curTraits) :
            base(name, curTraits)
        {
        }

        public override String RollResults
        {
            get { return _rollResults; }
            set { _rollResults = value; }
        }

        public override String ChosenAttackValue
        {
            get { return ""; }
        }

        public bool SingleAttackOnly { get; set; }

        public override void RollInitiative()
        {
            var matchingNumeric =
                Traits.Where(x => (x as PathfinderNumericTrait) != null && x.TraitLabel.Equals("Initiative"))
                      .FirstOrDefault() as PathfinderNumericTrait;
            if (matchingNumeric == null)
                CurInitiative = -1;
            else
            {
                var rollIni = new PathfinderDicePool(1, 20, matchingNumeric.TraitValue);
                rollIni.Roll();
                CurInitiative = rollIni.TotalValue;
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
            IEnumerable<StringTrait> stringTraits = (from trait in Traits
                                                     where trait as StringTrait != null
                                                     select trait as StringTrait);
            StringTrait immuneTrait =
                stringTraits.FirstOrDefault(x => x.TraitLabel.Equals("Immunity") 
                    && x.TraitContents.Equals(descriptor));
            if (immuneTrait != null)
            {
                return new PathfinderDamage(receivedDamage.DamageDescriptor, 0);
            }
            NumericIntTrait resistTrait =
                NumericTraits.FirstOrDefault(x => x.TraitLabel.Equals("Resist") && x.TraitDescription.
                    Equals(descriptor));
            if (resistTrait != null)
            {
                int count = receivedDamage.DamageValue - resistTrait.TraitValue;
                if (count < 0)
                    count = 0;
                return new PathfinderDamage(receivedDamage.DamageDescriptor, count);
            }
            NumericIntTrait damageReductionTrait =
                NumericTraits.FirstOrDefault(x => x.TraitLabel.Equals("Damage Resistance"));
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

        internal virtual List<PathfinderDamage> HandleAttackResults(PathfinderDicePool curDamage, int damageMultiplier,
                                                                    PathfinderCharacter pathfinderTarget,
                                                                    String damageDescriptor, bool wasCrit)
        {
            curDamage.DiceQuantity = curDamage.DiceQuantity*damageMultiplier;
            curDamage.Modifier = curDamage.Modifier*damageMultiplier;
            curDamage.Roll();
            Report(curDamage.PoolDescription + " = " + curDamage.TotalValue + " " + damageDescriptor);
            PathfinderDamage doneDamage =
                pathfinderTarget.AdjustDamageByResistances(new PathfinderDamage(damageDescriptor,
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
                return new List<PathfinderDamage> {doneDamage};
            }
        }


        internal override List<Damage> AttackTarget(int RollModifier)
        {
            _rollResults = "";
            var damage = new List<Damage>();
            var pathfinderTarget = Target as PathfinderCharacter;
            int targetDefense = 0;
            var ChosenAttackTrait = FindNumericTrait(ChosenAttack) as AttackTrait;
            NumericIntTrait defenseTrait = pathfinderTarget.FindNumericTrait(ChosenAttackTrait.DefenseTarget);
            if (defenseTrait != null)
            {
                targetDefense = defenseTrait.TraitValue;
            }
            var allAttacks = new List<PathfinderAttackTrait> {ChosenAttackTrait as PathfinderAttackTrait};
            foreach (string cur in OtherAttackTraits)
            {
                NumericIntTrait matchingAttack = FindNumericTrait(cur);
                if (matchingAttack != null)
                {
                    var matchingPathfinderAttack = matchingAttack as PathfinderAttackTrait;
                    if (matchingPathfinderAttack != null)
                    {
                        allAttacks.Add(matchingPathfinderAttack);
                    }
                }
            }
            foreach (PathfinderAttackTrait attack in allAttacks)
            {
                foreach (int curBase in attack.ToHitBonusList)
                {
                    int curBonus = curBase + RollModifier;
                    var curAttack = new PathfinderDicePool(1, 20, 0);
                    curAttack.Roll();
                    int hitValue = curAttack.TotalValue + curBonus;
                    Report(Name + " attacked with " + attack.TraitDescription + ": Rolled " + curAttack.TotalValue + "+" +
                           curBonus + "=" + hitValue
                           + " VS " + attack.DefenseTarget + " of " + targetDefense);
                    if (curAttack.TotalValue != 1 &&
                        (curAttack.TotalValue == 20 || curAttack.TotalValue + curBonus >= targetDefense))
                    {
                        Report("\tDamage: ");
                        for (int curIndex = 0; curIndex < attack.DamageDice.Count; curIndex++)
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
                                    if (curAttack.TotalValue != 1 &&
                                        (curAttack.TotalValue == 20 || hitValue >= targetDefense))
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
                            PathfinderDicePool curDamage = attack.DamageDice[curIndex].CopyPool();
                            string damageDescriptor = attack.DamageDescriptors[curIndex];
                            if (DamageType != null)
                                damageDescriptor = DamageType;

                            List<PathfinderDamage> doneDamage = HandleAttackResults(curDamage,
                                                                                    damageMultiplier, pathfinderTarget,
                                                                                    damageDescriptor, wasCrit);
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
            IEnumerable<PathfinderNumericTrait> rollabletraits = (from trait in dicePools
                                                                  where trait as PathfinderNumericTrait != null
                                                                  select trait as PathfinderNumericTrait);
            foreach (PathfinderNumericTrait cur in rollabletraits)
            {
                modifier += cur.TraitValue;
            }
            var pool = new PathfinderDicePool(1, 20, modifier);
            pool.Roll();
            return pool;
        }
    }
}