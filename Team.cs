using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace XMLCharSheets
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

        [DataMember]
        public ObservableCollection<CharacterSheet> TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; }
        }

        [DataMember]
        public Color TeamColor { get; set; }

        public Brush TeamBrush
        {
            get { return _teamBrush; }
        }


        [DataMember]
        public String TeamName { get; set; }
    }
}