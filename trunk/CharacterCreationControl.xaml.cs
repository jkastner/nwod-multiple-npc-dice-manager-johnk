using GameBoard;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for CharacterCreationControl.xaml
    /// </summary>
    public partial class CharacterCreationControl : UserControl
    {
        CharacterCreationViewModel _characterCreationViewModel = new CharacterCreationViewModel();
        private readonly PictureSelectionViewModel _pictureSelectionViewModel = PictureSelectionViewModel.Instance;
        public CharacterCreationControl()
        {
            InitializeComponent();
            DataContext = _characterCreationViewModel;
            AvailableNPCS_ListBox.ItemsSource = _characterCreationViewModel.FilteredCharacters;
            PictureSearch_ListBox.ItemsSource = _pictureSelectionViewModel.ActiveLoadedPictures;
            TeamSelection_ListBox.ItemsSource = CombatService.RosterViewModel.Teams;
            
            /// Take largely from 
            /// http://www.intertech.com/Blog/post/How-to-Select-All-Text-in-a-WPF-Content-on-Focus.aspx
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText), true);
            ///

        }





        /// <summary>
        /// Take largely from 
        /// http://www.intertech.com/Blog/post/How-to-Select-All-Text-in-a-WPF-Content-on-Focus.aspx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var textbox = (sender as TextBox);
            if (textbox != null && !textbox.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }

        /// <summary>
        /// Take largely from 
        /// http://www.intertech.com/Blog/post/How-to-Select-All-Text-in-a-WPF-Content-on-Focus.aspx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }

        private void CharacterSheetSearcher_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunCharacterSheetSearch();
        }

        private void RunCharacterSheetSearch()
        {
            var searchText = CharacterSheetSearcher_TextBox.Text.Trim().ToLower();
            if(searchText.Contains("search..."))
                return;
            ImageSearch_TextBox.Text = CharacterSheetSearcher_TextBox.Text;
            _characterCreationViewModel.SearchForText(searchText);
            if (_characterCreationViewModel.FilteredCharacters.Count >= 1)
            {
                AvailableNPCS_ListBox.SelectedIndex = 0;
            }

        }

        private void ImageSearch_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RunImageSearch();
        }

        private void RunImageSearch()
        {
            var searchText = ImageSearch_TextBox.Text.Trim().ToLower();
            if (searchText.Contains("search..."))
            {
                return;
            }
            _pictureSelectionViewModel.AdjustList(searchText);
            if (PictureSearch_ListBox.Items.Count == 1)
            {
                PictureSearch_ListBox.SelectedIndex = 0;
            }
        }

        private void CharacterCreationControl_Loaded(object sender, RoutedEventArgs e)
        {
            _characterCreationViewModel.ResetActiveList();
            RunCharacterSheetSearch();
            RunImageSearch();
        }

        private void CharacterName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (
                !CharacterName_TextBox.Text.Equals("Name")
                &&
                _characterCreationViewModel.IsDefaultName(ImageSearch_TextBox.Text))
            {
                ImageSearch_TextBox.Text = CharacterName_TextBox.Text;
            }
        }

        private void CreateCharacter_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_characterCreationViewModel.SelectedNewCharacter==null)
            {
                if(PictureSearch_ListBox.SelectedItem==null && !CombatService.RosterViewModel.AllowCharactersWithNoVisuals )
                HighlightMissingData();
                //CreateCharacterError_Label.Content = "Please select the character to spawn an instance of.";
                return;
            }
            else
            {
                ResetMissingDataHighlight();
            }
            String newName = CharacterName_TextBox.Text.Trim();
            if (String.IsNullOrWhiteSpace(newName))
            {
                return;
            }
            int count = 1;
            String originalName = newName;
            while (CombatService.RosterViewModel.ActiveRoster.Any(x => x.Name.Equals(newName)))
            {
                newName = originalName + "_" + count;
                count++;
            }
            
            CharacterSheet newInstance = _characterCreationViewModel.SelectedNewCharacter.Copy(newName);
            newInstance.ResetIDOfCopy();

            newInstance.Ruleset = _characterCreationViewModel.SelectedNewCharacter.Ruleset;
            CombatService.RosterViewModel.RegisterNewCharacter(newInstance);

            var pictureInfo = PictureSearch_ListBox.SelectedItem as PictureFileInfo;
            var selectedTeam = TeamSelection_ListBox.SelectedItem as Team;
            if (selectedTeam == null)
            {
                selectedTeam = RosterViewModel.UnassignedTeam;
            }
            if (pictureInfo != null && selectedTeam != null)
            {
                CombatService.RosterViewModel.AddVisualToCharacters(new List<CharacterSheet>() { newInstance }, pictureInfo, selectedTeam);
                //Zoom on create
                
                VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.ZoomTo
                    (newInstance.FirstVisual.Location));
            }
            else if(CombatService.RosterViewModel.AllowCharactersWithNoVisuals)
            {
                CombatService.RosterViewModel.RegisterTeamMemberOnTeam(newInstance, selectedTeam);
            }
                 
        }

        private void ResetMissingDataHighlight()
        {
            AvailableNPCS_ListBox.Background = Brushes.White;
            PictureSearch_ListBox.Background = Brushes.White;
        }

        private void HighlightMissingData()
        {
            var pictureInfo = PictureSearch_ListBox.SelectedItem as PictureFileInfo;
            var selectedTeam = TeamSelection_ListBox.SelectedItem as Team;
            if(_characterCreationViewModel.SelectedNewCharacter == null )
            {
                AvailableNPCS_ListBox.Background = Brushes.Red;
            }
            if (PictureSearch_ListBox.SelectedItem == null && !CombatService.RosterViewModel.AllowCharactersWithNoVisuals)
            {
                PictureSearch_ListBox.Background = Brushes.Red;
            }
        }

    }
}
