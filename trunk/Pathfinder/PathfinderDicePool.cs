using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PathfinderDicePool : DicePool
    {
        private int diceQuantity;
        private int dieType;
        private int modifier;

        public PathfinderDicePool(int diceQuantity, int dieType, int modifier)
        {
            this.diceQuantity = diceQuantity;
            this.dieType = dieType;
            this.modifier = modifier;
        }
        internal override void Roll()
        {
            int total = 0;
            for (int curIndex = 0; curIndex < diceQuantity; curIndex++)
            {
                var rollResult = _theRandomGenerator.Next(1, dieType);
                _resultDescription = _resultDescription + "(d" + dieType + ") " + rollResult;
                total += rollResult;
            }
            _resultDescription = _resultDescription + "+" + modifier + "=";
            total += modifier;
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

        private int _totalValue;
        public int TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }
        

        public PathfinderDicePool CopyPool()
        {
            return new PathfinderDicePool(this.diceQuantity, this.dieType, this.modifier);
        }

        public override string ToString()
        {
            String result = diceQuantity + "d" + dieType + " + " + modifier;
            return result;
        }

    }
}
