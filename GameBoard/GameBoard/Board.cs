using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GameBoard
{
    [DataContract]
    public class Board
    {
        [DataMember]
        public string BoardName
        {
            get;
            private set;
        }

        private VisualsViewModel _visualsViewModel = new VisualsViewModel();
        [DataMember]
        public VisualsViewModel VisualsViewModel
        {
            get { return _visualsViewModel; }
            set { _visualsViewModel = value; }
        }

        
        public Board(GameBoardVisual gameBoardVisual, VisualsViewModel visualsViewModel, String boardName)
        {
            // TODO: Complete member initialization
            GameBoardVisual = gameBoardVisual;
            VisualsViewModel = visualsViewModel;
            VisualsViewModel.SetInitialBackground();
            BoardName = boardName; 
        }
        
        private GameBoardVisual _gameBoardVisual;
        public GameBoardVisual GameBoardVisual
        {
            get { return _gameBoardVisual; }
            set { _gameBoardVisual = value; }
        }
        
        internal static Board NewBoard(String boardName)
        {
            GameBoardVisual gbv = new GameBoardVisual();
            VisualsViewModel vvm = new VisualsViewModel();
            gbv.RegisterViewModel(vvm);
            vvm.SetGameBoardVisual(gbv);
            return new Board(gbv, vvm, boardName);
        }

        public Dispatcher Dispatcher
        {
            get { return _gameBoardVisual.Dispatcher; }
        }


        internal void Dispose()
        {
            
        }

    }
}
