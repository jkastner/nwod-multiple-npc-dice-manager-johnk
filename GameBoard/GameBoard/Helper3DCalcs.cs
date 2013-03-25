﻿using System;
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

        //Taken entirely from:
        //http://www.flipcode.com/archives/Fast_Point-In-Cylinder_Test.shtml
        public static bool PointIsInsideCylinder(Vector3D testpt, Vector3D pt1, Vector3D pt2, double lineDiameter, double lengthsq)
        {
            double lineRadius = lineDiameter;
            double radius_sq = Math.Pow(lineRadius, 2);
	                        
            double dx, dy, dz;	// vector d  from line segment point 1 to point 2
	        double pdx, pdy, pdz;	// vector pd from point 1 to test point
            double dot, dsq;

	        dx = pt2.X - pt1.X;	// translate so pt1 is origin.  Make vector from
	        dy = pt2.Y - pt1.Y;     // pt1 to pt2.  Need for this is easily eliminated
	        dz = pt2.Z - pt1.Z;

	        pdx = testpt.X - pt1.X;		// vector from pt1 to test point.
	        pdy = testpt.Y - pt1.Y;
	        pdz = testpt.Z - pt1.Z;

	        // Dot the d and pd vectors to see if point lies behind the 
	        // cylinder cap at pt1.x, pt1.y, pt1.z

	        dot = pdx * dx + pdy * dy + pdz * dz;

	        // If dot is less than zero the point is behind the pt1 cap.
	        // If greater than the cylinder axis line segment length squared
	        // then the point is outside the other end cap at pt2.

	        if( dot < 0.0f || dot > lengthsq )
	        {
		        //return( -1.0f );
                return false;
	        }
	        else 
	        {
		        // Point lies within the parallel caps, so find
		        // distance squared from point to line, using the fact that sin^2 + cos^2 = 1
		        // the dot = cos() * |d||pd|, and cross*cross = sin^2 * |d|^2 * |pd|^2
		        // Carefull: '*' means mult for scalars and dotproduct for vectors
		        // In short, where dist is pt distance to cyl axis: 
		        // dist = sin( pd to d ) * |pd|
		        // distsq = dsq = (1 - cos^2( pd to d)) * |pd|^2
		        // dsq = ( 1 - (pd * d)^2 / (|pd|^2 * |d|^2) ) * |pd|^2
		        // dsq = pd * pd - dot * dot / lengthsq
		        //  where lengthsq is d*d or |d|^2 that is passed into this function 

		        // distance squared to the cylinder axis:

		        dsq = (pdx*pdx + pdy*pdy + pdz*pdz) - dot*dot/lengthsq;

		        if( dsq > radius_sq )
		        {
			        //return( -1.0f );
                    return false;
		        }
		        else
		        {
                    return true;
                    //return( dsq );		// return distance squared to axis
		        }
	        }

        }

    }
}
