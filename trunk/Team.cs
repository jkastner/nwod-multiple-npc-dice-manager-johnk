using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class Team
    {
        private ObservableCollection<CharacterSheet> _teamMembers = new ObservableCollection<CharacterSheet>();
        [DataMember]
        public ObservableCollection<CharacterSheet> TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; }
        }

        private Color _teamColor;
        [DataMember]
        public Color TeamColor
        {
            get { return _teamColor; }
            set { _teamColor = value; }
        }

        private Brush _teamBrush;
        public Brush TeamBrush
        {
            get { return _teamBrush; }
        }
        

        public Team(string p, Color color)
        {
            // TODO: Complete member initialization
            TeamName = p;
            TeamColor = color;
            _teamBrush = new SolidColorBrush(TeamColor);
        }
        private String _teamName;
        [DataMember]
        public String TeamName
        {
            get { return _teamName; }
            set { _teamName = value; }
        }
        


    }
}
