using System;
using System.Collections;
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

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for PathfinderControl.xaml
    /// </summary>
    public partial class PathfinderControl : UserControl
    {
        RosterViewModel _viewModel;
        public PathfinderControl()
        {
            InitializeComponent();
            _viewModel = ViewModelService.RosterViewModel;
            DataContext = _viewModel;
        }

        private void WillSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String>() { "Will" });
        }

   
        private void ReflexSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String>() { "Reflex" });
        }

        private void FortitudeSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String>() { "Fortitude" });
        }
        protected IList ActiveList()
        {
            MainWindow parentWindow = Window.GetWindow(this) as MainWindow;
            return parentWindow.ActiveList();
        }


        private void DoDamage_ButtonClick(object sender, RoutedEventArgs e)
        {
            int sliderValue = -(int)DamageSlider.Value;
            _viewModel.DoDamage(ActiveList(), sliderValue, DamageDescriptor_TextBox.Text);
        }

        private void DamageBox_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DoDamage_ButtonClick(sender, e);
            }
            int newVal = 0;
            if (int.TryParse(DamageValue_TextBox.Text, out newVal))
            {
                DamageSlider.Value = newVal;
            }
        }
    }
}
