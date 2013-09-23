using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class PathfinderAttackTrait : AttackTrait, IPathfinderTrait
    {
        private List<String> _damageDescriptors = new List<String>();
        private List<PathfinderDicePool> _damageDice = new List<PathfinderDicePool>();
        private List<int> _toHitBonusList = new List<int>();

        private PathfinderAttackTrait(string label, int attackValue, String targetDefense,
                                      List<String> damageDescriptors, List<int> toHitBonusList,
                                      List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)
            : base(label, attackValue, targetDefense, string.Join(",", damageDescriptors.ToArray()))
        {
            TraitLabel = label;
            TraitValue = attackValue;
            DamageDescriptors = damageDescriptors;
            ToHitBonusList = toHitBonusList;
            DamageDice = damageDice;
            LowestValueToCrit = lowestValueToCrit;
            CritMultipier = critMultipier;
        }

        public PathfinderAttackTrait(string label, int attackValue, string descriptors, string attackbonuses,
                                     string damage, string triggers, string triggeredeffects, string targetDefense,
                                     string critRange, string critMultiplier)
            //public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            : base(label, attackValue, targetDefense, descriptors)
        {
            //string triggers, string triggeredeffects, string critRange, string critMultiplier
            String[] descriptorList = descriptors.Split(',');
            foreach (string cur in descriptorList)
            {
                DamageDescriptors.Add(cur.Trim());
            }
            String[] attackList = attackbonuses.Split('/');
            foreach (string cur in attackList)
            {
                int toHit = int.Parse(cur);
                ToHitBonusList.Add(toHit);
            }

            String[] damageDice = damage.Split(',');
            foreach (string curPool in damageDice)
            {
                var damagePool = PathfinderDicePool.ParseString(curPool);

                _damageDice.Add(damagePool);
            }

            //CritRange='19-20' CritMultiplier='2'
            LowestValueToCrit = 20;
            if (critRange != null)
            {
                String[] critLowest = critRange.Split('-');
                LowestValueToCrit = int.Parse(critLowest[0]);
            }
            CritMultipier = 2;
            if (critMultiplier != null)
            {
                CritMultipier = int.Parse(critMultiplier);
            }
        }

        [DataMember]
        public List<String> DamageDescriptors
        {
            get { return _damageDescriptors; }
            set { _damageDescriptors = value; }
        }

        [DataMember]
        public int CritMultipier { get; set; }

        [DataMember]
        public int LowestValueToCrit { get; set; }

        [DataMember]
        public List<int> ToHitBonusList
        {
            get { return _toHitBonusList; }
            set { _toHitBonusList = value; }
        }

        [DataMember]
        public List<PathfinderDicePool> DamageDice
        {
            get { return _damageDice; }
            set { _damageDice = value; }
        }

        //Copy constructor.public PathfinderAttackTrait(string label, int attackValue, List<String> damageDescriptors, List<int> toHitBonusList, List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)


        public override object BaseTraitContents
        {
            get
            {
                string attacksCsv = string.Join("/", ToHitBonusList.ToArray());
                return attacksCsv;
            }
            set
            {
                if(value==null)
                    return;
                var bonuses = value.ToString().Split('\\');
                List<int> newBonusHitList = new List<int>();
                bool allValid = true;
                foreach (var cur in bonuses)
                {
                    int curBonus = 0;
                    if (!int.TryParse(cur, out curBonus))
                    {
                        allValid = false;
                        break;
                    }
                    newBonusHitList.Add(curBonus);
                }
                if (allValid)
                {
                    ToHitBonusList.Clear();
                    ToHitBonusList = newBonusHitList;
                }
            }
        }


        public override String TraitDescription
        {
            get
            {
                String result = TraitLabel + " ";
                for (int curIndex = 0; curIndex < ToHitBonusList.Count; curIndex++)
                {
                    if (curIndex == 0)
                    {
                        result = result + ToHitBonusList[curIndex];
                    }
                    else
                    {
                        result = result + "/" + ToHitBonusList[curIndex];
                    }
                }
                result = result + " -- ";
                for (int curIndex = 0; curIndex < DamageDice.Count; curIndex++)
                {
                    if (curIndex == 0)
                    {
                        result = result + DamageDice[curIndex];
                    }
                    else
                    {
                        result = result + ", " + DamageDice[curIndex];
                    }
                }
                for (int curIndex = 0; curIndex < DamageDescriptors.Count; curIndex++)
                {
                    if (curIndex == 0)
                    {
                        result = result + " " + DamageDescriptors[curIndex];
                    }
                    else
                    {
                        result = result + ", " + DamageDescriptors[curIndex];
                    }
                }
                return result;
            }
        }

        public override Trait CopyTrait()
        {
            //public PathfinderAttackTrait(string label, int attackValue, List<String> damageDescriptors,
            //List<int> toHitBonusList, List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)
            List<String> descriptors = DamageDescriptors.ToList();
            List<int> toHit = ToHitBonusList.ToList();
            var dDice = new List<PathfinderDicePool>();
            foreach (PathfinderDicePool cur in DamageDice)
            {
                dDice.Add(cur.CopyPool());
            }
            var copyTrait = new PathfinderAttackTrait(TraitLabel, TraitValue, DefenseTarget, descriptors, toHit, dDice,
                                                      LowestValueToCrit, CritMultipier);
            return copyTrait;
        }
    }
}