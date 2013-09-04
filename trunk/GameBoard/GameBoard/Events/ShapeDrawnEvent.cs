using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GameBoard
{
    public class ShapeDrawnEvent : EventArgs
    {
        //1 point == sphere
        public List<Point3D> Points = new List<Point3D>();

        public ShapeDrawnEvent(IList<Point3D> otherPoints)
        {
            Points.AddRange(otherPoints);
        }
        
    }
}
