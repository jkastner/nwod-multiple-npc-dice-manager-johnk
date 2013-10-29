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
    public partial class GameBoardVisual : UserControl
    {
        Material _frontMaterial, _backMaterial;


        private VisualsViewModel _viewModel;

        public GameBoardVisual()
        {
            InitializeComponent();

            AddLights();
            Loaded += ZoomOnLoad;


            Viewport.Background = new SolidColorBrush(Colors.SkyBlue);
            Viewport.Camera.Position = new Point3D(1, 52, 5);
            Viewport.Camera.UpDirection = new Vector3D(0, 1, 1);
            Viewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            Viewport.IsRotationEnabled = false;
        }

        public void AddLights()
        {
            Viewport.Children.Add(new DefaultLights());
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, 1)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));

            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 1, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));

            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, 0, 0)));
            Viewport.Lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 0, 0)));
        }

        public void RegisterViewModel(VisualsViewModel ViewModel)
        {
            _viewModel = ViewModel;
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
                rayMeshResult.VisualHit.Equals(_viewModel.GroupSingleMovementCircle) ||
                rayMeshResult.VisualHit.Equals(_viewModel.VisualGrid))
            {
                return HitTestResultBehavior.Continue;
            }
            if (_viewModel.TapeMeasurerActive)
            {
                _viewModel.HandleTapeMeasurerHit(rayMeshResult);
                return HitTestResultBehavior.Stop;

            }
            foreach (var curVisual in _viewModel.CharactersToMoveablePicture)
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
            if (_viewModel.ShapeSelection == GameBoard.VisualsViewModel.ShapeMode.None)
            {
                if (!doubleClick && rayMeshResult.VisualHit is RectangleVisual3D)
                {
                    if (!multiSelect)
                        _viewModel.ClearSelectedCharacters();
                    _viewModel.ToggleSelectCharacterFromVisual(rayMeshResult.VisualHit as RectangleVisual3D);
                }
                else if (doubleClick)
                {
                    if (_viewModel.CharactersToMoveablePicture.Any(x => x.Value.IsSelected))
                    {
                        var movedHit = rayMeshResult.PointHit;
                        _viewModel.MoveSelectedPiecesTo(movedHit);
                    }
                }
            }
            else
            {
                DrawShape(rayMeshResult.PointHit, true);
            }
        }

        List<Point3D> _drawnPoints = new List<Point3D>();
        private void DrawShape(Point3D point3D, bool thisBoardOriginatedTheShape)
        {
            _drawnPoints.Add(point3D);
            switch (_viewModel.ShapeSelection)
            {
                case GameBoard.VisualsViewModel.ShapeMode.Sphere:
                    {
                        _viewModel.DrawSphere(point3D);
                        if (thisBoardOriginatedTheShape)
                            OnShapeDrawn(new ShapeDrawnEvent(_drawnPoints));
                        _drawnPoints.Clear();
                        break;
                    }
                case GameBoard.VisualsViewModel.ShapeMode.Cone:
                    {
                        if (_drawnPoints.Count==2)
                        {
                            _viewModel.DrawCone(_drawnPoints.First(), point3D);
                            if(thisBoardOriginatedTheShape)
                                OnShapeDrawn(new ShapeDrawnEvent(_drawnPoints));
                            _drawnPoints.Clear();
                        }
                        break;
                    }
                case GameBoard.VisualsViewModel.ShapeMode.Line:
                    {
                        if (_drawnPoints.Count==2)
                        {
                            _viewModel.DrawLine(_drawnPoints.First(), point3D);
                            if (thisBoardOriginatedTheShape)
                                OnShapeDrawn(new ShapeDrawnEvent(_drawnPoints));
                            _drawnPoints.Clear();
                        }
                        break;
                    }
                default:
                    throw new Exception("Unknown shape");
                    break;
            }



        }

        public event EventHandler ShapeDrawn;
        protected virtual void OnShapeDrawn(ShapeDrawnEvent e)
        {
            EventHandler handler = ShapeDrawn;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        bool doubleClick = false;
        private void Viewport_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            doubleClick = true;
            ViewportMouseDown(sender, e);
            doubleClick = false;
        }



        internal void OtherBoardShapeDrawn(object sender, EventArgs e)
        {
            var shapeDrawnEvent = e as ShapeDrawnEvent;
            foreach (var cur in shapeDrawnEvent.Points)
            {
                DrawShape(cur, false);
            }
        }

        internal void RemoveVisual(Visual3D toRemove)
        {
            if (toRemove == null)
                return;
            if (Viewport.Children.Contains(toRemove))
            {
                Viewport.Children.Remove(toRemove);
            }
        }

        internal void AddVisual(Visual3D toAdd)
        {
            if (toAdd == null)
                return;
            if (!Viewport.Children.Contains(toAdd))
            {
                Viewport.Children.Add(toAdd);
            }
        }

        Stack<Rect3D> _lastZoom = new Stack<Rect3D>();
        internal void ZoomTo(Rect3D rect3D, int zoomSpeed)
        {
            //When the board isn't loaded, zooming using helix had an odd unfixable crash.
            //this dodges that.
            if (this.ActualHeight != 0 && this.ActualWidth != 0)
            {
                Viewport.ZoomExtents(rect3D, zoomSpeed);
            }
            else
            {
                _lastZoom.Push(rect3D);
            }
        }

        private void ZoomOnLoad(object sender, RoutedEventArgs e)
        {
            if (_lastZoom.Count > 0)
            {
                ZoomTo(_lastZoom.Pop(), 0);
                _lastZoom.Clear();
            }
        }

        public void MatchOtherCamera(ProjectionCamera otherCamera)
        {
            Viewport.Camera.LookDirection = otherCamera.LookDirection;
            Viewport.Camera.Position = otherCamera.Position;
            Viewport.Camera.UpDirection = otherCamera.UpDirection;
        }

        public ProjectionCamera Camera
        {
            get
            {
                return Viewport.Camera;
            }
        }


        public bool LockRotation 
        {
            get
            {
                return Viewport.IsRotationEnabled;
            }
            set
            {
                Viewport.IsRotationEnabled = !value;
            }
        }
    }
}
