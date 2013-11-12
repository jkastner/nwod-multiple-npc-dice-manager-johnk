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
    /// Interaction logic for RollTraitsWindow.xaml
    /// </summary>
    public partial class RollTraitsWindow : Window
    {
        public RollTraitsWindow(List <CharacterSheet> selectedCharacters)
        {
            InitializeComponent();
            List<NumericIntTrait> _allTraits = new List<NumericIntTrait>();
            foreach(var cur in selectedCharacters)
            {
                var numericTraits = cur.NumericTraits;
                if (_allTraits.Any())
                {
                    foreach (var curTrait in numericTraits)
                    {
                        if (!_allTraits.Any(x => x.TraitLabel.Equals(curTrait.TraitLabel)))
                        {
                            UnsharedTraits_ListBox.Items.Add(curTrait);
                        }
                    }
                }
                else
                {
                    _allTraits.AddRange(numericTraits);
                    foreach (var curTrait in _allTraits)
                    {
                        AvailableTraits_ListBox.Items.Add(curTrait);
                    }
                }
            }
            if (!(UnsharedTraits_ListBox.Items.Count > 0))
            {
                UnsharedTraits_Column.Width = new GridLength(0);
            }
            WasCancel = true;
        }
        public bool WasCancel { get; set; }

        public IEnumerable<String> SelectedTraitLabels
        {
            get
            {
                foreach (var cur in AvailableTraits_ListBox.SelectedItems)
                {
                    var curtrait = cur as Trait;
                    yield return curtrait.TraitLabel;
                }
                foreach (var cur in UnsharedTraits_ListBox.SelectedItems)
                {
                    var curtrait = cur as Trait;
                    yield return curtrait.TraitLabel;
                }
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = true;
            this.Close();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = false;
            this.Close();
        }
    }
}
