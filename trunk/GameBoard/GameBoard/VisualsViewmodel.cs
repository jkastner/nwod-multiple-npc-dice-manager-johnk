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
    public class VisualsViewmodel
    {
        public VisualsViewmodel()
        {
        }

        public void Initialize()
        {
            ImageBrush boardFrontBrush = new ImageBrush();
            String boardImage = @"MapPictures\BattleMap.jpg";

            Material frontMaterial = MaterialMaker.MakeImageMaterial(boardImage);
            Material backMaterial = MaterialMaker.PaperbackMaterial();
            var mb = InitializeBoardBoundaries(300);
            _theMap = MeshToVisual3D(mb, frontMaterial, backMaterial);
        }
        
        
        public MeshBuilder InitializeBoardBoundaries(double scale)
        {
            List<Point3D> board = new List<Point3D>();
            board.Add(new Point3D(-scale, 0, 0));

            board.Add(new Point3D(0, -scale, 0));

            board.Add(new Point3D(scale, 0, 0));
            board.Add(new Point3D(0, scale, 0));


            var mb = new MeshBuilder();

            mb.AddQuad(board[0], board[1], board[2], board[3]);

            return mb;

        }


        private HelixViewport3D _viewport;
        public HelixViewport3D Viewport
        {
            get { return _viewport; }
            set { _viewport = value; }
        }
        

        private Visual3D _theMap;

        public Visual3D TheMap
        {
            get { return _theMap; }
            set { _theMap = value; }
        }

        private RectangleVisual3D _lastHit = null;
        public RectangleVisual3D LastHit
        {
            get { return _lastHit; }
            set { _lastHit = value; }
        }


        private Dictionary<Visual3D, MoveablePicture>  _visualToMoveablePicturesDictionary = new Dictionary<Visual3D,MoveablePicture>();
        public Dictionary<Visual3D, MoveablePicture> VisualToMoveablePicturesDictionary
        {
            get { return _visualToMoveablePicturesDictionary; }
            set { _visualToMoveablePicturesDictionary = value; }
        }
        


        Color defaultColor = Colors.Gray;
        public MoveablePicture AddImagePieceToMap(String charImageFile, Color pieceColor, String name, int speed, int height)
        {
            double heightFeet = height / 12;
            MoveablePicture charImage = new MoveablePicture(charImageFile, heightFeet / 1.618, heightFeet, name, pieceColor, speed);
            _visualToMoveablePicturesDictionary.Add(charImage.CharImage, charImage);
            Viewport.Children.Add(charImage.CharImage);
            Viewport.Children.Add(charImage.BaseCone);
            return charImage;
        }

        Dictionary<AnimationClock, MeshElement3D> _attackLines = new Dictionary<AnimationClock, MeshElement3D>();
        public void DrawAttack(MoveablePicture attacker, MoveablePicture target)
        {
            DoubleAnimation moveAnimationPic = new DoubleAnimation(1, .1, new Duration(new TimeSpan(0, 0, 0, 3)));
            AnimationClock clock1 = moveAnimationPic.CreateClock();
            Point3DCollection thePath = new Point3DCollection(new List<Point3D>() { attacker.CharImage.Origin, target.CharImage.Origin });
            TubeVisual3D tube = new TubeVisual3D()
            {
                Path = thePath,
                Material = Materials.Red,
                Diameter = 1,

            };
            tube.ApplyAnimationClock(TubeVisual3D.DiameterProperty, clock1);
            Viewport.Children.Add(tube);
            clock1.Completed += removeTube;
            _attackLines.Add(clock1, tube);
        }

        private void removeTube(object sender, EventArgs e)
        {
            Viewport.Children.Remove(_attackLines[sender as AnimationClock]);
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

        private MoveablePicture _selectedPiece;

        public MoveablePicture SelectedPiece
        {
            get { return _selectedPiece; }
            set 
            { 
                _selectedPiece = value;
                OnPieceSelected(new PieceSelectedEventArgs(_selectedPiece));
            }
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

        
        
        public void SelectCharacterFromVisual(RectangleVisual3D lastHit)
        {
            SelectedPiece = VisualToMoveablePicturesDictionary[lastHit];
        }

        public void SetActive(bool isActive, MoveablePicture moveablePicture, Color pieceColor, bool drawMovementCircle)
        {
            if (!isActive)
            {
                moveablePicture.StopActive();
                RemoveIfPresent(moveablePicture.MovementCircle);
                RemoveIfPresent(moveablePicture.DoubleMovementCircle);
            }
            else
            {
                moveablePicture.StartActive();
                if (drawMovementCircle)
                {
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
        private void AddIfNew(MeshElement3D Visual3D)
        {
            if (!_viewport.Children.Contains(Visual3D))
            {
                _viewport.Children.Add(Visual3D);
            }
        }

        public void RemovePiece(MoveablePicture moveablePicture)
        {
            _viewport.Children.Remove(moveablePicture.CharImage);
            _viewport.Children.Remove(moveablePicture.BaseCone);
            _visualToMoveablePicturesDictionary.Remove(moveablePicture.CharImage);
            moveablePicture.CharImage = null;
            moveablePicture.BaseCone = null;
            moveablePicture = null;
        }
    }
}
