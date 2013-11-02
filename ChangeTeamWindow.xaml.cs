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

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for ChangeTeamWindow.xaml
    /// </summary>
    public partial class ChangeTeamWindow : Window
    {
        public ChangeTeamWindow()
        {
            InitializeComponent();
            TeamSelection_ListBox.ItemsSource = CombatService.RosterViewModel.Teams;
        }

        public bool WasCancel = false;
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = false;
            this.Close();
        }

        public Team ChosenTeam
        {
            get
            {
                if (TeamSelection_ListBox.SelectedItem != null)
                {
                    var newTeam = TeamSelection_ListBox.SelectedItem as Team;
                    return newTeam;
                }
                return null;
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = true;
            this.Close();
        }
    }
}
