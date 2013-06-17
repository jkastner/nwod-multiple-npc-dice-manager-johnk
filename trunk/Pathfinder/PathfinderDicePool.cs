using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderDicePool : DicePool
    {
        public int DiceQuantity;
        public int DieType;
        public int Modifier;

        public PathfinderDicePool(int diceQuantity, int dieType, int modifier)
        {
            this.DiceQuantity = diceQuantity;
            this.DieType = dieType;
            this.Modifier = modifier;
        }
        internal override void Roll()
        {
            int total = 0;
            for (int curIndex = 0; curIndex < DiceQuantity; curIndex++)
            {
                var rollResult = _theRandomGenerator.Next(1, DieType+1);
                _resultDescription = _resultDescription + rollResult+" (d" + DieType + ")";
                total += rollResult;
            }
            _resultDescription = _resultDescription + "+" + Modifier + "=";
            total += Modifier;
            _totalValue = total;
            _resultDescription = _resultDescription + total;
        }

        private String _resultDescription = "";
        public override String ResultDescription
        {
            get
            {
                return _resultDescription;
            }
            set
            {
                _resultDescription = value;
            }
        }

        public String PoolDescription
        {
            get
            {
                return DiceQuantity + "d" + DieType + "+" + Modifier;
            }
        }

        private int _totalValue;
        public int TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }
        

        public PathfinderDicePool CopyPool()
        {
            return new PathfinderDicePool(this.DiceQuantity, this.DieType, this.Modifier);
        }

        public override string ToString()
        {
            String result = DiceQuantity + "d" + DieType + " + " + Modifier;
            return result;
        }

    }
}
