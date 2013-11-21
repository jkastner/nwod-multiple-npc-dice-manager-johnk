using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CombatAutomationTheater
{
    /// <summary>
    ///     Interaction logic for SelectVisualWindow.xaml
    /// </summary>
    public partial class SelectVisualWindow : Window
    {
        private readonly String _baseName = "";
        private readonly PictureSelectionViewModel _pictureSelectionViewModel;
        private Color _chosenColor = Colors.Gray;

        public SelectVisualWindow(String baseName, PictureSelectionViewModel pictureSelectionViewModel,
                                  ObservableCollection<Team> teams)
        {
            _pictureSelectionViewModel = pictureSelectionViewModel;
            _baseName = baseName;
            InitializeComponent();
            DataContext = pictureSelectionViewModel;
            _pictureSelectionViewModel.ResetActiveList();
            TeamSelection_ListBox.ItemsSource = teams;
            SearchedDisplayItems_ListBox.ItemsSource = _pictureSelectionViewModel.ActiveLoadedPictures;
            TeamSelection_ListBox.SelectedIndex = 0;
        }


        public bool WasCancel { get; set; }

        public Color ChosenColor
        {
            get { return _chosenColor; }
            set { _chosenColor = value; }
        }

        public Team ChosenTeam
        {
            get { return (TeamSelection_ListBox.SelectedItem as Team); }
        }


        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
            ChosenColor = (TeamSelection_ListBox.SelectedItem as Team).TeamColor;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            WasCancel = true;
            Close();
        }

        private void TrimList_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            RunSearch();
        }

        private void RunSearch()
        {
            _pictureSelectionViewModel.AdjustList(TrimList_TextBox.Text.Trim().ToLower());
            if (SearchedDisplayItems_ListBox.Items.Count == 1)
            {
                SearchedDisplayItems_ListBox.SelectedIndex = 0;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (
                _pictureSelectionViewModel.AllLoadedPictures.Any(
                    x => x.PictureName.ToLower().Contains(_baseName.ToLower())))
            {
                TrimList_TextBox.Text = _baseName;
                RunSearch();
            }
        }

        private void SelectVisualWindow_KeyDown(object sender, KeyEventArgs e)
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
    }
}