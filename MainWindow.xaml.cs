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
using System.Text.RegularExpressions;

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

        private void Roll_Button_Click(object sender, RoutedEventArgs e)
        {
            List<String> results = _viewModel.RollCharacters(ActiveCharacters_ListBox.SelectedItems, CurrentTraits_ListBox.SelectedItems).ToList();
            foreach(String curResult in results)
            {
                MessageBox.Show(curResult);
            }
        }

        private void Do_Bashing_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DoBashing(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Do_Lethal_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DoLethal(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Do_Aggrivated_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DoAggrivated(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Reset_Health_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetHealth(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Initiative_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollInitiative();
        }

        private void RemoveCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveActiveCharacters(ActiveCharacters_ListBox.SelectedItems);
        }

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void Modifier_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            
            {
                e.Handled = true;
            }
        }

        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            Regex regex = new Regex("[^0-9-]"); //regex that matches disallowed text
            bool isGood = !regex.IsMatch(text);
            return isGood;
        }


        private void Modifier_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isGood = IsTextNumeric(e.Text);
            e.Handled = !isGood;
            
        }

        // Use the DataObject.Pasting Handler 
        private void Modifier_TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}





