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
        private HelixViewport3D _viewport;
        public MoveablePicture(String pictureFile, double width, double length, HelixViewport3D viewport)
        {
            _charImage = ImageToRectangle(pictureFile, width, length);
            _viewport = viewport;
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
            //    BackMaterial = _backMaterial,
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
                Material = Materials.DarkGray,
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

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                _baseCone.Material = Materials.Gold;
            }
            else
            {
                _baseCone.Material = Materials.Gray;
            }
        }

        TubeVisual3D tube = new TubeVisual3D();
        public void DrawAttack(MoveablePicture target)
        {
            DoubleAnimation moveAnimationPic = new DoubleAnimation(1, .1, new Duration(new TimeSpan(0, 0, 0, 3)));
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            Point3DCollection thePath = new Point3DCollection(new List<Point3D>() { CharImage.Origin, target.CharImage.Origin });
            tube = new TubeVisual3D()
                {
                    Path = thePath,
                    Material = Materials.Red,
                    Diameter = 1,

                };
            tube.ApplyAnimationClock(TubeVisual3D.DiameterProperty, clock1);
            _viewport.Children.Add(tube);
            clock1.Completed += removeTube;

        }

        private void removeTube(object sender, EventArgs e)
        {
            _viewport.Children.Remove(tube);
        }
    }
}
