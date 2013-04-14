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
using System.Text.RegularExpressions;
using System.Collections;
using GameBoard;
using Microsoft.Win32;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RosterViewModel _viewModel;
        GameBoardVisual _gameBoardVisual;
        public VisualsViewmodel _visualsViewmodel = new VisualsViewmodel();
        PictureSelectionViewModel _pictureSelectionViewModel = new PictureSelectionViewModel();
        public MainWindow()
        {
            _viewModel = new RosterViewModel(_visualsViewmodel);
            InitializeComponent();
            _gameBoardVisual = new GameBoardVisual(_visualsViewmodel);
            _viewModel.PopulateCharacters(Directory.GetCurrentDirectory()+"\\Sheets");
            this.DataContext = _viewModel;
            _viewModel.DamageTypes.Add("Bashing");
            _viewModel.DamageTypes.Add("Lethal");
            _viewModel.DamageTypes.Add("Aggrivated");
            _gameBoardVisual.Show();
            _visualsViewmodel.PieceSelected += VisualPieceSelected;
            _visualsViewmodel.ClearSelectedPieces += ClearSelectedPieces;
            ShapeLength_TextBox.Text = "20";
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
                var matchingChar = _viewModel.ActiveRoster.Where(x => x.Visual != null &&
                            x.Visual.Equals(pieceEvent.SelectedPiece)).FirstOrDefault();
                if (matchingChar != null)
                {
                    ActiveCharacters_ListBox.SelectedItems.Add(matchingChar);
                    ActiveCharacters_CreationPage_ListBox.SelectedItems.Add(matchingChar);
                }
            }
        }


        private bool ruleSetChosen = false;
        private void AddCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedFullCharacter == null)
            {
                CreateCharacterError_Label.Content = "Please select the character to spawn an instance of.";
                return;
            }
            String newName = NewCharacterName_TextBox.Text.Trim();
            if(String.IsNullOrWhiteSpace(newName))
            {
                CreateCharacterError_Label.Content = "Please enter a name.";
                return;
            }
            CreateCharacterError_Label.Content = "";
            int count = 1;
            String originalName = newName;
            while (_viewModel.ActiveRoster.Any(x => x.Name.Equals(newName)))
            {
                newName =  originalName + "_"+count;
                count ++;
            }
            CharacterSheet newInstance = _viewModel.SelectedFullCharacter.Copy(newName);
            newInstance.Ruleset = _viewModel.SelectedFullCharacter.Ruleset;
            _viewModel.ActiveRoster.Add(newInstance);
            if (!ruleSetChosen)
            {
                _viewModel.SetMode(newInstance.Ruleset);
                CustomCombatPanel.Children.Add(_viewModel.ControlFor(newInstance.Ruleset));
                ruleSetChosen = true;
            }
        }

        private void ActiveCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupTraits(ActiveCharacters_ListBox);
            if (ActiveCharacters_ListBox.SelectedItems.Count > 0)
                _viewModel.SelectedActiveCharacter = ActiveCharacters_ListBox.SelectedItems[ActiveCharacters_ListBox.SelectedItems.Count - 1] as CharacterSheet;
            else
                _viewModel.SelectedActiveCharacter = null;
            _viewModel.SetVisualActive(ActiveCharacters_ListBox.SelectedItems);
        }

        private void Roll_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveList().Count == 0 || CurrentTraits_ListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an active character and at least one trait.");
                return;
            }
            _viewModel.RollCharacters(ActiveList(), CurrentTraits_ListBox.SelectedItems);
        }

        private void Do_Bashing_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.DoBashing(ActiveList());
        }

        private void Do_Lethal_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.DoLethal(ActiveList());
        }

        private void Do_Aggrivated_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.DoAggrivated(ActiveList());
        }

        private void Reset_Health_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.ResetHealth(ActiveList());
        }

        private void Initiative_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollInitiative();
            _viewModel.NewRound();
        }

        private void RemoveCharacter_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.RemoveActiveCharacters(ActiveList());
        }

        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            Regex regex = new Regex("[^0-9-]"); //regex that matches disallowed text
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
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
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
            if (ActiveList().Count == _viewModel.ActiveRoster.Count())
            {
                MessageBox.Show("Please select an active character. Some characters must remain unselcted to provide targets.");
                return;
            }
            SelectTarget st = new SelectTarget(ActiveList(), _viewModel.ActiveRoster, _viewModel.DamageTypes, _visualsViewmodel);
            st.ShowDialog();
            if (!st.WasCancel&&st.SelectedTarget != null)
            {
                _viewModel.SetTargets(ActiveList(), 
                    st.Other_Traits_ListBox.SelectedItems, st.SelectedTarget, st.ChosenAttack, st.WoundType);
            }
        }

        private void Attack_Target_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValidActive())
                return;
            _viewModel.RollAttackTarget(ActiveList());

        }

        private bool CheckValidActive()
        {
            if (_viewModel.SelectedActiveCharacter == null && _viewModel.SelectedDeceasedCharacter==null)
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
            _viewModel.RecalculateCombatStats(ActiveList());
        }

        private void Make_Status_Button_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValidActive())
            {
                StatusEffectWindow se = new StatusEffectWindow();
                se.ShowDialog();
                if (!se.WasCancel&&!String.IsNullOrWhiteSpace(se.StatusDescription.Text))
                {
                    int duration = Int32.Parse(se.StatusDuration.Text);
                    String description = se.StatusDescription.Text;
                    _viewModel.AssignStatus(ActiveList(), duration, description);
                }
            }
        }

        private void Blood_Heal_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BloodHeal(ActiveList());
        }

        private void Blood_Buff_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BloodBuff(ActiveList());
        }

        private void Refill_Vitae_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RefillVitae(ActiveList());
        }

        private void CleanDeceasedCharacters_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MarkCharactersAsDeceased();
        }

        private void DeceasedCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupTraits(DeceasedCharacters);
        }

        private void SetupTraits(ListBox characterListbox)
        {
            _viewModel.CurrentTraits.Clear();
            if (characterListbox.SelectedItems.Count == 0)
            {
                return;
            }
            List<String> curTraits = new List<String>();
            foreach (var cur in characterListbox.SelectedItems)
            {
                CharacterSheet curChar = cur as CharacterSheet;
                foreach (var curTrait in curChar.Traits)
                {
                    curTraits.Add(curTrait.TraitLabel);
                }
            }
            curTraits = curTraits.Distinct().ToList();
            foreach (var cur in curTraits)
            {
                _viewModel.CurrentTraits.Add(cur);
            }
        }


        private IList ActiveList()
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
            SelectVisualWindow _visualWindow = new SelectVisualWindow(possibleName, _pictureSelectionViewModel, _viewModel.Teams);
            _visualWindow.ShowDialog();
            var pictureInfo = _visualWindow.SearchedDisplayItems_ListBox.SelectedItem as PictureFileInfo;
            Color pieceColor = _visualWindow.ChosenColor;
            Team chosenTeam = _visualWindow.ChosenTeam;
            if (!_visualWindow.WasCancel && pictureInfo != null && pieceColor!=null)
            {
                _viewModel.AddVisualToCharacters(ActiveCharacters_ListBox.SelectedItems, pictureInfo, pieceColor, chosenTeam);
            }
        }

        private void RemoveVisual_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveCharacters_ListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one active character.");
                return;
            }
            _viewModel.RemoveVisuals(ActiveCharacters_ListBox.SelectedItems);
        }

        private void ChangeBackground_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.InitialDirectory = Directory.GetCurrentDirectory() + @"\MapPictures\";
            o.Multiselect = false;
            o.ShowDialog();
            if (!String.IsNullOrWhiteSpace(o.FileName))
            {
                SetBoardDimensions sbd = new SetBoardDimensions(_visualsViewmodel.BoardHeight, _visualsViewmodel.BoardWidth);
                sbd.ShowDialog();
                if (!sbd.WasCancel && sbd.HasBoardHeight && sbd.HasBoardWidth)
                {
                    _visualsViewmodel.SetBoardBackground(o.FileName, sbd.BoardHeight, sbd.BoardWidth, sbd.MaintainRatio);

                }
            }
        }

        private void AllCharacters_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedFullCharacter == null)
            {
                return;
            }
            String givenName = NewCharacterName_TextBox.Text.Trim();
            bool matchOrEmpty = String.IsNullOrWhiteSpace(givenName);
            if (!matchOrEmpty)
            {
                matchOrEmpty = _viewModel.FullRoster.Any(x => x.Name.Equals(givenName));
            }
            if (matchOrEmpty)
            {
                NewCharacterName_TextBox.Text = (_viewModel.SelectedFullCharacter as CharacterSheet).Name;
            }
        }

        private void TeamColor_ActiveCharacters_ListBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //ActiveCharacters_ListBox.SelectedItems.Clear();
                var team = (ActiveCharacters_ListBox.SelectedItem as CharacterSheet).Team;
                if(team!=null)
                {
                    ActiveCharacters_ListBox.SelectedItems.Clear();
                    List<MoveablePicture> visuals = new List<MoveablePicture>();
                    foreach (var cur in team.TeamMembers)
                    {
                        var sheet = cur as CharacterSheet;
                        if (sheet.Visual != null)
                        {
                            visuals.Add(sheet.Visual);
                        }
                        ActiveCharacters_ListBox.SelectedItems.Add(cur);
                    }
                    if(visuals.Count>0)
                        _visualsViewmodel.ZoomTo(visuals);
                    e.Handled = true;
                }
            }
        }

        private void Target_ActiveCharacters_ListBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                foreach (var cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if(selectedCharacter.Visual!=null&&selectedCharacter.Target.Visual!=null)
                        _visualsViewmodel.DrawAttack(selectedCharacter.Visual, selectedCharacter.Target.Visual, selectedCharacter.PieceColor, new Duration(new TimeSpan(0,0,0,0,800)));
                }
            }
        }


        private void SelectionMode_Button_Checked(object sender, RoutedEventArgs e)
        {
            _visualsViewmodel.SetShapeMode(GameBoard.VisualsViewmodel.ShapeMode.None);
        }

        private void DrawLine_Button_Checked(object sender, RoutedEventArgs e)
        {
            _visualsViewmodel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            _visualsViewmodel.SetShapeMode(GameBoard.VisualsViewmodel.ShapeMode.Line);
        }

        private void DrawCone_Button_Checked(object sender, RoutedEventArgs e)
        {
            _visualsViewmodel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            _visualsViewmodel.SetShapeMode(GameBoard.VisualsViewmodel.ShapeMode.Cone);
        }

        private void DrawSphere_Button_Checked(object sender, RoutedEventArgs e)
        {
            _visualsViewmodel.ShapeSize = double.Parse(ShapeLength_TextBox.Text);
            _visualsViewmodel.SetShapeMode(GameBoard.VisualsViewmodel.ShapeMode.Sphere);
        }

        private void ZoomToVisual_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //_visualsViewmodel.ZoomTo(
                foreach (var cur in ActiveCharacters_ListBox.SelectedItems)
                {
                    var selectedCharacter = cur as CharacterSheet;

                    if (selectedCharacter.Visual != null)
                        _visualsViewmodel.ZoomTo(selectedCharacter.Visual.Location);
                }
            }
        }

        private void RestoreDeceased_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MoveDeceasedToActive();
        }

    }
}





