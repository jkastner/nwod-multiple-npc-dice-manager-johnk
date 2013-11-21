using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace CombatAutomationTheater
{
    [DataContract(Namespace = "")]
    public class Team
    {
        private readonly Brush _teamBrush;
        private ObservableCollection<CharacterSheet> _teamMembers = new ObservableCollection<CharacterSheet>();

        public Team(string p, Color color)
        {
            // TODO: Complete member initialization
            TeamName = p;
            TeamColor = color;
            _teamBrush = new SolidColorBrush(TeamColor);
        }

        /// <summary>
        /// The 'unassigned' team is a special case. Technically, they will be in this list of 'teammembers' but will not consider their 
        /// teams to be equals. This allows for a collection for all the free-for-all team members, but without making them equal for 
        /// purposes of target selection etc.
        /// </summary>
        [DataMember]
        public ObservableCollection<CharacterSheet> TeamMembers
        {
            get { return _teamMembers; }
            set 
            { _teamMembers = value; }
        }

        [DataMember]
        public Color TeamColor { get; set; }

        public Brush TeamBrush
        {
            get { return _teamBrush; }
        }

        [DataMember]
        public String TeamName { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as Team;
            if (t == null)
            {
                return false;
            }
            if (this==RosterViewModel.UnassignedTeam||t==RosterViewModel.UnassignedTeam)
            {
                return false;
            }
            return this == t;
        }
     

    }
}