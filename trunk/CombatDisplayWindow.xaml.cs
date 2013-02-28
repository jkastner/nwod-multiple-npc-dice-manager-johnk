using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for CombatDisplayWindow.xaml
    /// </summary>
    public partial class CombatDisplayWindow : Window
    {
        private RosterViewModel _viewModel;


        public CombatDisplayWindow(RosterViewModel _viewModel)
        {
            DataContext = _viewModel;
            // TODO: Complete member initialization
            this._viewModel = _viewModel;
            InitializeComponent();

        }
    }
}
