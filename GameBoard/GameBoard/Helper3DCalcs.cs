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


        public static List<Point3D> CirclePoints(double radius, Point3D origin)
        {
            List<Point3D> circlePoints = new List<Point3D>();
            //int pointsCount = 360;
            //double slice = 2 * Math.PI / pointsCount;
            for (int angleIndex = 0; angleIndex < 361; angleIndex++)
            {
                //double angle = slice * i;
                //int newX = (int)(center.X + radius * Math.Cos(angle));
                //int newY = (int)(center.Y + radius * Math.Sin(angle));
                Point3D p = PointOnCircle(radius, angleIndex, origin);
                circlePoints.Add(p);
            }
            return circlePoints;
        }
        //Burgled from:
        //http://stackoverflow.com/questions/839899/how-do-i-calculate-a-point-on-a-circles-circumference
        private static Point3D PointOnCircle(double radius, double angleInDegrees, Point3D origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            double x = (double)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.X;
            double y = (double)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.Y;

            return new Point3D(x, y, 10);
        }
    }
}
