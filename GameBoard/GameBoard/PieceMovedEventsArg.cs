using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class PieceMovedEventsArg : EventArgs
    {
        public MoveablePicture Mover { get; set; }
        public PieceMovedEventsArg(MoveablePicture mover)
        {
            Mover = mover;
        }

    }
}
