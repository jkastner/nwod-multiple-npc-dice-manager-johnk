﻿using System;
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





        
        internal void Roll()
        {
            int maxSides = 11;
            int minSuccess = 8;
            int minAgain = 10;
            int uberFail = 0;
            CurrentSuccesses = 0;
            _resultDescription = "";
            int numInPool = NumberOfDice;
            if(NumberOfDice==0)
            {
                numInPool = 1;
                minSuccess = 10;
                uberFail = 1;
            }
            RollPool(numInPool, maxSides, minSuccess, minAgain, uberFail);
            
        }

        private void RollPool(int numInPool, int maxSides, int minSuccess, int minAgain, int uberFail)
        {
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
                if(result == uberFail)
                {
                    _resultDescription = "Dramatic failure.";
                    return;
                }
            }
        }


    }
}
