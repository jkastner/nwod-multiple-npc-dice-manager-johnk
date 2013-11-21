using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameBoard;

namespace CombatAutomationTheater
{
    [DataContract]
    public class Combat
    {
        public Combat(IList<CharacterSheet> active, IList<CharacterSheet> deceased, IList<Team> teams, IList<Board> boards,
                      String currentOutput)
        {
            ActiveRoster = active;
            DeceasedRoster = deceased;
            Teams = teams;
            Boards = boards;
            OutputText = currentOutput;
        }

        [DataMember]
        public IList<CharacterSheet> ActiveRoster { get; set; }

        [DataMember]
        public IList<Team> Teams { get; set; }

        [DataMember]
        public IList<CharacterSheet> DeceasedRoster { get; set; }

        [DataMember]
        public IList<Board> Boards{ get; set; }

        [DataMember]
        public String OutputText { get; set; }
    }
}