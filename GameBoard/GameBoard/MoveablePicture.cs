using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.Runtime.Serialization;

namespace GameBoard
{
    [DataContract(Namespace = "")]
    public class MoveablePicture : INotifyPropertyChanged
    {
        private RectangleVisual3D _charImage;
        public RectangleVisual3D CharImage
        {
            get { return _charImage; }
            set { _charImage = value; }
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
        [DataMember]
        public double PictureOffset
        {
            get { return _pictureOffset; }
            set { _pictureOffset = value; }
        }
        private String _name;
        [DataMember]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public double Speed { get; set; }
        [DataMember]
        public Color PieceColor { get; set; }

        [DataMember]
        public double LongestPictureSide { get; set; }

        public Point3D Location
        {
            get { return CharImage.Origin; }
            set
            {
                LocationForSave = value;
                MoveTo(value);
            }
        }
        [DataMember]
        public Point3D LocationForSave
        {get;set;}

        public double BaseConeRadius { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                _isSelected = value;
                if (_isSelected)
                    StartActive();
                else
                    StopActive();
                OnPropertyChanged("IsSelected");
            }
        }

        [DataMember]
        public String PictureFileName { get; set; }

        private List<StatusEffectDisplay> _statusEffects = new List<StatusEffectDisplay>();
        [DataMember]
        public List<StatusEffectDisplay> StatusEffects
        {
            get { return _statusEffects; }
            set { _statusEffects = value; }
        }
        public MoveablePicture(String pictureFile, double longestEdge, String imageName, Color pieceColor, Point3D location, List<StatusEffectDisplay> statuses, double speed)
        {
            PieceColor = pieceColor;
            Name = imageName;
            Speed = speed;
            LocationForSave = location;
            PictureFileName = pictureFile;
            _charImage = ImageToRectangle(pictureFile, longestEdge, location);
            AssociatedVisuals.Add(BaseCone);
            AssociatedVisuals.Add(MovementCircle);
            AssociatedVisuals.Add(DoubleMovementCircle);
            AssociatedVisuals.Add(CharImage);
            LongestPictureSide = longestEdge;
            RemakeInfoText(statuses);
            AssociatedVisuals.Add(InfoText);
            StatusEffects = statuses;

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            _charImage = ImageToRectangle(PictureFileName, LongestPictureSide, LocationForSave);
            _associatedVisuals = new List<Visual3D>();
            AssociatedVisuals.Add(BaseCone);
            AssociatedVisuals.Add(MovementCircle);
            AssociatedVisuals.Add(DoubleMovementCircle);
            AssociatedVisuals.Add(CharImage);
            RemakeInfoText(StatusEffects);
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
                        CharImage.Origin.Z + PictureOffset + LongestPictureSide + 3),
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


        private RectangleVisual3D ImageToRectangle(String imageFile,  double longestEdge, Point3D location)
        {
            ImageBrush frontBrush =  MaterialMaker.MakeImageMaterial(imageFile);
            Material frontMaterial = new DiffuseMaterial(frontBrush);
            PictureOffset = 3.5;
            double length, width, baseConeRadius;
            double origWidth= frontBrush.ImageSource.Width;
            double origLength = frontBrush.ImageSource.Height;
            //The base cone should be larger than the smallest side.
            if (origWidth > origLength)
            {
                width = longestEdge;
                length = longestEdge / (origWidth / origLength);
                baseConeRadius = length / 1.3; 
            }
            else
            {
                width = (origWidth / origLength) * longestEdge;
                length = longestEdge;
                baseConeRadius = width / 1.3;
            }

            RectangleVisual3D charPic = new RectangleVisual3D()
            {
                Material = frontMaterial,
                BackMaterial = MaterialMaker.PaperbackMaterial(),
                Length = length,
                Width = width,
                Origin = new Point3D(location.X, location.Y, PictureOffset),
                Normal = new Vector3D(0.00001, 0.00001, 1),
                LengthDirection = new Vector3D(0, -.5, .5),
            };

            BaseConeRadius = longestEdge / 3;
            BaseCone = new TruncatedConeVisual3D()
            {
                Height = PictureOffset-1,
                BaseRadius = BaseConeRadius,
                TopRadius = BaseConeRadius,
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


        internal void MoveTo(Point3D point3D)
        {
            LocationForSave = point3D;
            Point3DAnimation moveAnimationPic = new Point3DAnimation(CharImage.Origin, point3D, new Duration(new TimeSpan(0,0,0,1)));
            Point3DAnimation moveAnimationCone = new Point3DAnimation(BaseCone.Origin, new Point3D(point3D.X, point3D.Y, 0), new Duration(new TimeSpan(0, 0, 0, 1)));
            Point3DAnimation textAnimation = new Point3DAnimation(InfoText.Position, new Point3D(point3D.X, point3D.Y, InfoText.Position.Z), new Duration(new TimeSpan(0, 0, 0, 1)));
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            AnimationClock clock2 = moveAnimationCone.CreateClock();
            AnimationClock clock3 = textAnimation.CreateClock();
            CharImage.ApplyAnimationClock(RectangleVisual3D.OriginProperty, clock1);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.OriginProperty, clock2);
            InfoText.ApplyAnimationClock(BillboardTextVisual3D.PositionProperty, clock3);
            //CharImage.Origin = point1;
            //BaseCone.Origin = ;
        }



        internal void StopActive()
        {
            if (activeAnimation != null)
            {
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, null);
                BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, null);
            }
        }

        DoubleAnimation activeAnimation = null;
        internal void StartActive()
        {
            activeAnimation = new DoubleAnimation(BaseConeRadius, BaseConeRadius * 2.5, new Duration(new TimeSpan(0, 0, 0, 0, 800)));
            activeAnimation.AutoReverse = true;
            activeAnimation.RepeatBehavior = RepeatBehavior.Forever;
            AnimationClock _activeClock = activeAnimation.CreateClock();
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.TopRadiusProperty, _activeClock);
            BaseCone.ApplyAnimationClock(TruncatedConeVisual3D.BaseRadiusProperty, _activeClock);
            Point3DCollection circlePoints = new Point3DCollection(Helper3DCalcs.CirclePoints(Speed, CharImage.Origin));
            Point3DCollection doubleCirclePoints = new Point3DCollection(Helper3DCalcs.CirclePoints(Speed * 2, CharImage.Origin));
            MovementCircle.Path = circlePoints;
            DoubleMovementCircle.Path = doubleCirclePoints;
        }

        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

    }
}
