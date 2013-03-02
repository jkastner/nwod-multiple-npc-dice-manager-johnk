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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameBoard
{
    /// <summary>
    /// Interaction logic for OldBoard.xaml
    /// </summary>
    public partial class OldBoard : UserControl
    {
        public OldBoard()
        {
            InitializeComponent();
        }

//        using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Media.Media3D;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using HelixToolkit.Wpf;

//namespace GameBoardVisual
//{
//    /// <summary>
//    /// Interaction logic for GameBoardVisual.xaml
//    /// </summary>
//    public partial class GameBoardVisual : Window
//    {
//        private Dictionary<ModelVisual3D, Point3D> _modelCenters = new Dictionary<ModelVisual3D, Point3D>();
//        Material _frontMaterial, _backMaterial;
//        public GameBoardVisual()
//        {
//            InitializeComponent();
//            var mb = new MeshBuilder();

            
//            List <Point3D> board = MakeBoard(1000);
            
            
//            mb.AddQuad(board[0], board[1], board[2], board[3]);

            

//            ImageBrush berriesBrush = new ImageBrush();
//            String boardImage = 
//@"C:\Users\Kastner\Documents\Visual Studio 2012\Projects\GameBoardVisual\GameBoardVisual\bin\Debug\MapFront.jpg";
//            String boardBackground =
//@"C:\Users\Kastner\Documents\Visual Studio 2012\Projects\GameBoardVisual\GameBoardVisual\bin\Debug\SquarePaper.jpg";
//            _frontMaterial = MakeImageMaterial(boardImage);
//            _backMaterial  = MakeImageMaterial(boardBackground);

//            Viewport.Children.Add(MeshToVisual3D(mb, _frontMaterial, _backMaterial, false));

//            MeshBuilder sphere = new MeshBuilder();
//            sphere.AddSphere(new Point3D(0, 0, 5), 2);
//            //Viewport.Children.Add(MeshToVisual3D(sphere, Materials.Red, Materials.Red, true));
//            String girlModel

//                = @"C:\Users\Kastner\Documents\Visual Studio 2012\Projects\GameBoardVisual\GameBoardVisual\bin\Debug\archer.obj";
            
//            Transform3DGroup myTransforms = new Transform3DGroup();
//            myTransforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90)
//                , new Point3D(0, 0, 0)));
//            //myTransforms.Children.Add(new ScaleTransform3D(8, 8, 8));

//            ModelVisual3D theGirl = ObjFile(girlModel, Materials.DarkGray, Materials.DarkGray, true);
//            theGirl.Transform = myTransforms;
//            //AddDirectionalManipulators(theGirl);
//            Viewport.Children.Add(theGirl);
            
            
//            _modelCenters.Add(theGirl, new Point3D(0, 0, 0));
//            String dragonLocation = @"C:\Users\Kastner\Documents\Visual Studio 2012\Projects\GameBoardVisual\GameBoardVisual\bin\Debug\dragon.obj";
//            ModelVisual3D dragonDone = ObjFile(dragonLocation, Materials.Red, Materials.Black, false);
//            _modelCenters.Add(dragonDone, new Point3D(550, 50, 0));
//            Transform3DGroup dragonTransform = new Transform3DGroup();
//            dragonTransform.Children.Add(new ScaleTransform3D(20,20,20));
//            dragonTransform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90)
//                , new Point3D(50, 0, 0)));
//            dragonTransform.Children.Add(new TranslateTransform3D(550,50,130));

//            dragonDone.Transform = dragonTransform;
//            Viewport.Children.Add(dragonDone);
            
            
//            Viewport.Background = new SolidColorBrush(Colors.Gray);
//            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
//            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));
//            Viewport.Camera.Position = new Point3D(10, 520, 1000);
//        }

//        private ModelVisual3D ObjFile(string modelPath, Material frontMaterial, Material backMaterial, bool useManipulator)
//        {
//            Model3DGroup group = HelixToolkit.Wpf.ModelImporter.Load(modelPath);
//            ModelVisual3D myModelVisual3D = new ModelVisual3D();
//            myModelVisual3D.Content = group;
//            if (useManipulator)
//            {
//                AddDirectionalManipulators(myModelVisual3D);
//            }
//            foreach (GeometryModel3D geometry in (myModelVisual3D.Content as Model3DGroup).Children)
//            {
//                geometry.Material = frontMaterial;
//                geometry.BackMaterial = backMaterial;
//            }

//            var modelContent = myModelVisual3D.Content as GeometryModel3D;
//            return myModelVisual3D;
//        }


       

//        private Material MakeImageMaterial(string imageLocation)
//        {
//            ImageBrush imageBrush = new ImageBrush();


//            imageBrush.ImageSource =
//                new BitmapImage(
//                    new Uri(imageLocation, UriKind.Relative)
//                );
//            Material image = new DiffuseMaterial(imageBrush);
//            return image;
//        }

//        private Visual3D MeshToVisual3D(MeshBuilder mb, Material theMaterial, 
//            Material backMaterial = null,  bool useManipulator = false)
//        {
//            ModelVisual3D myModelVisual3D = new ModelVisual3D();
//            GeometryModel3D theModel = new GeometryModel3D(mb.ToMesh(), theMaterial);
//            if (backMaterial == null)
//            {
//                backMaterial = theMaterial;
//            }
//            else
//            {
//                theModel.BackMaterial = backMaterial;
//            }
//            myModelVisual3D.Content = theModel;
//            if (useManipulator)
//            {
//                AddDirectionalManipulators(myModelVisual3D);

//            }
//            return myModelVisual3D;
//        }

//        private void AddDirectionalManipulators(ModelVisual3D myModelVisual3D)
//        {
//            //AddTranslator(myModelVisual3D, new Vector3D(0, 0, -1), Colors.Black);
//            //AddTranslator(myModelVisual3D, new Vector3D(0, 0, 1), Colors.Black);
            
//            AddTranslator(myModelVisual3D, new Vector3D(0, 1, 0), Colors.Red);
//            AddTranslator(myModelVisual3D, new Vector3D(0, -1, 0), Colors.Red);

//            //AddTranslator(myModelVisual3D, new Vector3D(1, 0, 0), Colors.Blue);
//            //AddTranslator(myModelVisual3D, new Vector3D(-1, 0, 0), Colors.Blue);

//            AddRotateTranslator(myModelVisual3D, Colors.LightSkyBlue);



//        }

//        private void AddRotateTranslator(ModelVisual3D myModelVisual3D, Color color)
//        {
//            RotateManipulator around = new RotateManipulator()
//            {
//                Length = 30,
//                Diameter = 40,
//                InnerDiameter = 10,
//                Color = color,

//                Axis=new Vector3D(0,1,0),
//            };
//            around.Bind(myModelVisual3D);
//            Viewport.Children.Add(around);
//        }

//        private void AddTranslator(ModelVisual3D myModelVisual3D, Vector3D vector3D, Color color)
//        {
//            TranslateManipulator forward = new TranslateManipulator()
//            {
//                Direction = vector3D,
//                Length = 30,
//                Diameter = 4,
//                Color = color,
//            };
//            forward.Bind(myModelVisual3D);
//            Viewport.Children.Add(forward);
//        }




//        private List<Point3D> MakeBoard(int p)
//        {
//            List<Point3D> board = new List<Point3D>();
//            board.Add(new Point3D(-p, 0, 0));
            
//            board.Add(new Point3D(0, -p, 0));
            
//            board.Add(new Point3D(p, 0, 0));
//            board.Add(new Point3D(0, p, 0));



//            return board;

//        }
//        ModelVisual3D _currentVisual = null;
//        private void ClickUp(object sender, MouseButtonEventArgs e)
//        {

//            Point ptMouse = e.GetPosition(Viewport);
//            var result = VisualTreeHelper.HitTest(Viewport, ptMouse) as RayMeshGeometry3DHitTestResult;
            
//            // We're only interested in 3D hits.
//            RayMeshGeometry3DHitTestResult result3d =
//                                result as RayMeshGeometry3DHitTestResult;
//            if (result3d == null)
//                return;

//            // We're only interested in ModelVisual3D hits.
//            var newVisual = (result3d.VisualHit as ModelVisual3D);
//            if (newVisual == null)
//                return;
//            var geometryModel = newVisual.Content as GeometryModel3D;
//            if (geometryModel != null)
//            {
//                if (_currentVisual == null)
//                    return;

//                var curPos = _modelCenters[_currentVisual];
//                var curTransform = _currentVisual.Transform as Transform3DGroup;
//                curTransform.Children.Add(new TranslateTransform3D(result.PointHit.X - curPos.X, result.PointHit.Y - curPos.Y, result.PointHit.Z - curPos.Z));
//                _modelCenters[_currentVisual] = result.PointHit;
//                _currentVisual = null;
//                return;

//            }
//            _currentVisual = newVisual;



//            e.Handled = true;
//        }


//    }
//}

    }
}
