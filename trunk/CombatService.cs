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

        private static Combat _currentCombat = new Combat();
        public static Combat CurrentCombat
        {
            get
            {
                return _currentCombat;
            }
            set
            {
                _currentCombat = value;
            }

        }
        public static RosterViewModel RosterViewModel
        {
            get
            {
                return _currentCombat.RosterViewModel;
            }
        }

        public static VisualsViewModel VisualsViewModel
        {
            get
            {
                return _currentCombat.VisualsViewModel;
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
