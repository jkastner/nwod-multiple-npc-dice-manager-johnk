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
        private readonly ObservableCollection<String> _attackTraits = new ObservableCollection<String>();
        private readonly ObservableCollection<String> _otherAttackTraits = new ObservableCollection<String>();
        private readonly ObservableCollection<String> _otherTraits = new ObservableCollection<String>();
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
            OtherAttacks_ListBox.ItemsSource = _otherAttackTraits;
            OtherTraits_ListBox.ItemsSource = _otherTraits;
            RemakeListOfTargets();
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
                        if (!_attackTraits.Contains(attackTrait.TraitLabel))
                        {
                            _attackTraits.Add(attackTrait.TraitLabel);
                            _otherAttackTraits.Add(attackTrait.TraitLabel);
                        }
                    }
                    else
                    {
                        if (!_otherTraits.Contains(curTrait.TraitLabel))
                            _otherTraits.Add(curTrait.TraitLabel);
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
            RemakeListOfTargets();
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
            RemakeListOfTargets();
        }


    }
}
