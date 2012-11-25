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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RosterViewModel _viewModel = new RosterViewModel();

        public MainWindow()
        {
            InitializeComponent();
            _viewModel.PopulateCharacters(Directory.GetCurrentDirectory()+"\\Sheets");
            this.DataContext = _viewModel;
            
        }


        private void AddCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedFullCharacter == null)
                return;
            GetCharacterName gcn = new GetCharacterName(_viewModel.SelectedFullCharacter.Name, _viewModel.ActiveRoster);
            gcn.ShowDialog();
            if (!gcn.WasCancel)
            {
                CharacterSheet newInstance = CharacterSheet.Copy(_viewModel.SelectedFullCharacter, gcn.ProvidedName);
                _viewModel.ActiveRoster.Add(newInstance);
            }
        }

        private void ActiveCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.CurrentTraits.Clear();
            if (ActiveCharacters_ListBox.SelectedItems.Count == 0)
            {
                return;
            }
            List<String> curTraits = new List<String>();
            foreach (var cur in ActiveCharacters_ListBox.SelectedItems)
            {
                CharacterSheet curChar = cur as CharacterSheet;
                foreach (var curTrait in curChar.NumberedTraits)
                {
                    curTraits.Add(curTrait.TraitLabel);
                }
            }
            curTraits = curTraits.Distinct().ToList();
            foreach (var cur in curTraits)
            {
                _viewModel.CurrentTraits.Add(cur);
            }
        }







    }
}
