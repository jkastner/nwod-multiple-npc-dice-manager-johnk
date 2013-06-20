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
    public class CombatService
    {

        private static RosterViewModel _rosterViewModel = new RosterViewModel();
        public static RosterViewModel RosterViewModel
        {
            get
            {
                return _rosterViewModel;
            }
        }

        private static VisualsViewModel _visualsViewModel = new VisualsViewModel();
        public static VisualsViewModel VisualsViewModel
        {
            get
            {
                return _visualsViewModel;
            }
        }

        private static GameBoardVisual _gameBoardVisual = new GameBoardVisual();
        public static GameBoardVisual GameBoardVisual 
        {
            get
            {
                return _gameBoardVisual;
            }
        }
    }
}
