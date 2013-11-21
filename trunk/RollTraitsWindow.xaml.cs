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

namespace CombatAutomationTheater
{
    /// <summary>
    /// Interaction logic for RollTraitsWindow.xaml
    /// </summary>
    public partial class RollTraitsWindow : Window
    {
        public RollTraitsWindow(List <CharacterSheet> selectedCharacters)
        {
            InitializeComponent();
            List<String> sharedTraits = new List<String>();
            List<String> unsharedTraits = new List<String>();
            foreach (var curCharacter in selectedCharacters)
            {
                foreach (var curTrait in curCharacter.NumericTraits)
                {
                    if (!sharedTraits.Contains(curTrait.TraitLabel))
                    {
                        sharedTraits.Add(curTrait.TraitLabel);
                    }
                }
            }
            foreach(var curCharacter in selectedCharacters)
            {
                List<String> toRemove = new List<String>();
                foreach (var cur in sharedTraits)
                {
                    if (!curCharacter.HasTrait(cur))
                    {
                        toRemove.Add(cur);
                    }
                }
                foreach (var cur in toRemove)
                {
                    sharedTraits.Remove(cur);
                    unsharedTraits.Add(cur);
                }
            }


            foreach (var cur in sharedTraits)
            {
                AvailableTraits_ListBox.Items.Add(cur);
            }
            foreach (var cur in unsharedTraits)
            {
                UnsharedTraits_ListBox.Items.Add(cur);
            }
            if ((UnsharedTraits_ListBox.Items.Count == 0))
            {
                UnsharedTraits_Column.Width = new GridLength(0);
            }
            if ((AvailableTraits_ListBox.Items.Count == 0))
            {
                SharedTraits_Column.Width = new GridLength(0);
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
                    yield return cur.ToString();
                }
                foreach (var cur in UnsharedTraits_ListBox.SelectedItems)
                {
                    yield return cur.ToString();
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
