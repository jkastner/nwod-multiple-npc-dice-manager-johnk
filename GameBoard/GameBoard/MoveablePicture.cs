using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace GameBoard
{
    public class MoveablePicture
    {
        private RectangleVisual3D _charImage;
        public RectangleVisual3D CharImage
        {
            get { return _charImage; }
            set { _charImage = value; }
        }

        private RotateManipulator _rotator;
        public RotateManipulator Rotator
        {
            get { return _rotator; }
            set { _rotator = value; }
        }

        private TruncatedConeVisual3D _baseCone;

        public TruncatedConeVisual3D BaseCone
        {
            get { return _baseCone; }
            set { _baseCone = value; }
        }

        public TubeVisual3D MovementCircle { get; set; }
        public TubeVisual3D DoubleMovementCircle { get; set; }

        public String AdditionalDisplayText { get; set; }

        public BillboardTextVisual3D InfoText
        { get; set; }

        private List<Visual3D> _associatedVisuals = new List<Visual3D>();
        public List<Visual3D> AssociatedVisuals
        {
            get { return _associatedVisuals; }
            set { _associatedVisuals = value; }
        }
        

        private double _pictureOffset;

        public double PictureOffset
        {
            get { return _pictureOffset; }
            set { _pictureOffset = value; }
        }

        private double _length;
	    public double Length
	    {
		    get { return _length;}
		    set { _length = value;}
	    }

        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double Speed { get; set; }

        public Color PieceColor { get; set; }

        public double PictureLength { get; set; }

        public Point3D Location
        {
            get { return CharImage.Origin; }
            set { MoveTo(value); }
        }


        public MoveablePicture(String pictureFile, double width, double length, String imageName, Color pieceColor, Point3D location, List<StatusEffectDisplay> statuses)
        {
            PieceColor = pieceColor;
            Name = imageName;
            _charImage = ImageToRectangle(pictureFile, width, length, location);
            AssociatedVisuals.Add(BaseCone);
            AssociatedVisuals.Add(MovementCircle);
            AssociatedVisuals.Add(DoubleMovementCircle);
            AssociatedVisuals.Add(CharImage);
            PictureLength = length;
            RemakeInfoText(statuses);
            AssociatedVisuals.Add(InfoText);
        }

        public void RemakeInfoText(List<StatusEffectDisplay> statuses)
        {
            if (InfoText == null)
            {
                InfoText = new BillboardTextVisual3D()
                {
                    Position = new Point3D(
                        CharImage.Origin.X, CharImage.Origin.Y,
                        CharImage.Origin.Z + PictureOffset + PictureLength + 3),
                    FontSize = 18,
                    Background= new SolidColorBrush(Colors.Wheat),
                };
            }

            InfoText.Text = Name;
            foreach (var cur in statuses)
            {
                InfoText.Text = InfoText.Text + "\n   " + cur.Description + " -- R:" + cur.TurnsRemaining;
            }
        }


        private RectangleVisual3D ImageToRectangle(String imageFile, double width, double length, Point3D location)
        {
            ImageBrush frontBrush =  MaterialMaker.MakeImageMaterial(imageFile);
            Material frontMaterial = new DiffuseMaterial(frontBrush);
            PictureOffset = 3.5;
            _length = length;
            RectangleVisual3D charPic = new RectangleVisual3D()
            {
                Material = frontMaterial,
                BackMaterial = MaterialMaker.PaperbackMaterial(),
                Length = _length,
                Width = width,
                Origin = new Point3D(location.X, location.Y, PictureOffset),
                Normal = new Vector3D(0.00001, 0.00001, 1),
                LengthDirection = new Vector3D(0, 0, -1),
            };
            //charPic.Transform = new RotateTransform3D(new RotateTransform3D(

            //RectangleVisual3D charPic = new RectangleVisual3D()
            //{
            //    Material = frontMaterial,
            //    TopFace=false,
            //    BottomFace=false,
            //    BackMaterial = backMaterial,
            //    Length=length,
            //    Width=width,
            //    Height=height,
            //    Center = new Point3D(0, 0, height/2+5),
            //};
            BaseCone = new TruncatedConeVisual3D()
            {
                Height = PictureOffset-1,
                BaseRadius = length/3,
                TopRadius = length/3,
                Material = new DiffuseMaterial(new SolidColorBrush(PieceColor)),
                Origin = new Point3D(location.X, location.Y, 0),
            };
            MovementCircle = new TubeVisual3D()
            {
                Diameter = .5,
                Material = new DiffuseMaterial(new SolidColorBrush(PieceColor)),
                ThetaDiv = 15,
            };
            DoubleMovementCircle = new TubeVisual3D()
            {
                Diameter = .5,
                Material = new DiffuseMaterial(new SolidColorBrush(PieceColor)),
                ThetaDiv = 15,
            };
            return charPic;

        }

        int c = 1;
        internal void AddaRotateTranslator(Point3D location)
        {
            Color color = Colors.Gray;
            location.Z = location.Z + 50;
            RotateManipulator around = new RotateManipulator()
            {
                Length = _pictureOffset,
                Diameter = _length*c,
                InnerDiameter = _length * .4 * c,
                Color = color,
                Axis = new Vector3D(0, 0, 1),
                Position = location,
            };
            around.Bind(CharImage);
            _rotator = around;
        }


        internal void MoveTo(Point3D point3D)
        {
            Point3DAnimation moveAnimationPic = new Point3DAnimation(CharImage.Origin, point3D, new Duration(new TimeSpan(0,0,0,1)));
            Point3DAnimation moveAnimationCone = new Point3DAnimation(BaseCone.Origin, new Point3D(point3D.X, point3D.Y, 0), new Duration(new TimeSpan(0, 0, 0, 1)));
            Point3DAnimation textAnimation = new Point3DAnimation(InfoText.Position, new Point3D(point3D.X, point3D.Y, InfoText.Position.Z), new Duration(new TimeSpan(0, 0, 0, 1)));
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            AnimationClock clock2 = moveAnimationCone.CreateClock();
            AnimationClock clock3 = textAnimation.CreateClock();
            CharImage.ApplyAnimationClock(RectangleVisual3D.OriginProperty, clock1);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.OriginProperty, clock2);
            InfoText.ApplyAnimationClock(BillboardTextVisual3D.PositionProperty, clock3);
            //CharImage.Origin = point3D;
            //BaseCone.Origin = ;
        }



        internal void StopActive()
        {
            _isActive = false;
            if (activeAnimation != null)
            {
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, null);
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, null);
            }
        }

        DoubleAnimation activeAnimation = null;
        bool _isActive = false;
        internal void StartActive()
        {
            _isActive = true;
            activeAnimation = new DoubleAnimation(BaseCone.TopRadius, BaseCone.TopRadius * 1.5, new Duration(new TimeSpan(0, 0, 0, 0, 800)));
            activeAnimation.AutoReverse = true;
            activeAnimation.RepeatBehavior = RepeatBehavior.Forever;
            AnimationClock _activeClock = activeAnimation.CreateClock();
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, _activeClock);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, _activeClock);
            Point3DCollection circlePoints = CirclePoints(Speed);
            Point3DCollection doubleCirclePoints = CirclePoints(Speed*2);
            MovementCircle.Path = circlePoints;
            DoubleMovementCircle.Path = doubleCirclePoints;
        }

        private Point3DCollection CirclePoints(double radius)
        {
            List<Point3D> circlePoints = new List<Point3D>();
            //int pointsCount = 360;
            //double slice = 2 * Math.PI / pointsCount;
            Point3D center = CharImage.Origin;
            for (int angleIndex = 0; angleIndex < 361; angleIndex++)
            {
                //double angle = slice * i;
                //int newX = (int)(center.X + radius * Math.Cos(angle));
                //int newY = (int)(center.Y + radius * Math.Sin(angle));
                Point3D p = PointOnCircle(radius, angleIndex, center);
                circlePoints.Add(p);
            }
            Point3DCollection points = new Point3DCollection(circlePoints);
            return points;
        }
        //Burgled from:
        //http://stackoverflow.com/questions/839899/how-do-i-calculate-a-point-on-a-circles-circumference
        public static Point3D PointOnCircle(double radius, double angleInDegrees, Point3D origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            double x = (double)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.X;
            double y = (double)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.Y;

            return new Point3D(x, y, 10);
        }
           

    }
}
