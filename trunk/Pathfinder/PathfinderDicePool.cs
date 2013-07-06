using System;
using System.Runtime.Serialization;

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
                _resultDescription = _resultDescription + rollResult + " (d" + DieType + ")";
                total += rollResult;
            }
            _resultDescription = _resultDescription + "+" + Modifier + "=";
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
    }
}