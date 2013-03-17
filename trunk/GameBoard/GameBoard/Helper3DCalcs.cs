using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GameBoard
{
    public static class Helper3DCalcs
    {
        public static double DistanceBetween(Point3D p1, Point3D p2)
        {
            double radicand = Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2);
            return Math.Sqrt(radicand);
        }

        public static Point3D MovePointTowards(Point3D a, Point3D b, double distance)
        {
            var vector = new Point3D(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            var length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            var unitVector = new Point3D(vector.X / length, vector.Y / length, vector.Z/length);
            return new Point3D(a.X + unitVector.X * distance, a.Y + unitVector.Y * distance, a.Z + unitVector.Z * distance);
        }

        public static Point3D FindMidpoint(IEnumerable<Point3D> points)
        {
            Point3D commonOrigin = new Point3D();
            foreach (var cur in points)
            {
                commonOrigin.X += cur.X;
                commonOrigin.Y += cur.Y;
                commonOrigin.Z += cur.Z;
            }
            int count = points.Count();
            commonOrigin.X /= count;
            commonOrigin.Y /= count;
            commonOrigin.Z /= count;
            return commonOrigin;
        }
    }
}
