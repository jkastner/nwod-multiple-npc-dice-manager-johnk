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


        public VisualsViewmodel _viewModel;

        public GameBoardVisual(VisualsViewmodel ViewModel)
        {
            InitializeComponent();
            _viewModel = ViewModel;
            _viewModel.Viewport = Viewport;
            _viewModel.Initialize();

            //_visuals.Add(knight.CharImage, knight);
            //_viewport.Children.Add(knight.CharImage);


            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0,0,1)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));
            
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 1, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));

            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, 0, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 0, 0)));

            Viewport.Background = new SolidColorBrush(Colors.SkyBlue);
            Viewport.Camera.Position = new Point3D(1, 52, 5);
            Viewport.Camera.LookDirection = new Vector3D(.01, .01, -1);
        }


        bool multiSelect = false;
        public void ViewportMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point mouseposition = args.GetPosition(Viewport);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                multiSelect = true;
            }
            else
            {
                multiSelect = false;
            }
            //test for a result in the Viewport3D
            VisualTreeHelper.HitTest(Viewport, null, HTResult, pointparams);
        }

        public HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            //MessageBox.Show(rawresult.ToString());
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
                if (rayMeshResult != null)
                {
                    return HandleHit(rayMeshResult);
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private HitTestResultBehavior HandleHit(RayMeshGeometry3DHitTestResult rayMeshResult)
        {
            GeometryModel3D hitgeo = rayMeshResult.ModelHit as GeometryModel3D;
            if (rayMeshResult.VisualHit.Equals(_viewModel.GroupDoubleMovementCircle) ||
                rayMeshResult.VisualHit.Equals(_viewModel.GroupSingleMovementCircle))
            {
                return HitTestResultBehavior.Continue;
            }
            foreach (var curVisual in _viewModel.VisualToMoveablePicturesDictionary)
            {
                var curMoveable = curVisual.Value;
                IEnumerable<Visual3D> targets;
                //If they double-click to move, don't allow movement based on its own image to cause the image to float.
                if (doubleClick)
                {
                    targets = curMoveable.AssociatedVisuals;
                }
                else
                {
                    targets = curMoveable.AssociatedVisuals.Where(x => !x.Equals(curMoveable.CharImage));
                }
                foreach (var curAssociatedVisual in targets)
                {

                    if (rayMeshResult.VisualHit.Equals(curAssociatedVisual))
                    {
                        return HitTestResultBehavior.Continue;
                    }
                }
            }
            HandleValidHit(rayMeshResult);
            return HitTestResultBehavior.Stop;
        }

        private void HandleValidHit(RayMeshGeometry3DHitTestResult rayMeshResult)
        {
            if (_viewModel.ShapeSelection == GameBoard.VisualsViewmodel.ShapeMode.None)
            {
                if (!doubleClick && rayMeshResult.VisualHit is RectangleVisual3D)
                {
                    if (!multiSelect)
                        _viewModel.ClearSelectedCharacters();
                    _viewModel.ToggleSelectCharacterFromVisual(rayMeshResult.VisualHit as RectangleVisual3D);
                }
                else if (doubleClick)
                {
                    if (_viewModel.VisualToMoveablePicturesDictionary.Any(x => x.Value.IsSelected))
                    {
                        var movedHit = rayMeshResult.PointHit;
                        _viewModel.MoveSelectedPiecesTo(movedHit);
                    }
                }
            }
            else
            {
                DrawShape(rayMeshResult.PointHit);
            }
        }

        //Used because Point3D is non-nullable.
        List<Point3D> startPoint = new List<Point3D>();
        private void DrawShape(Point3D point3D)
        {
            switch (_viewModel.ShapeSelection)
            {
                case GameBoard.VisualsViewmodel.ShapeMode.Sphere:
                        {
                            _viewModel.DrawSphere(point3D);
                            break;
                        }
                case GameBoard.VisualsViewmodel.ShapeMode.Cone:
                    {
                        if (startPoint.Count == 0)
                        {
                            startPoint.Add(point3D);
                        }
                        else
                        {
                            _viewModel.DrawCone(startPoint.First(), point3D);
                            startPoint.Clear();
                        }
                        break;
                    }
                case GameBoard.VisualsViewmodel.ShapeMode.Line:
                    {
                        if (startPoint.Count == 0)
                        {
                            startPoint.Add(point3D);
                        }
                        else
                        {
                            _viewModel.DrawLine(startPoint.First(), point3D);
                            startPoint.Clear();
                        }
                        break;
                    }
                default:
                    throw new Exception("Unknown shape");
                    break;
            }
        }

        bool doubleClick = false;
        private void Viewport_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            doubleClick = true;
            ViewportMouseDown(sender, e);
            doubleClick = false;
        }


    }
}
