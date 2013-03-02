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



        public Color PieceColor { get; set; }
        
        public MoveablePicture(String pictureFile, double width, double length, String imageName, Color pieceColor)
        {
            PieceColor = pieceColor;
            Name = imageName;
            _charImage = ImageToRectangle(pictureFile, width, length);
        }

        private RectangleVisual3D ImageToRectangle(String imageFile, double width, double length)
        {
            Material frontMaterial = MaterialMaker.MakeImageMaterial(imageFile);
            _pictureOffset = length / 2 + 5;
            _length = length;
            RectangleVisual3D charPic = new RectangleVisual3D()
            {
                Material = frontMaterial,
                BackMaterial = MaterialMaker.PaperbackMaterial(),
                Length = _length,
                Width = width,
                Origin = new Point3D(5, 8, _pictureOffset),
                Normal = new Vector3D(1, 1, 0),
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
                Height = 2.5,
                BaseRadius = length/3,
                TopRadius = length/3,
                Material = new DiffuseMaterial(new SolidColorBrush(PieceColor)),
                Origin = new Point3D(5, 8, 0),
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
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            AnimationClock clock2 = moveAnimationCone.CreateClock();
            CharImage.ApplyAnimationClock(RectangleVisual3D.OriginProperty, clock1);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.OriginProperty, clock2);
            //CharImage.Origin = point3D;
            //BaseCone.Origin = ;
        }



        internal void StopActiveAnimation()
        {
            if (activeAnimation != null)
            {
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, null);
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, null);
            }
        }

        DoubleAnimation activeAnimation = null;
        internal void StartActiveAnimation()
        {
            activeAnimation = new DoubleAnimation(BaseCone.TopRadius, BaseCone.TopRadius * 1.5, new Duration(new TimeSpan(0, 0, 0, 0, 800)));
            activeAnimation.AutoReverse = true;
            activeAnimation.RepeatBehavior = RepeatBehavior.Forever;
            AnimationClock _activeClock = activeAnimation.CreateClock();
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, _activeClock);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, _activeClock);
        }
    }
}
