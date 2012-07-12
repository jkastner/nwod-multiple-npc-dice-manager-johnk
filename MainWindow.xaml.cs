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

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Roster _roster = new Roster();
        private Traits _traits = new Traits();

        public MainWindow()
        {
            InitializeComponent();
            TempPopulateCharacters();
            



        }

        private void TempPopulateCharacters()
        {
            Character Bob = new Character("Bob");
            Character Tim = new Character("Tim");
            Character Zazu = new Character("Zazu");



            NumberedTrait b1 = new NumberedTrait(100, _traits.AddIfNew("IQ"));
            NumberedTrait b2 = new NumberedTrait(2, _traits.AddIfNew("Age"));

            NumberedTrait t1 = new NumberedTrait(120, _traits.AddIfNew("IQ"));
            NumberedTrait t2 = new NumberedTrait(74, _traits.AddIfNew("Age"));

            NumberedTrait z1 = new NumberedTrait(70, _traits.AddIfNew("IQ"));
            NumberedTrait z2 = new NumberedTrait(44, _traits.AddIfNew("Age"));



            Bob.NumberedTraits.Add(b1);
            Bob.NumberedTraits.Add(b2);
            Tim.NumberedTraits.Add(t1);
            Tim.NumberedTraits.Add(t2);
            Zazu.NumberedTraits.Add(z1);
            Zazu.NumberedTraits.Add(z2);

            _roster.Add(Bob);
            _roster.Add(Tim);
            _roster.Add(Zazu);


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _roster;
            AllAvailableTraits.DataContext = _traits;

        }

        private void SumSingle_Click(object sender, RoutedEventArgs e)
        {
            int q = 0;
            foreach (NumberedTrait selectedTrait in SingleTraitSelectionBox.SelectedItems)
            {
                q += selectedTrait.TraitValue;
            }
            MessageBox.Show(q.ToString());
        }

        private void SumAll_Click(object sender, RoutedEventArgs e)
        {
            String result = "";
            foreach (Character curChar in SelectedCharacters.SelectedItems)
            {
                int q = 0;
                foreach (Trait selectedTrait in AllAvailableTraits.SelectedItems)
                {
                    NumberedTrait foundTrait = curChar.FindTrait(selectedTrait.TraitName);
                    q += foundTrait.TraitValue;
                }
                result = result+"\n"+curChar.Name + " - " + q;
            }
            MessageBox.Show(result);
        }



        private void SelectedCharacter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCharacters.SelectedItems.Count > 1)
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



    }
}
