﻿using GameBoard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    [DataContract]
    public class Combat
    {

        [DataMember]
        public IList<CharacterSheet> ActiveRoster { get; set; }
        [DataMember]
        public IList<CharacterSheet> DeceasedRoster { get; set; }
        [DataMember]
        public BoardInfo BoardInfo { get; set; }

        public Combat(IList<CharacterSheet> active, IList<CharacterSheet> deceased, BoardInfo boardInfo)
        {
            // TODO: Complete member initialization
            ActiveRoster = active;
            DeceasedRoster = deceased;
            this.BoardInfo = boardInfo;
        }



    }
}