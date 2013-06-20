using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class VisualsService
    {
        private static HelixViewport3D _viewport;
        public static HelixViewport3D Viewport
        {
            get { return _viewport; }
            set { _viewport = value; }
        }

        private static GameBoardVisual _gameBoardVisual;
        public static GameBoardVisual GameBoardVisual
        {
            get { return _gameBoardVisual; }
            set { _gameBoardVisual = value; }
        }

    }
}
