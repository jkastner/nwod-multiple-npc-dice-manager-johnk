using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XMLCharSheets
{
    /// <summary>
    ///     Interaction logic for PathfinderControl.xaml
    /// </summary>
    public partial class PathfinderControl : UserControl
    {
        private readonly RosterViewModel _viewModel;

        public PathfinderControl()
        {
            InitializeComponent();
            _viewModel = CombatService.RosterViewModel;
            DataContext = _viewModel;
        }

        private void WillSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> {"Will"});
        }


        private void ReflexSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> {"Reflex"});
        }

        private void FortitudeSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> {"Fortitude"});
        }

        protected IList ActiveList()
        {
            var parentWindow = Window.GetWindow(this) as MainWindow;
            return parentWindow.ActiveList();
        }


        private void DoDamage_ButtonClick(object sender, RoutedEventArgs e)
        {
            int sliderValue = -(int) DamageSlider.Value;
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

        private void Pathfinder_SingleAttack_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PathfinderSingleAttack(ActiveList());
        }
    }
}