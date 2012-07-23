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
        private Roster _fullRoster = new Roster();
        private Roster _activeRoster = new Roster();
        private Traits _traits = new Traits();
        public RollDice _rollDice = new RollDice(5);

        public MainWindow()
        {
            InitializeComponent();
            PopulateCharacters(Directory.GetCurrentDirectory()+"\\Sheets");
        }

        private void PopulateCharacters(String sourceDir)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(sourceDir);
            foreach (string fileName in fileEntries)
            {
                _fullRoster.Add(Character.MakeChar(fileName, _traits));
            }


            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            foreach (string subdir in subdirEntries)
            {
                PopulateCharacters(subdir);
            }
        }






        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AllCharacters_Listbox.DataContext = _fullRoster;
            AllAvailableTraits.DataContext = _traits;
            this.DataContext = _activeRoster;

        }

        private void SumSingle_Click(object sender, RoutedEventArgs e)
        {
            int diceToRoll = 0;
            String result = "";
            foreach (NumberedTrait selectedTrait in SingleTraitSelectionBox.SelectedItems)
            {
                diceToRoll += selectedTrait.TraitValue;
            }
            diceToRoll += GetModifier();
            _rollDice.NumberOfDice = diceToRoll;
            
            _rollDice.Roll();

            Character curChar = (Character)AllCharacters_Listbox.SelectedItem;
            result = result + "\n" + curChar.Name + " (Pool " + _rollDice.NumberOfDice + "):  " +
                     _rollDice.CurrentSuccesses + "\n" + _rollDice.ResultDescription;
            MessageBox.Show(result);
        }

        private void SumAll_Click(object sender, RoutedEventArgs e)
        {
            String result = "";
            foreach (Character curChar in AllCharacters_Listbox.SelectedItems)
            {
                int diceToRoll = 0;
                bool errorFindingTrait = false;
                foreach (Trait selectedTrait in AllAvailableTraits.SelectedItems)
                {
                    NumberedTrait foundTrait = curChar.FindTrait(selectedTrait.TraitName);
                    if (foundTrait == null)
                    {
                        result = result+ "\n" +curChar.Name + " did not have " + selectedTrait.TraitName;
                        errorFindingTrait = true;
                        break;
                    }
                    else
                    {
                        diceToRoll += foundTrait.TraitValue;
                    }
                }
                if (!errorFindingTrait)
                {
                    int modifier = GetModifier();
                    diceToRoll += modifier;
                    _rollDice.NumberOfDice = diceToRoll;
                    _rollDice.Roll();
                    result = result + "\n" + curChar.Name + " (Pool " + _rollDice.NumberOfDice + "):  " +
                             _rollDice.CurrentSuccesses + "\n" + _rollDice.ResultDescription;
                }
            }
            MessageBox.Show(result);
        }

        private int GetModifier()
        {
            String boxContents = ModifierTextBox.Text;
            try
            {
                return Int32.Parse(boxContents);
            }
            catch (Exception)
            {

                return 0;
            }
        }



        private void ActiveCharactersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveCharactersListBox.SelectedItems.Count > 1)
            {
                //More than one character is selected.
                AllAvailableTraits.Visibility = System.Windows.Visibility.Visible;
                SingleTraitSelectionBox.Visibility = System.Windows.Visibility.Hidden;
                SumAllButton.Visibility = System.Windows.Visibility.Visible;
                SumSingleButton.Visibility = System.Windows.Visibility.Hidden;
                Label_EditTraitValue.Visibility = System.Windows.Visibility.Hidden;
                UpdateTraitValueBox.Visibility = System.Windows.Visibility.Hidden;
                SelectedTraitsMultiple.Visibility = System.Windows.Visibility.Visible;
                SelectedTraitsSingle.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                //One or zero characters are selected.
                AllAvailableTraits.Visibility = System.Windows.Visibility.Hidden;
                SingleTraitSelectionBox.Visibility = System.Windows.Visibility.Visible;
                SumAllButton.Visibility = System.Windows.Visibility.Hidden;
                SumSingleButton.Visibility = System.Windows.Visibility.Visible;
                Label_EditTraitValue.Visibility = System.Windows.Visibility.Visible;
                UpdateTraitValueBox.Visibility = System.Windows.Visibility.Visible;
                SelectedTraitsMultiple.Visibility = System.Windows.Visibility.Hidden;
                SelectedTraitsSingle.Visibility = System.Windows.Visibility.Visible;
            }
            
        }


        private void SingleTraitSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SingleTraitSelectionBox.SelectedItems.Count == 1)
            {
                this.SelectedTraitsSingle.SelectedIndex = 0;
            }
        }

        private void AddToActiveCharactersButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Character cur in AllCharacters_Listbox.SelectedItems)
            {
                Character copyCharacter = Character.CopyChar(cur, _traits);
                _activeRoster.Add(copyCharacter);
            }
        }

        private void RemoveFromActiveCharactersButton_Click(object sender, RoutedEventArgs e)
        {
            for (int index = ActiveCharactersListBox.SelectedItems.Count - 1; index >= 0; index--)
            {
                _activeRoster.Remove(ActiveCharactersListBox.SelectedItems[index] as Character);
            }
        }


    }
}
