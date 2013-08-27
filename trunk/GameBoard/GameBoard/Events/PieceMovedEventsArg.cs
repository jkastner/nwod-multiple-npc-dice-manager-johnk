using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class PieceMovedEventsArg : EventArgs
    {
        public Guid MoverID { get; set; }
        public PieceMovedEventsArg(Guid mover)
        {
            MoverID = mover;
        }

    }
}
