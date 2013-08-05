using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameBoard;

namespace XMLCharSheets
{
    [DataContract]
    public class Combat
    {
        public Combat(IList<CharacterSheet> active, IList<CharacterSheet> deceased, IList<Team> teams, BoardInfo boardInfo,
                      String currentOutput)
        {
            ActiveRoster = active;
            DeceasedRoster = deceased;
            Teams = teams;
            BoardInfo = boardInfo;
            OutputText = currentOutput;
        }

        [DataMember]
        public IList<CharacterSheet> ActiveRoster { get; set; }

        [DataMember]
        public IList<Team> Teams { get; set; }

        [DataMember]
        public IList<CharacterSheet> DeceasedRoster { get; set; }

        [DataMember]
        public BoardInfo BoardInfo { get; set; }

        [DataMember]
        public String OutputText { get; set; }
    }
}