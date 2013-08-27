using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class BoardRegisteredEventArgs : EventArgs
    {
        public Board NewBoard { get; set; }
        public BoardRegisteredEventArgs(Board newBoard)
        {
            NewBoard = newBoard;
        }
    }
}
