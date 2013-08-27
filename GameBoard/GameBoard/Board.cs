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
        private VisualsViewModel _visualsViewModel = new VisualsViewModel();
        [DataMember]
        public VisualsViewModel VisualsViewModel
        {
            get { return _visualsViewModel; }
            set { _visualsViewModel = value; }
        }

        
        public Board(GameBoardVisual gameBoardVisual, VisualsViewModel visualsViewModel)
        {
            // TODO: Complete member initialization
            GameBoardVisual = gameBoardVisual;
            VisualsViewModel = visualsViewModel;
            VisualsViewModel.Initialize();
        }
        
        private GameBoardVisual _gameBoardVisual;
        public GameBoardVisual GameBoardVisual
        {
            get { return _gameBoardVisual; }
            set { _gameBoardVisual = value; }
        }
        
        internal static Board NewBoard()
        {
            GameBoardVisual gbv = new GameBoardVisual();
            VisualsViewModel vvm = new VisualsViewModel();
            gbv.RegisterViewModel(vvm);
            vvm.SetViewport(gbv.Viewport);
            return new Board(gbv, vvm);
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
