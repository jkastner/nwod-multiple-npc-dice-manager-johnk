using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class PathfinderAttackTrait : AttackTrait, IPathfinderTrait
    {


        private List<String> _damageDescriptors = new List<String>();
        [DataMember]
        public List<String> DamageDescriptors
        {
            get { return _damageDescriptors; }
            set { _damageDescriptors = value; }
        }

        private int _critMultiplier;
        [DataMember]
        public int CritMultipier
        {
            get { return _critMultiplier; }
            set { _critMultiplier = value; }
        }

        private int _lowestValueToCrit;
        [DataMember]
        public int LowestValueToCrit
        {
            get { return _lowestValueToCrit; }
            set { _lowestValueToCrit = value; }
        }

        private List<int> _toHitBonusList = new List<int>();
        [DataMember]
        public List<int> ToHitBonusList
        {
            get { return _toHitBonusList; }
            set { _toHitBonusList = value; }
        }

        private List<PathfinderDicePool> _damageDice = new List<PathfinderDicePool>();
        [DataMember]
        public List<PathfinderDicePool> DamageDice
        {
            get { return _damageDice; }
            set { _damageDice = value; }
        }

        //Copy constructor.public PathfinderAttackTrait(string label, int attackValue, List<String> damageDescriptors, List<int> toHitBonusList, List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)
        private PathfinderAttackTrait(string label, int attackValue, String targetDefense, List<String> damageDescriptors, List<int> toHitBonusList, List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)
            : base(label, attackValue, targetDefense, damageDescriptors.ToString())
        {
            this.TraitLabel = label;
            this.TraitValue = attackValue;
            this.DamageDescriptors =  damageDescriptors;
            this.ToHitBonusList = toHitBonusList;
            this.DamageDice = damageDice;
            this.LowestValueToCrit = lowestValueToCrit;
            this.CritMultipier = critMultipier;

        }


        public override object BaseTraitContents
        {
            get
            {
                string attacksCsv = string.Join("/", ToHitBonusList.ToArray());
                return attacksCsv;
            }
            set
            {
                //Todo - change attack bonuses?
            }
        }


        public PathfinderAttackTrait(string label, int attackValue, string descriptors, string attackbonuses,
            string damage, string triggers, string triggeredeffects, string targetDefense, string critRange, string critMultiplier)
            //public AttackTrait(string label, int value, String defenseTarget, String damageType) :
            : base(label, attackValue, targetDefense, descriptors)
        {
            //string triggers, string triggeredeffects, string critRange, string critMultiplier
            String[] descriptorList = descriptors.Split(',');
            foreach (var cur in descriptorList)
            {
                DamageDescriptors.Add(cur.Trim());
            }
            String[] attackList = attackbonuses.Split('/');
            foreach (var cur in attackList)
            {
                int toHit = int.Parse(cur);
                ToHitBonusList.Add(toHit);
            }
            
            String [] damageDice = damage.Split(',');
            foreach (var curPool in damageDice)
            {
                //Damage='2d8+13'
                String[] diceTypes = curPool.Split('d');
                String[] secondHalf = diceTypes[1].Split('+');
                int diceQuantity = int.Parse(diceTypes[0]);
                int dieType = int.Parse(secondHalf[0]);
                int modifier = 0;
                if (secondHalf.Length == 2)
                {
                    modifier = int.Parse(secondHalf[1]);
                }
                PathfinderDicePool damagePool = new PathfinderDicePool(diceQuantity, dieType, modifier);
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

        public override Trait CopyTrait()
        {
            //public PathfinderAttackTrait(string label, int attackValue, List<String> damageDescriptors,
            //List<int> toHitBonusList, List<PathfinderDicePool> damageDice, int lowestValueToCrit, int critMultipier)
            List<String> descriptors = DamageDescriptors.ToList();
            List<int> toHit = ToHitBonusList.ToList();
            List<PathfinderDicePool> dDice = new List<PathfinderDicePool>();
            foreach(var cur in this.DamageDice)
            {
                dDice.Add(cur.CopyPool());
            }
            var copyTrait = new PathfinderAttackTrait(this.TraitLabel, this.TraitValue, this.DefenseTarget, descriptors, toHit, dDice, this.LowestValueToCrit, this.CritMultipier);
            return copyTrait;
                        
        }
        public override String TraitDescription
        {
            get
            {
                String result = TraitLabel+" ";
                for(int curIndex=0;curIndex<ToHitBonusList.Count;curIndex++)
                {
                    if(curIndex == 0)
                    {
                        result = result +ToHitBonusList[curIndex];
                    }
                    else
                    {
                        result = result + "/" + ToHitBonusList[curIndex];
                    }
                }
                result = result +" -- ";
                for (int curIndex = 0; curIndex < DamageDice.Count; curIndex++)
                {
                    if (curIndex == 0)
                    {
                        result = result + DamageDice[curIndex].ToString();
                    }
                    else
                    {
                        result = result + ", " + DamageDice[curIndex].ToString();
                    }
                }
                for (int curIndex = 0; curIndex < DamageDescriptors.Count; curIndex++)
                {
                    if (curIndex == 0)
                    {
                        result = result +" "+ DamageDescriptors[curIndex].ToString();
                    }
                    else
                    {
                        result = result + ", " + DamageDescriptors[curIndex];
                    }
                } 
                return result;
            }
        }
    }
}
