using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GameBoard;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for SelectTarget.xaml
    /// </summary>
    public partial class SelectTarget : Window
    {
        ObservableCollection<CharacterSheet> targetCharacters = new ObservableCollection<CharacterSheet>();
        ObservableCollection<String> attackTraits = new ObservableCollection<String>();
        ObservableCollection<String> otherTraits = new ObservableCollection<String>();
        List<CharacterSheet> _selectedCharacters = new List<CharacterSheet>();
        VisualsViewmodel _visualsViewModel;
        public SelectTarget(IList selectedCharacters, ObservableCollection<CharacterSheet> allCharacters, 
            ObservableCollection<string> damageTypes, VisualsViewmodel visualsViewModel)
        {
            _visualsViewModel = visualsViewModel;
            _allCharacters = allCharacters;
            foreach (var curChar in allCharacters)
            {
                targetCharacters.Add(curChar);
            }

            foreach (var curItem in selectedCharacters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                _selectedCharacters.Add(curChar);
                targetCharacters.Remove(curChar);
                foreach (var curTrait in curChar.Traits)
                {
                    var attackTrait = curTrait as AttackTrait;
                    if (attackTrait != null)
                    {
                        if (!attackTraits.Contains(attackTrait.TraitLabel))
                        {
                            attackTraits.Add(attackTrait.TraitLabel);
                        }
                    }
                    else
                    {
                        if (!otherTraits.Contains(curTrait.TraitLabel))
                            otherTraits.Add(curTrait.TraitLabel);
                    }
                }
            }
            InitializeComponent();
            
            
            TargetCharacters_ListBox.ItemsSource = targetCharacters;
            Shared_Attacks_ListBox.ItemsSource = attackTraits;
            Other_Traits_ListBox.ItemsSource = otherTraits;
            DamageTypes_ListBox.ItemsSource = damageTypes;
            TargetCharacters_ListBox.SelectedIndex = 0;
            Shared_Attacks_ListBox.SelectedIndex = 0;
            
        }

        bool _wasCancel;
        private ObservableCollection<CharacterSheet> _allCharacters;
        public bool WasCancel
        {
            get { return _wasCancel; }
            set { _wasCancel = value; }
        }

        public String ChosenAttack
        {
            get { return Shared_Attacks_ListBox.SelectedItem as String; }
        }

        public CharacterSheet SelectedTarget
        {
            get { return TargetCharacters_ListBox.SelectedItem as CharacterSheet; }
        }

        public String WoundType
        {
            get { return DamageTypes_ListBox.SelectedItem as String; }
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            WasCancel = true;
            this.Close();
        }

        private void GetName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OK_Button_Click(sender, e);
            }
            if (e.Key == Key.Escape)
            {
                Cancel_Button_Click(sender, e);
            }
        }

        private void Shared_Attacks_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AttackTrait attack = FindAttackTrait(Shared_Attacks_ListBox.SelectedItem as String);
            
            foreach (var curDamageType in DamageTypes_ListBox.Items)
            {
                String damageString = curDamageType as String;
                if (damageString.Equals(attack.DamageType))
                {
                    DamageTypes_ListBox.SelectedItem = curDamageType;
                }
            }
        }

        private AttackTrait FindAttackTrait(String attackName)
        {
            foreach (CharacterSheet curChar in _allCharacters)
            {
                foreach (Trait curTrait in curChar.Traits)
                {
                    if (curTrait.TraitLabel.Equals(attackName))
                    {
                        return curTrait as AttackTrait;
                    }
                }
            }
            return null;
        }

        private void PotentialTargetChanged_TargetCharacters_ListBox(object sender, SelectionChangedEventArgs e)
        {
            if(SelectedTarget.Visual==null)
                return;
            foreach (var cur in _selectedCharacters)
            {
                if (cur.Visual != null)
                {
                    _visualsViewModel.DrawAttack(cur.Visual, SelectedTarget.Visual, cur.Visual.PieceColor, new Duration(new TimeSpan(0,0,0,3)));
                }
            }
        }

    }
}
