using GameBoard;
using System;
using System.Collections.Generic;
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
        private RosterViewModel _rosterViewModel = new RosterViewModel();
        public RosterViewModel RosterViewModel
        {
            get
            {
                return _rosterViewModel;
            }
            private set
            {
                _rosterViewModel = value;
            }
        }

        [DataMember]
        private VisualsViewModel _visualsViewModel = new VisualsViewModel();
        public  VisualsViewModel VisualsViewModel
        {
            get
            {
                return _visualsViewModel;
            }
            private set
            {
                _visualsViewModel = value;
            }
        }
    }
}
