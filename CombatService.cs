﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using GameBoard;

namespace CombatAutomationTheater
{
    [DataContract]
    public class CombatService
    {
        private static readonly RosterViewModel _rosterViewModel = new RosterViewModel();


        public static RosterViewModel RosterViewModel
        {
            get { return _rosterViewModel; }
        }

        

        

    }
}