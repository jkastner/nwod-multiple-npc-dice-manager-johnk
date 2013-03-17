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

namespace GameBoard
{
    public class VisualsViewmodel
    {
        public VisualsViewmodel()
        {
        }

        public void Initialize()
        {
            SetBoardBackground(@"MapPictures\BattleMap.jpg", 0, 0, true);
        }


        public MeshBuilder InitializeBoardBoundaries(double height, double width)
        {
            List<Point3D> board = new List<Point3D>();
            board.Add(new Point3D(-width / 2, -height / 2, 0));

            board.Add(new Point3D(width / 2, -height / 2, 0));

            board.Add(new Point3D(width/2, height/2, 0));
            board.Add(new Point3D(-width/2, height / 2, 0));


            var mb = new MeshBuilder();

            mb.AddQuad(board[0], board[1], board[2], board[3]);
            //mb.AddQuad(board[2], board[0], board[1], board[3]);
            return mb;

        }


        private HelixViewport3D _viewport;
        public HelixViewport3D Viewport
        {
            get { return _viewport; }
            set { _viewport = value; }
        }
        

        private Visual3D _theMap;
        public double BoardHeight { get; set; }
        public double BoardWidth { get; set; }

        public Visual3D TheMap
        {
            get { return _theMap; }
            set { _theMap = value; }
        }


        private Dictionary<Visual3D, MoveablePicture>  _visualToMoveablePicturesDictionary = new Dictionary<Visual3D,MoveablePicture>();
        public Dictionary<Visual3D, MoveablePicture> VisualToMoveablePicturesDictionary
        {
            get { return _visualToMoveablePicturesDictionary; }
            set { _visualToMoveablePicturesDictionary = value; }
        }

        Color defaultColor = Colors.Gray;
        public MoveablePicture AddImagePieceToMap(String charImageFile, Color pieceColor, String name, int height, Point3D location, List <StatusEffectDisplay> statusEffects)
        {
            double heightFeet = height / 12;
            MoveablePicture charImage = new MoveablePicture(charImageFile, heightFeet, name, pieceColor, location, statusEffects);
            VisualToMoveablePicturesDictionary.Add(charImage.CharImage, charImage);
            Viewport.Children.Add(charImage.CharImage);
            Viewport.Children.Add(charImage.BaseCone);
            return charImage;
        }

        Dictionary<AnimationClock, MeshElement3D> _attackLines = new Dictionary<AnimationClock, MeshElement3D>();
        public void DrawAttack(MoveablePicture attacker, MoveablePicture target, Color materialColor, Duration showDuration)
        {
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
            Viewport.Children.Add(tube);
            clock1.Completed += removeVisualTick;
            _attackLines.Add(clock1, tube);
        }

        private void removeVisualTick(object sender, EventArgs e)
        {
            RemoveIfPresent(_attackLines[sender as AnimationClock]);
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

        
        public void SelectCharacterFromVisual(RectangleVisual3D lastHit)
        {
            VisualToMoveablePicturesDictionary[lastHit].IsSelected = true;
            OnPieceSelected(new PieceSelectedEventArgs(VisualToMoveablePicturesDictionary[lastHit]));
        }

        public void SetActive(bool isActive, MoveablePicture moveablePicture, Color pieceColor, bool drawSingleSelectionDetails, double curSpeed, List<StatusEffectDisplay> statuses)
        {
            if (!isActive)
            {
                moveablePicture.IsSelected = false;
                RemoveIfPresent(moveablePicture.MovementCircle);
                RemoveIfPresent(moveablePicture.DoubleMovementCircle);
                RemoveIfPresent(moveablePicture.InfoText);
            }
            else
            {
                moveablePicture.Speed = curSpeed;
                moveablePicture.IsSelected = true;
                AddIfNew(moveablePicture.InfoText);
                if (drawSingleSelectionDetails)
                {
                    moveablePicture.RemakeInfoText(statuses);
                    AddIfNew(moveablePicture.MovementCircle);
                    AddIfNew(moveablePicture.DoubleMovementCircle);
                }
            }
        }

        private void RemoveIfPresent(MeshElement3D Visual3D)
        {
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

        public void RemovePiece(MoveablePicture moveablePicture)
        {
            foreach (var cur in moveablePicture.AssociatedVisuals)
            {
                RemoveIfPresent(cur);
            }
            _visualToMoveablePicturesDictionary.Remove(moveablePicture.CharImage);
            moveablePicture = null;
        }

        public void SetBoardBackground(string newImage, double definedBoardHeight, double definedBoardWidth, bool maintainRatio)
        {
            if(_theMap!=null)
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




        internal void MoveSelectedPiecesTo(Point3D point3D)
        {
            var selectedCharacters = VisualToMoveablePicturesDictionary.Where(x=>x.Value.IsSelected);
            if (selectedCharacters.Count() == 1)
            {
                var fc = selectedCharacters.First().Value;
                fc.MoveTo(new Point3D(point3D.X, point3D.Y, point3D.Z + fc.PictureOffset));
            }
            else
             {
                //Move the height of the picture away from the targeted location.
                var middleSelection = Helper3DCalcs.FindMidpoint(selectedCharacters.Select(x=>x.Value.Location));
                foreach (var cur in selectedCharacters)
                {
                    var curP = cur.Value.Location;
                    var vectorToOldMidpoint = middleSelection - curP;
                    vectorToOldMidpoint.Normalize();
                    double distance = Helper3DCalcs.DistanceBetween(curP, middleSelection);
                    vectorToOldMidpoint = vectorToOldMidpoint * distance;
                    var np = new Point3D(vectorToOldMidpoint.X + point3D.X, vectorToOldMidpoint.Y + point3D.Y, vectorToOldMidpoint.Z + point3D.Z);
                    cur.Value.MoveTo(new Point3D(np.X, np.Y, np.Z + cur.Value.PictureOffset));
                }
            }
        }

        internal void ClearSelectedCharacters()
        {
            foreach (var cur in VisualToMoveablePicturesDictionary.Where(x => x.Value.IsSelected))
            {
                cur.Value.IsSelected = false;
            }
            OnClearSelectedPieces(new ClearSelectedPiecesEventArgs());
        }

    }
}
