using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [DataContract]
    public class VisualsViewModel
    {
        
        public VisualsViewModel()
        {
            Initialize();
        }

        public void SetInitialBackground()
        {
            SetBoardBackground(@"MapPictures\BattleMap.jpg", 0, 0, true);
        }


        public MeshBuilder InitializeBoardBoundaries(double height, double width)
        {
            List<Point3D> board = new List<Point3D>();
            board.Add(new Point3D(-width / 2, -height / 2, 0));

            board.Add(new Point3D(width / 2, -height / 2, 0));

            board.Add(new Point3D(width / 2, height / 2, 0));
            board.Add(new Point3D(-width / 2, height / 2, 0));


            var mb = new MeshBuilder();

            mb.AddQuad(board[0], board[1], board[2], board[3]);
            //mb.AddQuad(board[2], board[0], board[1], board[3]);
            return mb;

        }


        private Visual3D _theMap;
        [DataMember]
        public double BoardHeight { get; set; }
        [DataMember]
        public double BoardWidth { get; set; }

        public Visual3D TheMap
        {
            get { return _theMap; }
            set { _theMap = value; }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc)
        {
            Initialize();
        }

        private void Initialize()
        {
            _temporaryVisuals = new Dictionary<AnimationClock, MeshElement3D>();
            _groupSingleMovementCircle = new TubeVisual3D()
            {
                Diameter = .5,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkBlue)),
                ThetaDiv = 15,
            };
            _groupDoubleMovementCircle = new TubeVisual3D()
            {
                Diameter = .5,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkBlue)),
                ThetaDiv = 15,
            };
        }

        Color defaultColor = Colors.Gray;
        public MoveablePicture AddImagePieceToMap(String charImageFile, Color pieceColor, String name, double height,
            Point3D location, List<StatusEffectDisplay> statusEffects, double speed, Guid characterGuid)
        {
            double heightFeet = height;
            while (CharactersToMoveablePicture.Values.Any(
                existingImage =>
                    existingImage.Location.X == location.X &&
                    existingImage.Location.Y == location.Y &&
                    existingImage.Location.Z == location.Z + existingImage.PictureOffset))
            {
                location = new Point3D(location.X + heightFeet, location.Y, location.Z);
            }
            MoveablePicture charImage = new MoveablePicture(charImageFile, heightFeet, name, pieceColor, location, statusEffects, speed);
            RegisterMoveablePicture(charImage, characterGuid);
            return charImage;
        }

        private void RegisterMoveablePicture(MoveablePicture charImage, Guid characterGuid)
        {
            if(_charactersToMoveablePicture.ContainsKey(characterGuid))
            {
                _charactersToMoveablePicture.Remove(characterGuid);
            }
            _charactersToMoveablePicture.Add(characterGuid, charImage);
            _viewport.Children.Add(charImage.CharImage);
            _viewport.Children.Add(charImage.BaseCone);
        }

        Dictionary<AnimationClock, MeshElement3D> _temporaryVisuals = new Dictionary<AnimationClock, MeshElement3D>();
        public void DrawAttack(Guid attackerID, Guid targetID, Color materialColor, Duration showDuration)
        {
            MoveablePicture attacker = _charactersToMoveablePicture[attackerID];
            MoveablePicture target = _charactersToMoveablePicture[targetID];

            DoubleAnimation moveAnimationPic = new DoubleAnimation(1, .1, showDuration);
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            Point3DCollection thePath = new Point3DCollection(new List<Point3D>() { attacker.CharImage.Origin, target.CharImage.Origin });
            ArrowVisual3D tube = new ArrowVisual3D()
            {
                Origin = attacker.CharImage.Origin,
                Point2 = target.CharImage.Origin,
                Material = new DiffuseMaterial(new SolidColorBrush(materialColor)),
                Diameter = 1,

            };
            tube.ApplyAnimationClock(ArrowVisual3D.DiameterProperty, clock1);
            _viewport.Children.Add(tube);
            clock1.Completed += removeVisualTick;
            _temporaryVisuals.Add(clock1, tube);
        }

        private void removeVisualTick(object sender, EventArgs e)
        {
            RemoveIfPresent(_temporaryVisuals[sender as AnimationClock]);
        }


        private Visual3D MeshToVisual3D(MeshBuilder mb, Material theMaterial,
            Material backMaterial = null)
        {
            ModelVisual3D myModelVisual3D = new ModelVisual3D();
            GeometryModel3D theModel = new GeometryModel3D(mb.ToMesh(), theMaterial);
            if (backMaterial == null)
            {
                backMaterial = theMaterial;
            }
            else
            {
                theModel.BackMaterial = backMaterial;
            }
            myModelVisual3D.Content = theModel;
            return myModelVisual3D;
        }




        private ModelVisual3D MakeModelVisual3DFromObjectFile(string modelPath, Material frontMaterial, Material backMaterial)
        {
            Model3DGroup group = HelixToolkit.Wpf.ModelImporter.Load(modelPath);
            ModelVisual3D myModelVisual3D = new ModelVisual3D();
            myModelVisual3D.Content = group;
            foreach (GeometryModel3D geometry in (myModelVisual3D.Content as Model3DGroup).Children)
            {
                geometry.Material = frontMaterial;
                geometry.BackMaterial = backMaterial;
            }

            var modelContent = myModelVisual3D.Content as GeometryModel3D;
            return myModelVisual3D;
        }

        public event EventHandler PieceSelected;
        protected virtual void OnPieceSelected(PieceSelectedEventArgs e)
        {
            EventHandler handler = PieceSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public event EventHandler ClearSelectedPieces;
        protected virtual void OnClearSelectedPieces(ClearSelectedPiecesEventArgs e)
        {
            EventHandler handler = ClearSelectedPieces;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler PieceMoved;
        protected virtual void OnPieceMoved(PieceMovedEventsArg e)
        {
            EventHandler handler = PieceMoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ToggleSelectCharacterFromVisual(RectangleVisual3D lastHit)
        {
            MoveablePicture match = null;
            foreach (var cur in CharactersToMoveablePicture)
            {
                if (cur.Value.CharImage.Equals(lastHit))
                {
                    match = cur.Value;
                }
            }
            if (match.IsSelected)
            {
                List<MoveablePicture> previousSelections = CharactersToMoveablePicture.Where(x => x.Value.IsSelected).Select(z => z.Value).ToList();
                ClearSelectedCharacters();
                previousSelections.Remove(match);
                foreach (var cur in previousSelections)
                {
                    cur.IsSelected = true;
                    OnPieceSelected(new PieceSelectedEventArgs(IDFromPicture(cur)));
                }
            }
            else
            {
                match.IsSelected = true;
                OnPieceSelected(new PieceSelectedEventArgs(IDFromPicture(match)));
            }
        }

        private Guid IDFromPicture(MoveablePicture moveablePicture)
        {
            foreach (var cur in _charactersToMoveablePicture)
            {
                if (cur.Value == moveablePicture)
                {
                    return cur.Key;
                }
            }
            throw new Exception("Unregistered picture ID");
        }

        public void SetActive(Guid targetId, Color pieceColor, bool drawSingleSelectionDetails, double curSpeed, List<StatusEffectDisplay> statuses)
        {
            MoveablePicture moveablePicture = _charactersToMoveablePicture[targetId];
            moveablePicture.Speed = curSpeed;
            moveablePicture.IsSelected = true;
            moveablePicture.RemakeInfoText(statuses);
            AddIfNew(moveablePicture.InfoText);
            if (drawSingleSelectionDetails)
            {
                AddIfNew(moveablePicture.MovementCircle);
                AddIfNew(moveablePicture.DoubleMovementCircle);
            }
        }

        public void SetInactive(Guid targetID)
        {
            MoveablePicture moveablePicture = _charactersToMoveablePicture[targetID];
            RemoveIfPresent(_groupSingleMovementCircle);
            RemoveIfPresent(_groupDoubleMovementCircle);
            moveablePicture.IsSelected = false;
            RemoveIfPresent(moveablePicture.MovementCircle);
            RemoveIfPresent(moveablePicture.DoubleMovementCircle);
            RemoveIfPresent(moveablePicture.InfoText);
        }


        private void RemoveIfPresent(MeshElement3D Visual3D)
        {
            if (Visual3D == null)
                return;
            if (_viewport.Children.Contains(Visual3D))
            {
                _viewport.Children.Remove(Visual3D);
            }
        }
        private void RemoveIfPresent(Visual3D targetVisual)
        {
            if (_viewport.Children.Contains(targetVisual))
            {
                _viewport.Children.Remove(targetVisual);
            }
        }

        private void AddIfNew(MeshElement3D Visual3D)
        {
            if (!_viewport.Children.Contains(Visual3D))
            {
                _viewport.Children.Add(Visual3D);
            }
        }
        private void AddIfNew(Visual3D newVisual)
        {
            if (!_viewport.Children.Contains(newVisual))
            {
                _viewport.Children.Add(newVisual);
            }
        }

        public void RemovePiece(Guid uniqueID)
        {
            MoveablePicture match = _charactersToMoveablePicture[uniqueID];
            foreach (var cur in match.AssociatedVisuals)
            {
                RemoveIfPresent(cur);
            }
            CharactersToMoveablePicture.Remove(uniqueID);
            match = null;
        }

        private BoardInfo _currentBoardInfo;
        [DataMember]
        public BoardInfo CurrentBoardInfo
        {
            get { return _currentBoardInfo; }
            set { _currentBoardInfo = value; }
        }
        

        public void SetBoardBackground(string newImage, double definedBoardHeight, double definedBoardWidth, bool maintainRatio)
        {
            CurrentBoardInfo = new BoardInfo(newImage, definedBoardHeight, definedBoardWidth, maintainRatio);
            
            if (_theMap != null)
                RemoveIfPresent(_theMap);
            ImageBrush boardFrontBrush = new ImageBrush();
            ImageBrush frontBrush = MaterialMaker.MakeImageMaterial(newImage);
            Material frontMaterial = new DiffuseMaterial(frontBrush);
            if (maintainRatio)
            {
                if (definedBoardHeight <= 0)
                {
                    BoardHeight = frontBrush.ImageSource.Height;
                    BoardWidth = frontBrush.ImageSource.Width;
                }
                else
                {
                    BoardHeight = definedBoardHeight;
                    double normalHeight = frontBrush.ImageSource.Height;
                    double normalWidth = frontBrush.ImageSource.Width;
                    double ratio = normalHeight / normalWidth;
                    BoardWidth = (definedBoardHeight / ratio);
                }
            }
            else
            {
                BoardHeight = definedBoardHeight;
                BoardWidth = definedBoardWidth;

            }
            Material backMaterial = MaterialMaker.PaperbackMaterial();
            var mb = InitializeBoardBoundaries(BoardHeight, BoardWidth);
            var backward = mb.TextureCoordinates.Reverse().ToList();
            for (int curIndex = 0; curIndex < mb.TextureCoordinates.Count; curIndex++)
            {
                mb.TextureCoordinates[curIndex] = backward[curIndex];
            }
            _theMap = MeshToVisual3D(mb, frontMaterial, backMaterial);
            AddIfNew(_theMap);
        }

        public void MovePieceToPoint(Guid selectedID, Point3D targetPoint)
        {
            var matchingVisual = _charactersToMoveablePicture[selectedID];
            matchingVisual.MoveTo(new Point3D(targetPoint.X, targetPoint.Y, targetPoint.Z));
            OnPieceMoved(new PieceMovedEventsArg(selectedID, targetPoint));
        }

        internal void MoveSelectedPiecesTo(Point3D point3D)
        {
            var selectedCharacters = CharactersToMoveablePicture.Where(x => x.Value.IsSelected);
            if (selectedCharacters.Count() == 1)
            {
                var fc = selectedCharacters.First().Value;
                Point3D targetPoint = new Point3D(point3D.X, point3D.Y, point3D.Z + fc.PictureOffset);
                fc.MoveTo(targetPoint);
                OnPieceMoved(new PieceMovedEventsArg(IDFromPicture(fc), targetPoint));
            }
            else
            {
                //Move in formation to new location with respect to common centerpoint.
                var middleSelection = Helper3DCalcs.FindMidpoint(selectedCharacters.Select(x => x.Value.Location));
                foreach (var cur in selectedCharacters)
                {
                    var curP = cur.Value.Location;
                    var vectorToOldMidpoint = curP - middleSelection;
                    vectorToOldMidpoint.Normalize();
                    if (double.IsNaN(vectorToOldMidpoint.X) || double.IsNaN(vectorToOldMidpoint.Y) || double.IsNaN(vectorToOldMidpoint.Z))
                    {
                        cur.Value.MoveTo(new Point3D(point3D.X, point3D.Y, point3D.Z + cur.Value.PictureOffset));
                        continue;
                    }
                    double distance = Helper3DCalcs.DistanceBetween(curP, middleSelection);
                    vectorToOldMidpoint = vectorToOldMidpoint * distance;
                    var np = new Point3D(vectorToOldMidpoint.X + point3D.X, vectorToOldMidpoint.Y + point3D.Y, vectorToOldMidpoint.Z + point3D.Z);
                    Point3D targetPoint = new Point3D(np.X, np.Y, np.Z + cur.Value.PictureOffset);
                    cur.Value.MoveTo(targetPoint);
                    OnPieceMoved(new PieceMovedEventsArg(IDFromPicture(cur.Value), targetPoint));
                }
            }

        }

        internal void ClearSelectedCharacters()
        {
            foreach (var cur in CharactersToMoveablePicture.Where(x => x.Value.IsSelected))
            {
                cur.Value.IsSelected = false;
            }
            OnClearSelectedPieces(new ClearSelectedPiecesEventArgs());
        }

        private TubeVisual3D _groupSingleMovementCircle;
        public TubeVisual3D GroupSingleMovementCircle
        {
            get { return _groupSingleMovementCircle; }
            set { _groupSingleMovementCircle = value; }
        }

        private TubeVisual3D _groupDoubleMovementCircle;
        public TubeVisual3D GroupDoubleMovementCircle
        {
            get { return _groupDoubleMovementCircle; }
            set { _groupDoubleMovementCircle = value; }
        }

        public void DrawGroupMovementCircle()
        {
            List<Point3D> selectedPictures = CharactersToMoveablePicture.Where(x => x.Value.IsSelected).Select(z => z.Value.Location).ToList();
            double minSpeed = CharactersToMoveablePicture.Min(x => x.Value.Speed);
            Point3D midPoint = Helper3DCalcs.FindMidpoint(selectedPictures);
            var sMove = Helper3DCalcs.CirclePoints(minSpeed, midPoint);
            var dMove = Helper3DCalcs.CirclePoints(minSpeed * 2, midPoint);
            _groupSingleMovementCircle.Path = new Point3DCollection(sMove);
            _groupDoubleMovementCircle.Path = new Point3DCollection(dMove);
            AddIfNew(_groupSingleMovementCircle);
            AddIfNew(_groupDoubleMovementCircle);
        }

        private double _shapeSize = 20;
        public double ShapeSize
        {
            get { return _shapeSize; }
            set { _shapeSize = value; }
        }

        private bool _selectInsideShape = true;

        public bool SelectInsideShape
        {
            get { return _selectInsideShape; }
            set { _selectInsideShape = value; }
        }

        public enum ShapeMode
        {
            None, Sphere, Cone, Line
        }
        private ShapeMode _shapeSelection;
        public ShapeMode ShapeSelection
        {
            get { return _shapeSelection; }
            set { _shapeSelection = value; }
        }

        public void SetShapeMode(ShapeMode newMode)
        {
            ShapeSelection = newMode;
        }

        internal void DrawSphere(Point3D point3D)
        {
            var shape = new SphereVisual3D()
            {
                Center = point3D,
                Radius = ShapeSize,
                Material = new DiffuseMaterial(new SolidColorBrush(SelectedTeamColor())),
            };
            DoubleAnimation sizeChange = new DoubleAnimation(ShapeSize, ShapeSize, new Duration(new TimeSpan(0, 0, 0, 3)));
            AnimationClock animationClock = sizeChange.CreateClock();
            animationClock.Completed += removeVisualTick;
            shape.ApplyAnimationClock(SphereVisual3D.RadiusProperty, animationClock);
            _viewport.Children.Add(shape);
            _temporaryVisuals.Add(animationClock, shape);
            if (SelectInsideShape)
                SelectFromInsideShape(point3D, point3D);
        }

        private void SelectFromInsideShape(Point3D point1, Point3D point2)
        {
            List<MoveablePicture> selectedByShape = new List<MoveablePicture>();
            ClearSelectedCharacters();
            foreach (var cur in CharactersToMoveablePicture)
            {
                switch (ShapeSelection)
                {
                    case ShapeMode.Sphere:
                        double dist = Helper3DCalcs.DistanceBetween(cur.Value.Location, point1);
                        if (dist <= ShapeSize)
                        {
                            selectedByShape.Add(cur.Value);
                        }
                        break;
                    case ShapeMode.Cone:
                        //public static Boolean IsPointInCone(Point3D testPoint, Point3D coneApex, Point3D coneBaseCenter,
                        //            double aperture)
                        //
                        //     aperture angle
                        //       | \
                        //    H  |  \
                        //       |___\
                        //         R
                        var coneBaseCenter = Helper3DCalcs.MovePointTowards(point1, point2, ShapeSize);
                        var coneHeight = Helper3DCalcs.DistanceBetween(point1, coneBaseCenter);
                        var angle = Math.Atan(1);
                        if (Helper3DCalcs.IsPointInCone(cur.Value.Location, point1, coneBaseCenter, angle * 2))
                        {
                            selectedByShape.Add(cur.Value);
                        }
                        break;
                    case ShapeMode.Line:
                        Vector3D testpt = cur.Value.Location.ToVector3D();
                        Vector3D pt1 = point1.ToVector3D();
                        Vector3D pt2 = point2.ToVector3D();
                        double lengthsq = Math.Pow(Helper3DCalcs.DistanceBetween(point1, point2), 2);
                        if (Helper3DCalcs.PointIsInsideCylinder(testpt, pt1, pt2, LineDiameter, lengthsq))
                        {
                            selectedByShape.Add(cur.Value);
                        }
                        break;
                }

            }
            bool multiSelect = CharactersToMoveablePicture.Count(x => x.Value.IsSelected) > 1;
            foreach (var cur in selectedByShape)
            {
                //public void SetActive(MoveablePicture moveablePicture, Color pieceColor, bool drawSingleSelectionDetails, double curSpeed, List<StatusEffectDisplay> statuses)
                SetActive(IDFromPicture(cur), cur.PieceColor, !multiSelect, cur.Speed, cur.StatusEffects);
                OnPieceSelected(new PieceSelectedEventArgs(IDFromPicture(cur)));
            }

            if (multiSelect)
            {
                DrawGroupMovementCircle();
            }

        }


        internal void DrawCone(Point3D point1, Point3D point2)
        {
            Vector3D dir = point2 - point1;
            var shape = new TruncatedConeVisual3D()
            {
                Material = new DiffuseMaterial(new SolidColorBrush(SelectedTeamColor())),
                BaseRadius = 2,
                Height = ShapeSize,
                TopRadius = ShapeSize,
                Origin = point1,
                Normal = dir,
            };
            DoubleAnimation sizeChange = new DoubleAnimation(ShapeSize, ShapeSize, new Duration(new TimeSpan(0, 0, 0, 3)));
            AnimationClock animationClock = sizeChange.CreateClock();
            animationClock.Completed += removeVisualTick;
            shape.ApplyAnimationClock(SphereVisual3D.RadiusProperty, animationClock);
            _viewport.Children.Add(shape);
            _temporaryVisuals.Add(animationClock, shape);
            if (SelectInsideShape)
                SelectFromInsideShape(point1, point2);
        }

        const int LineDiameter = 5;
        internal void DrawLine(Point3D point1, Point3D point2)
        {
            var secondPoint = Helper3DCalcs.MovePointTowards(point1, point2, ShapeSize);
            var pathinfo = new Point3DCollection(new List<Point3D>() { point1, secondPoint });
            var shape = new TubeVisual3D()
            {
                Material = new DiffuseMaterial(new SolidColorBrush(SelectedTeamColor())),
                Path = pathinfo,
                Diameter = LineDiameter,
            };
            DoubleAnimation sizeChange = new DoubleAnimation(ShapeSize, ShapeSize, new Duration(new TimeSpan(0, 0, 0, 3)));
            AnimationClock animationClock = sizeChange.CreateClock();
            animationClock.Completed += removeVisualTick;
            shape.ApplyAnimationClock(SphereVisual3D.RadiusProperty, animationClock);
            _viewport.Children.Add(shape);
            _temporaryVisuals.Add(animationClock, shape);
            if (SelectInsideShape)
                SelectFromInsideShape(point1, point2);
        }

        public Color SelectedTeamColor()
        {
            var selectedCharacter = CharactersToMoveablePicture.FirstOrDefault(cur => cur.Value.IsSelected);
            if (selectedCharacter.Value != null)
                return selectedCharacter.Value.PieceColor;
            return Colors.Red;
        }

        public void ZoomTo(Point3D targetPoint)
        {
            _viewport.ZoomExtents(new Rect3D(targetPoint, new Size3D(0, 0, BoardWidth / 30)), 500);
        }

        GridLinesVisual3D _visualGrid = null;
        public GridLinesVisual3D VisualGrid
        {
            get
            {
                return _visualGrid;
            }
            set
            {
                _visualGrid = value;
            }
        }
        public void DrawGrid(int squareSizes)
        {
            _visualGrid = new GridLinesVisual3D()
            {
                Length = BoardWidth,
                Width = BoardHeight,
                Thickness = .3,
                MajorDistance = squareSizes,
                MinorDistance = squareSizes,
                Center = new Point3D(0, 0, 1),
                Material = Materials.Black,
            };
            _viewport.Children.Add(_visualGrid);
        }

        public void RemoveGrid()
        {
            if (_visualGrid != null)
            {
                _viewport.Children.Remove(_visualGrid);
                _visualGrid = null;
            }
        }


        private bool _tapeMeasurerActive = false;

        public bool TapeMeasurerActive
        {
            get { return _tapeMeasurerActive; }
            set
            {
                _tapeMeasurerActive = value;
                if (!value)
                {
                    CleanupTapeMeasurerVisuals();
                }
            }
        }


        private SphereVisual3D _startPoint;
        private SphereVisual3D _endPoint;
        private BillboardTextVisual3D _textVisual;
        private TubeVisual3D _distanceIndicator;
        internal void HandleTapeMeasurerHit(RayMeshGeometry3DHitTestResult rayMeshResult)
        {

            if (_startPoint != null && _endPoint != null)
            {
                CleanupTapeMeasurerVisuals();

            }
            if (_startPoint == null)
            {
                _startPoint = new SphereVisual3D()
                {
                    Center = rayMeshResult.PointHit,
                    Material = Materials.Black,
                    Radius = 2,
                };
                _viewport.Children.Add(_startPoint);
            }
            else if (_endPoint == null)
            {
                _endPoint = new SphereVisual3D()
                {
                    Center = rayMeshResult.PointHit,
                    Material = Materials.Black,
                    Radius = 2,
                };
                _viewport.Children.Add(_endPoint);
                double distance = (_startPoint.Center - _endPoint.Center).Length;
                distance = Math.Round(distance, 0);
                var textPos = _endPoint.Center;
                textPos.Z += 30;
                _textVisual = new BillboardTextVisual3D()
                {
                    Text = distance + " units.",
                    Position = textPos,
                    Foreground= new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Colors.Black),
                    FontSize = 28,
                };
                _distanceIndicator = new TubeVisual3D()
                {
                    Path = new Point3DCollection(new List<Point3D>() { _startPoint.Center, _endPoint.Center }),
                    Material = Materials.LightGray,
                };
                _viewport.Children.Add(_textVisual);
                _viewport.Children.Add(_distanceIndicator);
            }
        }

        private void CleanupTapeMeasurerVisuals()
        {
            if (_startPoint != null)
                _viewport.Children.Remove(_startPoint);
            if (_endPoint != null)
                _viewport.Children.Remove(_endPoint);
            if (_textVisual != null)
                _viewport.Children.Remove(_textVisual);
            if (_distanceIndicator != null)
                _viewport.Children.Remove(_distanceIndicator);

            _startPoint = null;
            _endPoint = null;
            _textVisual = null;
            _distanceIndicator = null;
        }


        public void ClearAll()
        {
            _viewport.Children.Clear();

        }

        public void OpenVisuals()
        {
            var keys = CharactersToMoveablePicture.Keys.ToList();
            var values = CharactersToMoveablePicture.Values.ToList();
            CharactersToMoveablePicture.Clear();
            for (int curIndex = 0; curIndex < keys.Count; curIndex++)
            {
                RegisterMoveablePicture(values[curIndex], keys[curIndex]);
            }
        }

        public void OpenBoardInfo(BoardInfo savedinfo)
        {
            SetBoardBackground(savedinfo.BoardImageName, savedinfo.DefinedBoardHeight, savedinfo.DefinedBoardWidth, savedinfo.MaintainRatio);
        }

        private HelixViewport3D _viewport;
        internal void SetViewport(HelixViewport3D helixViewport3D)
        {
            _viewport = helixViewport3D;
        }

        [DataMember]
        private Dictionary<Guid, MoveablePicture> _charactersToMoveablePicture = new Dictionary<Guid, MoveablePicture>();
        public Dictionary<Guid, MoveablePicture> CharactersToMoveablePicture
        {
            get { return _charactersToMoveablePicture; }
            set { _charactersToMoveablePicture = value; }
        }

        internal bool HasAssociatedVisual(Guid characterGuid)
        {
            return _charactersToMoveablePicture.ContainsKey(characterGuid);
        }

        public MoveablePicture MatchingVisual(Guid characterGuid)
        {
            return _charactersToMoveablePicture[characterGuid];
        }

        public void RemakeInfoTextFor(Guid guid, List<StatusEffectDisplay> statuses)
        {
            _charactersToMoveablePicture[guid].RemakeInfoText(statuses);
        }

        public void ZoomTo(List<Guid> visualIDs)
        {
            List<MoveablePicture> matchingPictures = new List<MoveablePicture>();
            foreach (var curID in visualIDs)
            {
                matchingPictures.Add(_charactersToMoveablePicture[curID]);
            }
            var midpoint = Helper3DCalcs.FindMidpoint(matchingPictures.Select(x => x.Location));
            ZoomTo(midpoint);
        }

        internal void OtherBoardPieceMoved(object sender, EventArgs e)
        {
            var movedEvent = e as PieceMovedEventsArg;
            var matchingVisual = _charactersToMoveablePicture[movedEvent.MoverID];
            matchingVisual.MoveTo(new Point3D(movedEvent.Destination.X, movedEvent.Destination.Y, movedEvent.Destination.Z));
        }


    }
}
