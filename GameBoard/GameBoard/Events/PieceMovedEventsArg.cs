using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GameBoard
{
    public class PieceMovedEventsArg : EventArgs
    {
        public Guid MoverID { get; set; }
        public Point3D Destination { get; set; }
        public PieceMovedEventsArg(Guid mover, Point3D destination)
        {
            MoverID = mover;
            Destination = destination;
        }

    }
}
