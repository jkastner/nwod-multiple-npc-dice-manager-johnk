using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class PieceSelectedEventArgs : EventArgs
    {
        public PieceSelectedEventArgs(Guid _selectedPiece)
        {
            // TODO: Complete member initialization
            SelectedPieceID = _selectedPiece;
        }
        public Guid SelectedPieceID { get; set; }
    }
}
