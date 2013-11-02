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
using System.Windows.Documents;

namespace XMLCharSheets
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PictureSelectionViewModel _pictureSelectionViewModel = new PictureSelectionViewModel();
        private Paragraph RichTextParagraph;
        List<GameBoardVisual_Window> _gameBoardWindows = new List<GameBoardVisual_Window>();
        private Board _targetBoard;
        public MainWindow()
        {
            InitializeComponent();
            CombatService.RosterViewModel.PopulateCharacters(Directory.GetCurrentDirectory() + "\\Sheets");
            CombatService.RosterViewModel.ShowErrors();

            VisualsService.BoardsViewModel.BoardRegistered += OnBoardRegistered;
            VisualsService.BoardsViewModel.BoardDeregistered += OnBoardDeregistered;

            var boardForMainTab = VisualsService.BoardsViewModel.CreateAndRegisterNewBoard(BoardsViewModel.MainBoardName);
            CombatService.RosterViewModel.MainCamera = boardForMainTab.GameBoardVisual.Camera;
            var boardForWindow = VisualsService.BoardsViewModel.CreateAndRegisterNewBoard(BoardsViewModel.WindowBoardName);
            var boardForVisualTab = VisualsService.BoardsViewModel.CreateAndRegisterNewBoard(BoardsViewModel.VisualTabBoardName);
            _targetBoard = VisualsService.BoardsViewModel.CreateAndRegisterNewBoard(BoardsViewModel.TargetBoardName);

            
            VisualControl_BoardSpace_DockPanel.Children.Add(boardForVisualTab.GameBoardVisual);
            MainBoard_DockPanel.Children.Add(boardForMainTab.GameBoardVisual);

            DataContext = CombatService.RosterViewModel;
            GameBoardVisual_Window boardVisualWindow = new GameBoardVisual_Window(boardForWindow);
            _gameBoardWindows.Add(boardVisualWindow);
            boardVisualWindow.Show();

            CombatService.RosterViewModel.RulesetSelected += RulesetSelectedResponse;

            CombatService.RosterViewModel.ReportTextEvent += UpdateResultsRichTextBox;
            CombatService.RosterViewModel.ClearReportTextEvent += ClearResultsRichTextBox;
            this.RichTextParagraph = new Paragraph();
            Results_RichTextBox.Document = new FlowDocument(RichTextParagraph);
            ShapeLength_TextBox.Text = "20";
        }

        private void OnBoardDeregistered(object sender, EventArgs e)
        {
            var boardEvent = e as BoardRegisteredEventArgs;
            boardEvent.NewBoard.VisualsViewModel.PieceSelected -= VisualPieceSelected;
            boardEvent.NewBoard.VisualsViewModel.ClearSelectedPieces -= ClearSelectedPieces;
            boardEvent.NewBoard.VisualsViewModel.PieceMoved -= OnVisualPieceMoved;
        }

        private void OnBoardRegistered(object sender, EventArgs e)
        {
            var boardEvent = e as BoardRegisteredEventArgs;
            boardEvent.NewBoard.VisualsViewModel.PieceSelected += VisualPieceSelected;
            boardEvent.NewBoard.VisualsViewModel.ClearSelectedPieces += ClearSelectedPieces;
            boardEvent.NewBoard.VisualsViewModel.PieceMoved += OnVisualPieceMoved;
        }

        private void OnVisualPieceMoved(object sender, EventArgs e)
        {
            var pieceEvent = e as PieceMovedEventsArg;
            if (pieceEvent != null)
            {
                if (pieceEvent.MoverID != null)
                {
                    CharacterSheet matchingChar = CombatService.RosterViewModel.ActiveRoster.
                        FirstOrDefault(x => x.UniqueCharacterID == pieceEvent.MoverID);
                    if (matchingChar != null)
                    {
                        matchingChar.HasMoved = true;
                    }
                }
            }
        }


        private void ClearResultsRichTextBox(object sender, EventArgs e)
        {
            RichTextParagraph.Inlines.Clear();
        }

        private void UpdateResultsRichTextBox(object sender, EventArgs e)
        {
            var textEvent = e as ReportTextEventArgs;
            RichTextParagraph.Inlines.Add(new Bold(new Run(textEvent.Message))
            {
                Foreground = textEvent.DisplayColor,
                FontSize = textEvent.FontSize,
            });
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
        }

        private void VisualPieceSelected(object sender, EventArgs e)
        {
            var pieceEvent = e as PieceSelectedEventArgs;
            if (pieceEvent != null)
            {
                CharacterSheet matchingChar = CombatService.RosterViewModel.ActiveRoster.FirstOrDefault(x => x.UniqueCharacterID == pieceEvent.SelectedPieceID);
                                                                           
                if (matchingChar != null)
                {
                    ActivateCharacter(matchingChar, false);
                }
            }
        }

        private void ActivateCharacter(CharacterSheet matchingChar, bool zoomToCharacter)
        {
            ActiveCharacters_ListBox.SelectedItems.Add(matchingChar);
            ActiveCharacters_ListBox.ScrollIntoView(matchingChar);
            if(zoomToCharacter)
                BoardsZoomTo(new List<Guid>() { matchingChar.UniqueCharacterID });
        }


        private void ActiveCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SetupTraits(ActiveCharacters_ListBox);
            if (ActiveCharacters_ListBox.SelectedItems.Count > 0)
                CombatService.RosterViewModel.SelectedActiveCharacter =
                    ActiveCharacters_ListBox.SelectedItems[ActiveCharacters_ListBox.SelectedItems.Count - 1] as
                    CharacterSheet;
            else
                CombatService.RosterViewModel.SelectedActiveCharacter = null;
            CombatService.RosterViewModel.ResetToActive(e.AddedItems, e.RemovedItems);
        }

        private void Roll_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (ActiveList().Count == 0 || CurrentTraits_ListBox.SelectedItems.Count == 0)
            //{
            //    MessageBox.Show("Please select an active character and at least one trait.");
            //    return;
            //}
            //CombatService.RosterViewModel.RollCharacters(ActiveList(), CurrentTraits_ListBox.SelectedItems);
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
            CombatService.RosterViewModel.CurrentRound++;
            CurrentRound_Label.Content = "Round " + CombatService.RosterViewModel.CurrentRound;
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

        private void SelectedTargetButton_Click(object sender, RoutedEventArgs e)
        {
            NewSelectTarget t = new NewSelectTarget(_targetBoard, ActiveCharacters_ListBox.SelectedItems);
            t.ShowDialog();

            if (!t.WasCancel && t.SelectedTarget != null)
            {
                CombatService.RosterViewModel.SetTargets(
                    attackers: t.Attackers.ToList(),
                    otherAttacks: t.OtherTraits.ToList(), 
                    target: t.SelectedTarget,
                    attackType: t.MainAttack, 
                    damageType: t.DamageType);
            }
        }
        private void Attack_Target_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            CombatService.RosterViewModel.RollAttackTarget(ActiveList());
        }


        private void Auto_Act_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            CombatService.RosterViewModel.PerformAutomaticActions(ActiveList());
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
            Results_RichTextBox.ScrollToEnd();
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
            CombatService.RosterViewModel.RemoveCharactersFromRosters(ActiveList());
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
                CombatService.RosterViewModel.AddVisualToCharacters(ActiveCharacters_ListBox.SelectedItems, pictureInfo, chosenTeam);
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
                var sbd = new SetBoardDimensions(BoardsViewModel.Instance.Boards.First().VisualsViewModel.BoardHeight,
                                                 BoardsViewModel.Instance.Boards.First().VisualsViewModel.BoardWidth);
                sbd.ShowDialog();
                if (!sbd.WasCancel && sbd.HasBoardHeight && sbd.HasBoardWidth)
                {
                    string targetFile = o.FileName.Replace(Directory.GetCurrentDirectory()+Path.DirectorySeparatorChar, "");

                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.SetBoardBackground(targetFile, sbd.BoardHeight, sbd.BoardWidth,
                                                                      sbd.MaintainRatio));
                }
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
                    var visuals = new List<Guid>();
                    foreach (CharacterSheet cur in team.TeamMembers)
                    {
                        CharacterSheet sheet = cur;
                        if (sheet.HasVisual)
                        {
                            visuals.Add(sheet.UniqueCharacterID);
                        }
                        ActiveCharacters_ListBox.SelectedItems.Add(cur);
                    }
                    if (visuals.Count > 0)
                    {
                        BoardsZoomTo(visuals);
                        VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.ZoomTo(visuals));
                    }
                    e.Handled = true;
                }
            }
        }

        private void BoardsZoomTo(List<Guid> visuals)
        {
            VisualsService.BoardsViewModel.ZoomTo(visuals, CombatService.RosterViewModel.OrientAllCamerasToMatchMain);
        }

        private void Target_ActiveCharacters_ListBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                foreach (object cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if (selectedCharacter.HasVisual && selectedCharacter.Target !=null 
                        && selectedCharacter.Target.HasVisual)
                        VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.DrawAttack(selectedCharacter.UniqueCharacterID,
                                                                  selectedCharacter.Target.UniqueCharacterID,
                                                                  selectedCharacter.Team.TeamColor,
                                                                  new Duration(new TimeSpan(0, 0, 0, 0, 800))));
                }
            }
        }


        private void SelectionMode_Button_Checked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.None));
        }

        private void DrawLine_Button_Checked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text));
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Line));
        }

        private void DrawCone_Button_Checked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text));
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Cone));
        }

        private void DrawSphere_Button_Checked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.ShapeSize = double.Parse(ShapeLength_TextBox.Text));
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetShapeMode(VisualsViewModel.ShapeMode.Sphere));
        }

        private void TapeMeasure_Button_Checked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.TapeMeasurerActive = true);
        }

        private void TapeMeasure_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.TapeMeasurerActive = false);
        }

        private void ZoomToVisual_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                foreach (object cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if (selectedCharacter.HasVisual)
                    {
                        BoardsZoomTo(new List<Guid>(){ selectedCharacter.UniqueCharacterID});
                    }
                }
            }
        }

        private void RestoreDeceased_Button_Click(object sender, RoutedEventArgs e)
        {
            CombatService.RosterViewModel.MoveDeceasedToActive();
        }

        private void GridIsChecked_ToggleButton(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.DrawGrid(5));
        }

        private void GridIsUnchecked_ToggleButton(object sender, RoutedEventArgs e)
        {
            VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.RemoveGrid());
        }

        private void OpenFile_Click_MenuItem(object sender, RoutedEventArgs e)
        {
            var readBoards = FileSaveOpenService.OpenFile();
            if (readBoards != null && readBoards.Count > 0)
            {
                foreach (var cur in _gameBoardWindows)
                {
                    cur.Close();
                }
                _gameBoardWindows.Clear();
                BoardsViewModel.Instance.ClearAllBoards();
                for (int curIndex = 0; curIndex < readBoards.Count; curIndex++)
                {
                    var curBoard = readBoards[curIndex];
                    BoardsViewModel.Instance.RegisterBoard(curBoard);
                    switch (curBoard.BoardName)
                    {
                        case BoardsViewModel.WindowBoardName:
                            {
                                GameBoardVisual_Window boardVisualWindow = new GameBoardVisual_Window(curBoard);
                                _gameBoardWindows.Add(boardVisualWindow);
                                boardVisualWindow.Show();
                            }
                            break;
                        case BoardsViewModel.MainBoardName:
                            {
                                MainBoard_DockPanel.Children.Clear();
                                CombatService.RosterViewModel.MainCamera = curBoard.GameBoardVisual.Camera;
                                MainBoard_DockPanel.Children.Add(curBoard.GameBoardVisual);
                            }
                            break;
                        case BoardsViewModel.VisualTabBoardName:
                            {
                                VisualControl_BoardSpace_DockPanel.Children.Clear();
                                VisualControl_BoardSpace_DockPanel.Children.Add(curBoard.GameBoardVisual);
                            }
                            break;
                        case BoardsViewModel.TargetBoardName:
                            {
                                _targetBoard = curBoard;
                                _targetBoard.GameBoardVisual.IsEnabled = false;
                            }
                            break;
                    }
                }
            }
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

        private void Auto_Run_Combat(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Run entire combat until only one team remains?", "Confirm Auto Combat Mode", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                
            }
            else
            {
                var teams = CombatService.RosterViewModel.ActiveRoster.Select(x=>x.Team).Distinct();
                if (teams.Count() <= 1)
                {
                    MessageBox.Show("Two teams must exist.");
                    return;
                }
                else
                {
                    while (teams.Count() != 1)
                    {
                        Initiative_Button_Click(this, null);
                        ActiveCharacters_ListBox.SelectAll();
                        Auto_Act_Button_Click(this, null);
                    }
                }
            }
        }

        private void OrientCamera(object sender, RoutedEventArgs e)
        {
            
        }


        private void ActivateNext_Button_Click(object sender, RoutedEventArgs e)
        {
            ActivateNextValid();
        }

        private void ActivateNextValid()
        {
            var firstChar = CombatService.RosterViewModel.ActiveRoster.Where(x =>
                !x.HasMoved &&
                !x.HasAttacked &&
                !x.IsIncapacitated).OrderByDescending(x => x.CurInitiative).FirstOrDefault();

            if (firstChar != null && ActiveCharacters_ListBox.Items.Contains(firstChar))
            {
                var curChar = ActiveCharacters_ListBox.SelectedItem as CharacterSheet;
                if (curChar == firstChar)
                {
                    curChar.HasAttacked = true;
                    curChar.HasMoved = true;
                }
                ClearSelectedPieces(this, null);
                ActivateCharacter(firstChar, true);
            }
            else
            {
                TextReporter.Report("No valid characters to select.\n");
            }
        }

    }
}

