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
using System.Windows.Shapes;

namespace GameBoard
{
    /// <summary>
    /// Interaction logic for GameBoardVisual_Window.xaml
    /// </summary>
    public partial class GameBoardVisual_Window : Window
    {
        private Board _board;

        public GameBoardVisual_Window(Board board)
        {
            InitializeComponent();
            this._board = board;
            MainGrid.Children.Add(board.GameBoardVisual);
        }
    }
}
