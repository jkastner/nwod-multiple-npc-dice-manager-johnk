using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class PieceSelectedEventArgs : EventArgs
    {
        public PieceSelectedEventArgs(MoveablePicture _selectedPiece)
        {
            // TODO: Complete member initialization
            SelectedPiece = _selectedPiece;
        }
        public MoveablePicture SelectedPiece { get; set; }
    }
}
