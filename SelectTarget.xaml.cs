using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using GameBoard;

namespace XMLCharSheets
{

    /// <summary>
    /// Interaction logic for SelectTarget.xaml
    /// </summary>
    public partial class SelectTarget : Window, INotifyPropertyChanged
    {
        ObservableCollection<CharacterSheet> targetCharacters = new ObservableCollection<CharacterSheet>();
        ObservableCollection<String> attackTraits = new ObservableCollection<String>();
        ObservableCollection<String> otherAttackTraits = new ObservableCollection<String>();
        ObservableCollection<String> otherTraits = new ObservableCollection<String>();
        List<CharacterSheet> _selectedCharacters = new List<CharacterSheet>();
        VisualsViewModel _visualsViewModel;
        public SelectTarget(IList selectedObjects, ObservableCollection<CharacterSheet> allCharacters, 
            ObservableCollection<string> damageTypes, VisualsViewModel visualsViewModel)
        {

            List<CharacterSheet> selectedCharacters = new List<CharacterSheet>();
            foreach (var cur in selectedObjects)
            {
                selectedCharacters.Add(cur as CharacterSheet);
            }
            List<CharacterSheet> allTargets = new List<CharacterSheet>();
            _visualsViewModel = visualsViewModel;
            _allCharacters = allCharacters;
            foreach (var curChar in allCharacters)
            {
                allTargets.Add(curChar);
            }
            //1. Remove all attackers from list of targets.
            List<Team> attackingTeams = new List<Team>();
            foreach (var curChar in selectedCharacters)
            {
                if (!attackingTeams.Contains(curChar.Team))
                {
                    attackingTeams.Add(curChar.Team);
                }
                _selectedCharacters.Add(curChar);
                allTargets.Remove(curChar);
                //2. Build list of possible attack traits.
                foreach (var curTrait in curChar.NumericTraits)
                {
                    var attackTrait = curTrait as AttackTrait;
                    if (attackTrait != null)
                    {
                        if (!attackTraits.Contains(attackTrait.TraitLabel))
                        {
                            attackTraits.Add(attackTrait.TraitLabel);
                            otherAttackTraits.Add(attackTrait.TraitLabel);
                        }
                    }
                    else
                    {
                        if (!otherTraits.Contains(curTrait.TraitLabel))
                            otherTraits.Add(curTrait.TraitLabel);
                    }
                }
            }
            //3. Put targets that share a team with at least on attacker in a separate list.
            IEnumerable<CharacterSheet> teammateTargets;
            teammateTargets = from element in allTargets where attackingTeams.Contains(element.Team) select element;
            //teammateTargets.Select(targetCharacters.Where(x => attackingTeams.Contains(x.Team)));
            var nonTeammateTargets = allTargets.Except(teammateTargets);
            //4. Sort the remaining targets by distance to a common 'origin' of the group.
            Point3D commonOrigin = Helper3DCalcs.FindMidpoint(selectedCharacters.Where(x=>x.Visual!=null).Select(x=>x.Visual.Location));
            nonTeammateTargets = nonTeammateTargets.OrderBy(x => x.DistanceTo(commonOrigin));
            teammateTargets = teammateTargets.OrderBy(x => x.DistanceTo(commonOrigin));
            foreach (var cur in nonTeammateTargets)
            {
                targetCharacters.Add(cur);
            }
            foreach (var cur in teammateTargets)
            {
                targetCharacters.Add(cur);
            }
            DataContext = this;
            InitializeComponent();


            Other_Attacks_ListBox.ItemsSource = otherAttackTraits;
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

        public List<String> OtherAttacks
        {
            get
            {
                List<String> attacks = new List<string>();
                foreach(var cur in Other_Attacks_ListBox.SelectedItems)
                {
                    attacks.Add(cur as string);
                }
                return attacks;
            }
        }

        private CharacterSheet _selectedTarget;
        public CharacterSheet SelectedTarget
        {
            get { return _selectedTarget; }
            set 
            { 
                _selectedTarget = value;
                OnPropertyChanged("SelectedTarget");
            }
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
            RefreshOtherAttacks(attack.TraitLabel);
            foreach (var curDamageType in DamageTypes_ListBox.Items)
            {
                String damageString = curDamageType as String;
                if (damageString.Equals(attack.DamageType))
                {
                    DamageTypes_ListBox.SelectedItem = curDamageType;
                    break;
                }
            }

        }

        private void RefreshOtherAttacks(string mainAttack)
        {
            List <String> previousItems = new List<string>();
            foreach (var cur in Other_Traits_ListBox.SelectedItems)
            {
                previousItems.Add(cur as String);
            }
            otherAttackTraits.Clear();
            foreach (var cur in attackTraits)
            {
                if (!cur.Equals(mainAttack))
                {
                    otherAttackTraits.Add(cur);
                }
            }
            foreach (var previousSelected in previousItems)
            {
                if (!previousSelected.Equals(mainAttack))
                {
                    Other_Attacks_ListBox.SelectedItems.Add(previousSelected);
                }
            }
        }


        private AttackTrait FindAttackTrait(String attackName)
        {
            foreach (CharacterSheet curChar in _allCharacters)
            {
                foreach (Trait curTrait in curChar.NumericTraits)
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
            SelectedTarget = TargetCharacters_ListBox.SelectedItem as CharacterSheet;
            DrawAttackLine();
        }

        private void DrawAttackLine()
        {
            if (SelectedTarget == null || SelectedTarget.Visual == null)
                return;
            foreach (var cur in _selectedCharacters)
            {
                if (cur.Visual != null)
                {
                    _visualsViewModel.DrawAttack(cur.Visual, SelectedTarget.Visual, cur.Visual.PieceColor, new Duration(new TimeSpan(0, 0, 0, 1)));
                }
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        private void DrawAttackLine_TargetCharacters_ListBox(object sender, MouseButtonEventArgs e)
        {
            DrawAttackLine();
        }
    }
}
