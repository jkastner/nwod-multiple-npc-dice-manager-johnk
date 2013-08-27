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

        public event EventHandler BoardRegistered;
        protected virtual void OnBoardRegistered(BoardRegisteredEventArgs e)
        {
            EventHandler handler = BoardRegistered;
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


    }
}
