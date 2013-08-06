using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class PathfinderDicePool : DicePool
    {
        [DataMember] public int DiceQuantity;
        [DataMember] public int DieType;
        [DataMember] public int Modifier;

        private String _resultDescription = "";
        private int _totalValue;

        public PathfinderDicePool(int diceQuantity, int dieType, int modifier)
        {
            DiceQuantity = diceQuantity;
            DieType = dieType;
            Modifier = modifier;
        }

        [DataMember]
        public override String ResultDescription
        {
            get { return _resultDescription; }
            set { _resultDescription = value; }
        }
        public String PoolDescription
        {
            get { return DiceQuantity + "d" + DieType + "+" + Modifier; }
        }

        [DataMember]
        public int TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }

        internal override void Roll()
        {
            int total = 0;
            for (int curIndex = 0; curIndex < DiceQuantity; curIndex++)
            {
                int rollResult = _theRandomGenerator.Next(1, DieType + 1);
                if (!String.IsNullOrWhiteSpace(_resultDescription))
                {
                    _resultDescription = _resultDescription + " + ";
                }
                _resultDescription = _resultDescription + rollResult + "(d" + DieType + ")";
                total += rollResult;
            }
            String modDesc = "";
            if (Modifier > 0)
            {
                modDesc = "+";
            }
            else if (Modifier < 0)
            {
                modDesc = "-";
            }

            _resultDescription = _resultDescription + modDesc + Modifier + "=";
            total += Modifier;
            _totalValue = total;
            _resultDescription = _resultDescription + total;
        }


        public PathfinderDicePool CopyPool()
        {
            return new PathfinderDicePool(DiceQuantity, DieType, Modifier);
        }

        public override string ToString()
        {
            String result = DiceQuantity + "d" + DieType + " + " + Modifier;
            return result;
        }

        //Damage='2d8+13'
        //1d4
        //1d4-20
        internal static PathfinderDicePool ParseString(string poolDescription)
        {
            poolDescription = poolDescription.ToLower().Trim();
            poolDescription = Regex.Replace(poolDescription, @"\s+", "");
            String[] diceTypes = poolDescription.Split('d');
            if(diceTypes.Length!=2)
            {
                return null;
            }
            //DiceTypes [0]=1  //DiceTypes [1]=d4+2  
            char plusOrMinus = '+';
            if (diceTypes[1].Contains("-"))
            {
                plusOrMinus = '-';
            }
            String[] secondHalf = diceTypes[1].Split(plusOrMinus);
            //secondHalf[0] = type   secondHalf[1] = modifier if any.
            int diceQuantity = 0;
            if (!int.TryParse(diceTypes[0], out diceQuantity))
            {
                return null;
            }
            int dieType = 0;
            if(!int.TryParse(secondHalf[0], out dieType))
            {
                return null;
            }
            int modifier = 0;
            if (secondHalf.Length == 2)
            {
                if (!int.TryParse(secondHalf[1], out modifier))
                {
                    return null;
                }
            }
            if (plusOrMinus == '-')
            {
                modifier = modifier * -1;
            }
            if (diceQuantity < 1||dieType<1)
            {
                return null;
            }
            return new PathfinderDicePool(diceQuantity, dieType, modifier);
        }
    }
}