using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    public class VisualsService
    {
        public static BoardsViewModel BoardsViewModel
        {
            get { return BoardsViewModel.Instance; }
        }

    }
}
