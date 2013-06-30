﻿using System;
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
using XMLCharSheets.NWoD;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for NWoDControl.xaml
    /// </summary>
    public partial class NWoDControl : UserControl
    {


        RosterViewModel _viewModel;
        NWoDRosterViewModel _nwodViewModel = new NWoDRosterViewModel();
        public NWoDControl()
        {
            InitializeComponent();
            _viewModel = CombatService.RosterViewModel;
            DataContext = _viewModel;
        }

        protected IList ActiveList()
        {
            MainWindow parentWindow = Window.GetWindow(this) as MainWindow;
            return parentWindow.ActiveList();
        }


        private void Do_Bashing_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _nwodViewModel.DoBashing(ActiveList());
        }

        private void Do_Lethal_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _nwodViewModel.DoLethal(ActiveList());
        }

        private void Do_Aggrivated_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _nwodViewModel.DoAggrivated(ActiveList());
        }

        private bool CheckValidActive()
        {
            MainWindow parentWindow = Window.GetWindow(this) as MainWindow;
            return parentWindow.CheckValidActive();
        }


        private void Blood_Heal_Button_Click(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.BloodHeal(ActiveList());
        }

        private void Blood_Buff_Button_Click(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.BloodBuff(ActiveList());
        }

        private void Refill_Vitae_Button_Click(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.RefillVitae(ActiveList());
        }
    }
}
