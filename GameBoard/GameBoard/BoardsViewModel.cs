using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GameBoard
{
    public class BoardsViewModel
    {
        public const String MainBoardName = "MainBoard";
        public const String VisualTabBoardName = "VisualTab";
        public const String WindowBoardName = "Window";
        public const String TargetBoardName = "TargetBoard";
        private static BoardsViewModel _instance;
        public static BoardsViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BoardsViewModel();
                }
                return _instance;
            }
        }

        private List<Board> _boards = new List<Board>();
        public List<Board> Boards
        {
            get { return _boards; }
        }

        public Board CreateAndRegisterNewBoard(string boardName)
        {
            var newB = Board.NewBoard(boardName);
            RegisterBoard(newB);
            return newB;
        }
        public void RemoveBoard(Board targetBoard)
        {
            DeregisterBoard(targetBoard);
        }
        private void DeregisterBoard(Board oldBoard)
        {
            oldBoard.Dispose();
            foreach (Board curboard in _boards)
            {
                oldBoard.VisualsViewModel.PieceMoved -= curboard.VisualsViewModel.OtherBoardPieceMoved;
                oldBoard.GameBoardVisual.ShapeDrawn -= curboard.GameBoardVisual.OtherBoardShapeDrawn;

                curboard.VisualsViewModel.PieceMoved -= oldBoard.VisualsViewModel.OtherBoardPieceMoved;
                curboard.GameBoardVisual.ShapeDrawn -= oldBoard.GameBoardVisual.OtherBoardShapeDrawn;

            }
            OnBoardDeregistered(new BoardRegisteredEventArgs(oldBoard));
        }

        public Board MainBoard
        {
            get
            {
                return _boards.Where(x => x.BoardName.Equals(MainBoardName)).FirstOrDefault(); ;
            }
        }

        public event EventHandler BoardRegistered;
        protected virtual void OnBoardRegistered(BoardRegisteredEventArgs e)
        {
            EventHandler handler = BoardRegistered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler BoardDeregistered;
        protected virtual void OnBoardDeregistered(BoardRegisteredEventArgs e)
        {
            EventHandler handler = BoardDeregistered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void ForeachBoard(Action<Board> action)
        {
            foreach (Board curBoard in _boards)
            {
                Board local = curBoard;
                curBoard.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => action(local)));
            }
        }

        public bool HasAssociatedVisual(Guid characterGuid)
        {
            return _boards.First().VisualsViewModel.HasAssociatedVisual(characterGuid);
        }
        public MoveablePicture AssociatedVisual(Guid characterGuid)
        {
            if(HasAssociatedVisual(characterGuid))
                return _boards.First().VisualsViewModel.MatchingVisual(characterGuid);
            return null;
        }


        public void ClearAllBoards()
        {
            foreach (var cur in Boards)
            {
                DeregisterBoard(cur);
            }
            Boards.Clear();
        }



        public void ImportBoardFromSave(Board cur)
        {
            cur.GameBoardVisual = new GameBoardVisual();
            cur.GameBoardVisual.RegisterViewModel(cur.VisualsViewModel);

            cur.VisualsViewModel.SetGameBoardVisual(cur.GameBoardVisual);
            cur.VisualsViewModel.OpenBoardInfo(cur.VisualsViewModel.CurrentBoardInfo);
            cur.VisualsViewModel.OpenVisuals();
        }

        public void RegisterBoard(Board newB)
        {
            foreach (Board curboard in _boards)
            {
                newB.VisualsViewModel.PieceMoved += curboard.VisualsViewModel.OtherBoardPieceMoved;
                newB.GameBoardVisual.ShapeDrawn += curboard.GameBoardVisual.OtherBoardShapeDrawn;

                curboard.VisualsViewModel.PieceMoved += newB.VisualsViewModel.OtherBoardPieceMoved;
                curboard.GameBoardVisual.ShapeDrawn += newB.GameBoardVisual.OtherBoardShapeDrawn;

            }
            _boards.Add(newB);
            OnBoardRegistered(new BoardRegisteredEventArgs(newB));
        }


        public void ZoomTo(List<Guid> visuals, bool useMainOnly)
        {
            if (!useMainOnly)
            {
                ForeachBoard(x => x.VisualsViewModel.ZoomTo(visuals));
            }
            else
            {
                MainBoard.VisualsViewModel.ZoomTo(visuals);
            }
        }
    }
}
