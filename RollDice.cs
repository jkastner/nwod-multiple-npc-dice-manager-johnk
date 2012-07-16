using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class RollDice
    {
        static Random _theRandomGenerator = new Random();
        private int _numberOfDice;

        public int NumberOfDice
        {
            get { return _numberOfDice; }
            set
            {
                if (value < 0)
                    _numberOfDice = 0;    
                else
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
        public String ResultDescription
        {
            get { return _resultDescription; }
            set { _resultDescription = value; }
        }



        public RollDice(int diceNum)
        {
            _numberOfDice = diceNum;
        }





        int _maxSides = 11;
        internal void Roll()
        {
            CurrentSuccesses = 0;
            _resultDescription = "";
            if(NumberOfDice==0)
            {
                int chanceRoll = _theRandomGenerator.Next(1, _maxSides);
                if(chanceRoll == 10)
                {
                    _resultDescription = 

                }
            }
            for (int i = 0; i < NumberOfDice; i++)
            {
                int result = _theRandomGenerator.Next(1, _maxSides);
                if (String.IsNullOrEmpty(_resultDescription))
                {
                    _resultDescription = result.ToString();
                }
                else
                {
                    _resultDescription = _resultDescription + ", " + result.ToString();
                }
                if (CheckSuccess(result))
                {
                    CurrentSuccesses++;
                }
                while (result == _maxSides-1)
                {
                    result = _theRandomGenerator.Next(1, _maxSides);
                    _resultDescription = _resultDescription+"->"+result.ToString();
                    if (CheckSuccess(result))
                    {
                        CurrentSuccesses++;
                    }
                }
            }
        }

        private bool CheckSuccess(int theResult)
        {
            return theResult > 7;
        }
    }
}
