using System.Collections;
using System.Windows;
using System.Windows.Controls;
using XMLCharSheets.NWoD;

namespace XMLCharSheets
{
    /// <summary>
    ///     Interaction logic for NWoDControl.xaml
    /// </summary>
    public partial class NWoDControl : UserControl
    {
        private readonly NWoDRosterViewModel _nwodViewModel = new NWoDRosterViewModel();
        private readonly RosterViewModel _viewModel;

        public NWoDControl()
        {
            InitializeComponent();
            _viewModel = CombatService.RosterViewModel;
            DataContext = _viewModel;
        }

        protected IList ActiveList()
        {
            var parentWindow = Window.GetWindow(this) as MainWindow;
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
            var parentWindow = Window.GetWindow(this) as MainWindow;
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

        private void Roll_Resolve_and_Resistance(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.Roll_Resolve_and_Resistance(ActiveList());
        }

        private void Roll_Composure_and_Resistance(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.Roll_Composure_and_Resistance(ActiveList());
        }

        private void Roll_Resolve_And_Composure(object sender, RoutedEventArgs e)
        {
            _nwodViewModel.Roll_Resolve_And_Composure(ActiveList());
        }
    }
}