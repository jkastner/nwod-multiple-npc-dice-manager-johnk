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

        public Board RegisterNewBoard()
        {
            var newB = Board.NewBoard();
            _boards.Add(newB);
            OnBoardRegistered(new BoardRegisteredEventArgs(newB));
            return newB;
        }

        private void DeregisterBoard(Board cur)
        {
            cur.Dispose();
            OnBoardDeregistered(new BoardRegisteredEventArgs(cur));
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
                curBoard.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => action(local)));
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
        }



        public void ImportBoardFromSave(Board cur)
        {
            Boards.Add(cur);
            cur.VisualsViewModel.OpenBoardInfo(cur.VisualsViewModel.CurrentBoardInfo);
            OnBoardRegistered(new BoardRegisteredEventArgs(cur));
        }
    }
}
