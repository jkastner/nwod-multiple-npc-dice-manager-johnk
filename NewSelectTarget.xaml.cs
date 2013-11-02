using GameBoard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for NewSelectTarget.xaml
    /// </summary>
    public partial class NewSelectTarget : Window
    {
        ObservableCollection <CharacterSheet> _selectedCharacters = new ObservableCollection<CharacterSheet>();
        ObservableCollection<CharacterSheet> _targetCharacters = new ObservableCollection<CharacterSheet>();
        private readonly ObservableCollection<Trait> _attackTraits = new ObservableCollection<Trait>();
        private readonly ObservableCollection<Trait> _otherAttackTraits = new ObservableCollection<Trait>();
        private readonly ObservableCollection<Trait> _otherTraits = new ObservableCollection<Trait>();
        public NewSelectTarget(Board targetBoard, IList selectedObjects)
        {
            InitializeComponent();
            Closing += RemoveBoard;
            Viewport_DockPanel.Children.Add(targetBoard.GameBoardVisual);
            foreach (object cur in selectedObjects)
            {
                _selectedCharacters.Add(cur as CharacterSheet);
            }
            Attackers_ListBox.ItemsSource = _selectedCharacters;
            PossibleTargets_ListBox.ItemsSource = _targetCharacters;
            MainAttacks_ListBox.ItemsSource = _attackTraits;
            OtherTraits_ListBox.ItemsSource = _otherTraits;
            DamageType_ListBox.ItemsSource = CombatService.RosterViewModel.DamageTypes;
            RemakeListOfTargets();
            SelectDefaults();
        }

        private void SelectDefaults()
        {
            if (MainAttacks_ListBox.Items.Count > 0)
            {
                MainAttacks_ListBox.SelectedIndex = 0;
                var selectedAttack = _attackTraits.First() as AttackTrait;
                var damageIndex = DamageType_ListBox.Items.IndexOf(selectedAttack.DamageType);
                if(damageIndex >= 0)
                {
                    DamageType_ListBox.SelectedIndex = damageIndex;
                }
                if (PossibleTargets_ListBox.Items.Count > 0)
                {
                    PossibleTargets_ListBox.SelectedIndex = 0;
                }
            }
            
            
        }

        public CharacterSheet SelectedTarget
        {
            get
            {
                if (PossibleTargets_ListBox.SelectedItem == null)
                {
                    return null;
                }
                return PossibleTargets_ListBox.SelectedItem as CharacterSheet;
            }
        }
        public IEnumerable<String> OtherTraits
        {
            get
            {
                foreach (var cur in OtherTraits_ListBox.SelectedItems)
                {
                    yield return (cur as Trait).TraitLabel;
                }
                for (int curIndex = 1; curIndex < MainAttacks_ListBox.SelectedItems.Count;curIndex++)
                {
                    var curTrait = MainAttacks_ListBox.SelectedItems[curIndex] as Trait;
                    yield return curTrait.TraitLabel;
                }
            }
        }


        public IEnumerable<CharacterSheet> Attackers
        {
            get
            {
                foreach (var cur in Attackers_ListBox.Items)
                    yield return cur as CharacterSheet;
            }
        }
        public String DamageType
        {
            get
            {
                if (DamageType_ListBox.SelectedItem == null)
                {
                    return null;
                }
                return DamageType_ListBox.SelectedItem.ToString();
            }
        }
        public String MainAttack
        {
            get
            {
                if (MainAttacks_ListBox.SelectedItems.Count ==  0)
                {
                    return null;
                }
                return (MainAttacks_ListBox.SelectedItems[0] as Trait).TraitLabel;
            }
        }



        private void RemakeListOfTargets()
        {
            _attackTraits.Clear();
            _targetCharacters.Clear();
            _otherAttackTraits.Clear();
            _otherTraits.Clear();
            var allTargets = new List<CharacterSheet>();
            foreach (CharacterSheet curChar in CombatService.RosterViewModel.ActiveRoster)
            {
                allTargets.Add(curChar);
            }
            //1. Remove all attackers from list of targets.
            var attackingTeams = new List<Team>();
            foreach (CharacterSheet curChar in _selectedCharacters)
            {
                if (!attackingTeams.Contains(curChar.Team))
                {
                    attackingTeams.Add(curChar.Team);
                }
                allTargets.Remove(curChar);
                //2. Build list of possible attack traits.
                foreach (NumericIntTrait curTrait in curChar.NumericTraits)
                {
                    var attackTrait = curTrait as AttackTrait;
                    if (attackTrait != null)
                    {
                        if (!_attackTraits.Any(x=>x.TraitLabel.Equals(attackTrait.TraitLabel)))
                        {
                            _attackTraits.Add(attackTrait);
                            _otherAttackTraits.Add(attackTrait);
                        }
                    }
                    else
                    {
                        if (!_otherTraits.Any(x=>x.TraitLabel.Equals(curTrait)))
                            _otherTraits.Add(curTrait);
                    }
                }
            }
            //3. Put targets that share a team with at least on attacker in a separate list.
            IEnumerable<CharacterSheet> teammateTargets;
            teammateTargets = from element in allTargets where attackingTeams.Contains(element.Team) select element;
            //teammateTargets.Select(targetCharacters.Where(x => attackingTeams.Contains(x.Team)));
            IEnumerable<CharacterSheet> nonTeammateTargets = allTargets.Except(teammateTargets);
            //4. Sort the remaining targets by distance to a common 'origin' of the group.
            Point3D commonOrigin =
                Helper3DCalcs.FindMidpoint(_selectedCharacters.Where(x => x.HasVisual)
                                                             .Select(x => x.FirstVisual.Location));
            nonTeammateTargets = nonTeammateTargets.OrderBy(x => x.DistanceTo(commonOrigin));
            teammateTargets = teammateTargets.OrderBy(x => x.DistanceTo(commonOrigin));
            foreach (CharacterSheet cur in nonTeammateTargets)
            {
                _targetCharacters.Add(cur);
            }
            foreach (CharacterSheet cur in teammateTargets)
            {
                _targetCharacters.Add(cur);
            }
        }

        private void RemoveBoard(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Viewport_DockPanel.Children.Clear();
        }

        private void MoveToTarget_Button_Click(object sender, RoutedEventArgs e)
        {
            List<CharacterSheet> _selected = new List<CharacterSheet>();
            foreach(var cur in Attackers_ListBox.SelectedItems)
            {
                _selected.Add(cur as CharacterSheet);
            }
            foreach (var cur in _selected)
            {
                _selectedCharacters.Remove(cur);
            }
            SetAttackersAsActive(); 
            RemakeListOfTargets();
            SelectDefaults();
        }

        private void SetAttackersAsActive()
        {
            CombatService.RosterViewModel.ResetToActive(Attackers_ListBox.Items, new List<CharacterSheet>());
        }

        private void MoveToAttacker_Button_Click(object sender, RoutedEventArgs e)
        {
            List<CharacterSheet> _selected = new List<CharacterSheet>();
            foreach (var cur in PossibleTargets_ListBox.SelectedItems)
            {
                _selected.Add(cur as CharacterSheet);
            }
            foreach (var cur in _selected)
            {
                _selectedCharacters.Add(cur);
            }
            SetAttackersAsActive(); 
            RemakeListOfTargets();
            SelectDefaults();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = true;
            this.Close();
        }



        public bool WasCancel { get; set; }

        private void Characters_ListBox_Selected(object sender, MouseButtonEventArgs e)
        {
            var curSelected = (sender as ListBoxItem).Content as CharacterSheet;
            if (curSelected != null)
            {
                if (curSelected.FirstVisual != null)
                {
                    VisualsService.BoardsViewModel.ZoomTo(
                        new List<Guid>() { curSelected.UniqueCharacterID },
                        CombatService.RosterViewModel.OrientAllCamerasToMatchMain);
                }
            }
            foreach (var curA in Attackers_ListBox.Items)
            {
                var curAttacker = curA as CharacterSheet;
                if(curAttacker.HasVisual &&
                      SelectedTarget!=null &&
                        SelectedTarget.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.DrawAttack(
                        curAttacker.UniqueCharacterID,
                        SelectedTarget.UniqueCharacterID,
                        Colors.Red,
                        new Duration(new TimeSpan(0, 0, 0, 1))));
                }
            }
        }

        private void SelectTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OK_Button_Click(sender, e);
            }
            if (e.Key == Key.Escape)
            {
                Cancel_Click(sender, e);
            }
        }

        private void MainAttacks_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var curAttack = e.AddedItems[0] as AttackTrait;
                if (curAttack != null)
                {
                    if(DamageType_ListBox.Items.Contains(curAttack.DamageType))
                    {
                        DamageType_ListBox.SelectedItem = curAttack.DamageType;
                    }
                }
            }
        }





    }
}
