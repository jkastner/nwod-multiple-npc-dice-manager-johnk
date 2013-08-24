using System.Runtime.Serialization;
using GameBoard;

namespace XMLCharSheets
{
    [DataContract]
    public class CombatService
    {
        private static readonly RosterViewModel _rosterViewModel = new RosterViewModel();

        private static readonly VisualsViewModel _visualsViewModel = new VisualsViewModel();

        private static readonly GameBoardVisual _gameBoardVisual = new GameBoardVisual();

        public static RosterViewModel RosterViewModel
        {
            get { return _rosterViewModel; }
        }

        public static VisualsViewModel VisualsViewModel
        {
            get { return _visualsViewModel; }
        }

        public static GameBoardVisual GameBoardVisual
        {
            get { return _gameBoardVisual; }
        }

    }
}