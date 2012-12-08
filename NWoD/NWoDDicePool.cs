using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class NWoDDicePool
    {
        static Random _theRandomGenerator = new Random();
        private int _numberOfDice;

        public int NumberOfDice
        {
            get { return _numberOfDice; }
            set
            {
                _numberOfDice = value;
            }
        }

        private int _currentSuccesses;
        public int CurrentSuccesses
        {
            get { return _currentSuccesses; }
            set { _currentSuccesses = value; }
        }

        private String _resultDescription;
        private int p;
        public String ResultDescription
        {
            get { return _resultDescription; }
            set { _resultDescription = value; }
        }

        public int ExplodesOn { get; set; }
        public int SubtractsOn { get; set; }
        public int AutomaticExtaSuccessesOnSuccess { get; set; } 


        public NWoDDicePool(NWoDTrait curTrait)
        {
            NumberOfDice = curTrait.TraitValue;
            ExplodesOn = curTrait.ExplodesOn;
            SubtractsOn = curTrait.SubtractsOn;

        }

        internal void Roll()
        {
            int maxSides = 11;
            int minSuccess = 8;
            int minAgain = ExplodesOn;
            int uberFail = 0;
            CurrentSuccesses = 0;
            _resultDescription = "";
            int numInPool = NumberOfDice;
            if(NumberOfDice<=0)
            {
                numInPool = 1;
                minSuccess = 10;
                uberFail = (NumberOfDice*-1)+1;
            }
            RollPool(numInPool, maxSides, minSuccess, minAgain, uberFail, SubtractsOn);
            
        }

        private void RollPool(int numInPool, int maxSides, int minSuccess, int minAgain, int uberFail, int subtractsOn)
        {
            if (uberFail >= maxSides)
            {
                _resultDescription = "Automatic dramatic failure.";
                return;
            }
            for (int i = 0; i < numInPool; i++)
            {
                int result = _theRandomGenerator.Next(1, maxSides);
                if (String.IsNullOrEmpty(_resultDescription))
                {
                    _resultDescription = result.ToString();
                }
                else
                {
                    _resultDescription = _resultDescription + ", " + result.ToString();
                }
                if (result >= minSuccess)
                {
                    CurrentSuccesses++;
                }
                while (result == minAgain)
                {
                    result = _theRandomGenerator.Next(1, maxSides);
                    _resultDescription = _resultDescription + "->" + result.ToString();
                    if (result >= minSuccess)
                    {
                        CurrentSuccesses++;
                    }
                }
                if(result<= subtractsOn)
                {
                    CurrentSuccesses--;
                    if (CurrentSuccesses < 0)
                        CurrentSuccesses = 0;
                }
                if (CurrentSuccesses == 0 && result <= uberFail)
                {
                    _resultDescription = "Dramatic failure -- "+_resultDescription;
                    return;
                }
            }
        }


    }
}
