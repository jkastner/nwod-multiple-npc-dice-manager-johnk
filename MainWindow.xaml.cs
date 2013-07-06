using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GameBoard;
using Microsoft.Win32;

namespace XMLCharSheets
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PictureSelectionViewModel _pictureSelectionViewModel = new PictureSelectionViewModel();

        public MainWindow()
        {
            InitializeComponent();
            CombatService.RosterViewModel.RegisterVisualsViewModel(CombatService.VisualsViewModel);
            CombatService.GameBoardVisual.RegisterViewModel(CombatService.VisualsViewModel);
            VisualsService.GameBoardVisual = CombatService.GameBoardVisual;
            CombatService.RosterViewModel.PopulateCharacters(Directory.GetCurrentDirectory() + "\\Sheets");
            DataContext = CombatService.RosterViewModel;
            CombatService.GameBoardVisual.Show();
            CombatService.RosterViewModel.RulesetSelected += RulesetSelectedResponse;
            CombatService.VisualsViewModel.PieceSelected += VisualPieceSelected;
            CombatService.VisualsViewModel.ClearSelectedPieces += ClearSelectedPieces;
            ShapeLength_TextBox.Text = "20";
        }

        private void RulesetSelectedResponse(object sender, EventArgs e)
        {
            var ruleEvent = e as RulesetSelectedEventArgs;
            if (ruleEvent != null)
            {
                CustomCombatPanel.Children.Add(CombatService.RosterViewModel.ControlFor(ruleEvent.SelectedRuleset));
                CombatService.RosterViewModel.LoadDamageFor(ruleEvent.SelectedRuleset);
            }
        }

        private void ClearSelectedPieces(object sender, EventArgs e)
        {
            ActiveCharacters_ListBox.SelectedItems.Clear();
            ActiveCharacters_CreationPage_ListBox.SelectedItems.Clear();
        }

        private void VisualPieceSelected(object sender, EventArgs e)
        {
            var pieceEvent = e as PieceSelectedEventArgs;
            if (pieceEvent != null)
            {
                CharacterSheet matchingChar = CombatService.RosterViewModel.ActiveRoster.Where(x => x.Visual != null &&
                                                                                                    x.Visual.Equals(
                                                                                                        pieceEvent
                                                                                                            .SelectedPiece))
                                                           .FirstOrDefault();
                if (matchingChar != null)
                {
                    ActiveCharacters_ListBox.SelectedItems.Add(matchingChar);
                    ActiveCharacters_CreationPage_ListBox.SelectedItems.Add(matchingChar);
                }
            }
        }


        private void AddCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CombatService.RosterViewModel.SelectedFullCharacter == null)
            {
                CreateCharacterError_Label.Content = "Please select the character to spawn an instance of.";
                return;
            }
            String newName = NewCharacterName_TextBox.Text.Trim();
            if (String.IsNullOrWhiteSpace(newName))
            {
                CreateCharacterError_Label.Content = "Please enter a name.";
                return;
            }
            CreateCharacterError_Label.Content = "";
            int count = 1;
            String originalName = newName;
            while (CombatService.RosterViewModel.ActiveRoster.Any(x => x.Name.Equals(newName)))
            {
                newName = originalName + "_" + count;
                count ++;
            }
            CharacterSheet newInstance = CombatService.RosterViewModel.SelectedFullCharacter.Copy(newName);
            newInstance.Ruleset = CombatService.RosterViewModel.SelectedFullCharacter.Ruleset;
            CombatService.RosterViewModel.RegisterNewCharacter(newInstance);
        }

        private void ActiveCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupTraits(ActiveCharacters_ListBox);
            if (ActiveCharacters_ListBox.SelectedItems.Count > 0)
                CombatService.RosterViewModel.SelectedActiveCharacter =
                    ActiveCharacters_ListBox.SelectedItems[ActiveCharacters_ListBox.SelectedItems.Count - 1] as
                    CharacterSheet;
            else
                CombatService.RosterViewModel.SelectedActiveCharacter = null;
            CombatService.RosterViewModel.SetVisualActive(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Roll_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveList().Count == 0 || CurrentTraits_ListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an active character and at least one trait.");
                return;
            }
            CombatService.RosterViewModel.RollCharacters(ActiveList(), CurrentTraits_ListBox.SelectedItems);
        }

        private void Reset_Health_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            CombatService.RosterViewModel.ResetHealth(ActiveList());
        }

        private void Initiative_Button_Click(object sender, RoutedEventArgs e)
        {
            CombatService.RosterViewModel.RollInitiative();
            CombatService.RosterViewModel.NewRound();
        }

        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            var regex = new Regex("[^0-9-]"); //regex that matches disallowed text
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
            if (e.DataObject.GetDataPresent(typeof (String)))
            {
                var text = (String) e.DataObject.GetData(typeof (String));
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

        private void SelectTarget_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            if (ActiveList().Count == CombatService.RosterViewModel.ActiveRoster.Count())
            {
                MessageBox.Show(
                    "Please select an active character. Some characters must remain unselcted to provide targets.");
                return;
            }
            var st = new SelectTarget(ActiveList(), CombatService.RosterViewModel.ActiveRoster,
                                      CombatService.RosterViewModel.DamageTypes, CombatService.VisualsViewModel);
            st.ShowDialog();
            if (!st.WasCancel && st.SelectedTarget != null)
            {
                CombatService.RosterViewModel.SetTargets(ActiveList(),
                                                         st.Other_Traits_ListBox.SelectedItems, st.SelectedTarget,
                                                         st.ChosenAttack, st.OtherAttacks, st.WoundType);
            }
        }

        private void Attack_Target_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            CombatService.RosterViewModel.RollAttackTarget(ActiveList());
        }

        internal bool CheckValidActive()
        {
            if (CombatService.RosterViewModel.SelectedActiveCharacter == null &&
                CombatService.RosterViewModel.SelectedDeceasedCharacter == null)
            {
                MessageBox.Show("Please select an active character.");
                return false;
            }
            return true;
        }

        private void Results_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Results_TextBox.ScrollToEnd();
        }

        private void DataChanged_DataGrid(object sender, EventArgs e)
        {
            CombatService.RosterViewModel.RecalculateCombatStats(ActiveList());
        }

        private void Make_Status_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValidActive())
            {
                var se = new StatusEffectWindow();
                se.ShowDialog();
                if (!se.WasCancel && !String.IsNullOrWhiteSpace(se.StatusDescription.Text))
                {
                    int duration = Int32.Parse(se.StatusDuration.Text);
                    String description = se.StatusDescription.Text;
                    CombatService.RosterViewModel.AssignStatus(ActiveList(), duration, description);
                }
            }
        }


        private void CleanDeceasedCharacters_Button_Click(object sender, RoutedEventArgs e)
        {
            CombatService.RosterViewModel.MarkCharactersAsDeceased();
        }

        private void DeceasedCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupTraits(DeceasedCharacters);
        }

        private void SetupTraits(ListBox characterListbox)
        {
            CombatService.RosterViewModel.CurrentTraits.Clear();
            if (characterListbox.SelectedItems.Count == 0)
            {
                return;
            }
            var curTraits = new List<String>();
            foreach (object cur in characterListbox.SelectedItems)
            {
                var curChar = cur as CharacterSheet;
                foreach (Trait curTrait in curChar.Traits)
                {
                    curTraits.Add(curTrait.TraitLabel);
                }
            }
            curTraits = curTraits.Distinct().ToList();
            foreach (string cur in curTraits)
            {
                CombatService.RosterViewModel.CurrentTraits.Add(cur);
            }
        }

        private void RemoveCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            CombatService.RosterViewModel.RemoveActiveCharacters(ActiveList());
        }

        internal IList ActiveList()
        {
            if (ActiveCharacters_ListBox.SelectedItems.Count == 0)
            {
                return DeceasedCharacters.SelectedItems;
            }
            return ActiveCharacters_ListBox.SelectedItems;
        }

        private void AddVisual_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveCharacters_ListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one active character.");
                return;
            }
            String possibleName = (ActiveCharacters_ListBox.SelectedItems[0] as CharacterSheet).Name;
            var _visualWindow = new SelectVisualWindow(possibleName, _pictureSelectionViewModel,
                                                       CombatService.RosterViewModel.Teams);
            _visualWindow.ShowDialog();
            var pictureInfo = _visualWindow.SearchedDisplayItems_ListBox.SelectedItem as PictureFileInfo;
            Color pieceColor = _visualWindow.ChosenColor;
            Team chosenTeam = _visualWindow.ChosenTeam;
            if (!_visualWindow.WasCancel && pictureInfo != null && pieceColor != null)
            {
                CombatService.RosterViewModel.AddVisualToCharacters(ActiveCharacters_ListBox.SelectedItems, pictureInfo,
                                                                    pieceColor, chosenTeam);
            }
        }

        private void RemoveVisual_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveCharacters_ListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one active character.");
                return;
            }
            CombatService.RosterViewModel.RemoveVisuals(ActiveCharacters_ListBox.SelectedItems);
        }

        private void ChangeBackground_Button_Click(object sender, RoutedEventArgs e)
        {
            var o = new OpenFileDialog();
            o.InitialDirectory = Directory.GetCurrentDirectory() + @"\MapPictures\";
            o.Multiselect = false;
            o.ShowDialog();
            if (!String.IsNullOrWhiteSpace(o.FileName))
            {
                var sbd = new SetBoardDimensions(CombatService.VisualsViewModel.BoardHeight,
                                                 CombatService.VisualsViewModel.BoardWidth);
                sbd.ShowDialog();
                if (!sbd.WasCancel && sbd.HasBoardHeight && sbd.HasBoardWidth)
                {
                    CombatService.VisualsViewModel.SetBoardBackground(o.FileName, sbd.BoardHeight, sbd.BoardWidth,
                                                                      sbd.MaintainRatio);
                }
            }
        }

        private void AllCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CombatService.RosterViewModel.SelectedFullCharacter == null)
            {
                return;
            }
            String givenName = NewCharacterName_TextBox.Text.Trim();
            bool matchOrEmpty = String.IsNullOrWhiteSpace(givenName);
            if (!matchOrEmpty)
            {
                matchOrEmpty = CombatService.RosterViewModel.FullRoster.Any(x => x.Name.Equals(givenName));
            }
            if (matchOrEmpty)
            {
                NewCharacterName_TextBox.Text = (CombatService.RosterViewModel.SelectedFullCharacter).Name;
            }
        }

        private void TeamColor_ActiveCharacters_ListBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //ActiveCharacters_ListBox.SelectedItems.Clear();
                Team team = (ActiveCharacters_ListBox.SelectedItem as CharacterSheet).Team;
                if (team != null)
                {
                    ActiveCharacters_ListBox.SelectedItems.Clear();
                    var visuals = new List<MoveablePicture>();
                    foreach (CharacterSheet cur in team.TeamMembers)
                    {
                        CharacterSheet sheet = cur;
                        if (sheet.Visual != null)
                        {
                            visuals.Add(sheet.Visual);
                        }
                        ActiveCharacters_ListBox.SelectedItems.Add(cur);
                    }
                    if (visuals.Count > 0)
                        CombatService.VisualsViewModel.ZoomTo(visuals);
                    e.Handled = true;
                }
            }
        }

        private void Target_ActiveCharacters_ListBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                foreach (object cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if (selectedCharacter.Visual != null && selectedCharacter.Target.Visual != null)
                        CombatService.VisualsViewModel.DrawAttack(selectedCharacter.Visual,
                                                                  selectedCharacter.Target.Visual,
                                                                  selectedCharacter.PieceColor,
                                                                  new Duration(new TimeSpan(0, 0, 0, 0, 800)));
                }
            }
        }


        private void SelectionMode_Button_Checked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.None);
        }

        private void DrawLine_Button_Checked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            CombatService.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Line);
        }

        private void DrawCone_Button_Checked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            CombatService.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Cone);
        }

        private void DrawSphere_Button_Checked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            CombatService.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Sphere);
        }

        private void TapeMeasure_Button_Checked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.TapeMeasurerActive = true;
        }

        private void TapeMeasure_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.TapeMeasurerActive = false;
        }

        private void ZoomToVisual_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //CombatService.VisualsViewModel.ZoomTo(
                foreach (object cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if (selectedCharacter.Visual != null)
                        CombatService.VisualsViewModel.ZoomTo(selectedCharacter.Visual.Location);
                }
            }
        }

        private void RestoreDeceased_Button_Click(object sender, RoutedEventArgs e)
        {
            CombatService.RosterViewModel.MoveDeceasedToActive();
        }

        private void GridIsChecked_ToggleButton(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.DrawGrid(5);
        }

        private void GridIsUnchecked_ToggleButton(object sender, RoutedEventArgs e)
        {
            CombatService.VisualsViewModel.RemoveGrid();
        }

        private void OpenFile_Click_MenuItem(object sender, RoutedEventArgs e)
        {
            FileSaveOpenService.OpenFile();
        }

        private void SaveFile_Click_MenuItem(object sender, RoutedEventArgs e)
        {
            FileSaveOpenService.SaveFile();
        }

        private void SaveAsFile_Click_MenuItem(object sender, RoutedEventArgs e)
        {
            FileSaveOpenService.SaveFileAs();
        }

        private void Exit_Click_MenuItem(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}