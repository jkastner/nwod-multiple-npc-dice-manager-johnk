using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;

namespace GameBoard
{
    /// <summary>
    /// Interaction logic for GameBoardVisual.xaml
    /// </summary>
    public partial class GameBoardVisual : Window
    {
        Material _frontMaterial, _backMaterial;
        Visual3D _theBackground;

        private Dictionary<Visual3D, MoveablePicture> _visuals = new Dictionary<Visual3D, MoveablePicture>();

        public GameBoardVisual()
        {
            InitializeComponent();
            var mb = new MeshBuilder();

            
            List <Point3D> board = MakeBoard(300);
            
            
            mb.AddQuad(board[0], board[1], board[2], board[3]);

            

            ImageBrush berriesBrush = new ImageBrush();
            String boardImage = @"MapPictures\BattleMap.jpg";

            _frontMaterial = MaterialMaker.MakeImageMaterial(boardImage);
            _backMaterial =  MaterialMaker.PaperbackMaterial();
            
            _theBackground = MeshToVisual3D(mb, _frontMaterial, _backMaterial);
            
            Viewport.Children.Add(_theBackground);

            AddToMap(@"PiecePictures\Velecia.jpg");

            //_visuals.Add(knight.CharImage, knight);
            //Viewport.Children.Add(knight.CharImage);


            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));
            
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 1, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));

            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, 0, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 0, 0)));

            Viewport.Background = new SolidColorBrush(Colors.SkyBlue);
            
            Viewport.Camera.Position = new Point3D(1, 52, 100);
        }

        public MoveablePicture AddToMap(String charImageFile)
        {
            MoveablePicture charImage = new MoveablePicture(charImageFile, 10, 10, Viewport);
            _visuals.Add(charImage.CharImage, charImage);
            Viewport.Children.Add(charImage.CharImage);
            Viewport.Children.Add(charImage.BaseCone);
            return charImage;
        }

        
        private ModelVisual3D ObjFile(string modelPath, Material frontMaterial, Material backMaterial)
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

        private List<Point3D> MakeBoard(int p)
        {
            List<Point3D> board = new List<Point3D>();
            board.Add(new Point3D(-p, 0, 0));
            
            board.Add(new Point3D(0, -p, 0));
            
            board.Add(new Point3D(p, 0, 0));
            board.Add(new Point3D(0, p, 0));

            return board;

        }



        public void ViewportMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point mouseposition = args.GetPosition(Viewport);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(Viewport, null, HTResult, pointparams);
        }

        RectangleVisual3D lastHit = null;
        public HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            //MessageBox.Show(rawresult.ToString());
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                if (rayMeshResult != null)
                {
                    GeometryModel3D hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
                    if (rayMeshResult.VisualHit is RectangleVisual3D)
                    {
                        if (lastHit == null)
                        {
                            lastHit = rayMeshResult.VisualHit as RectangleVisual3D;
                        }
                    }
                    else if (lastHit != null)
                    {
                        var movedHit = rayMeshResult.PointHit;
                        _visuals[lastHit].MoveTo(new Point3D(movedHit.X, movedHit.Y, movedHit.Z + _visuals[lastHit].PictureOffset));
                        lastHit = null;
                    }
                    else if (lastHit == null)
                    {

                        //Viewport.Children.Add(new SphereVisual3D()
                        //{
                        //    Material = Materials.Rainbow,
                        //    Center = new Point3D(rayMeshResult.PointHit.X, rayMeshResult.PointHit.Y, rayMeshResult.PointHit.Z),
                        //    Radius = 1,
                        //});
                    }
                }
            }

            return HitTestResultBehavior.Stop;
        }

        private void ViewportKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && lastHit != null)
            {
                Viewport.Children.Remove(lastHit);
                lastHit = null;
            }
        }

    }
}
