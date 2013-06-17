using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for SelectVisualWindow.xaml
    /// </summary>
    public partial class SelectVisualWindow : Window
    {

        private PictureSelectionViewModel _pictureSelectionViewModel;
        private String _baseName = "";
        public SelectVisualWindow(String baseName, PictureSelectionViewModel pictureSelectionViewModel, ObservableCollection<Team> teams)
        {
            this._pictureSelectionViewModel = pictureSelectionViewModel;
            _baseName = baseName;
            InitializeComponent();
            DataContext = pictureSelectionViewModel;
            _pictureSelectionViewModel.ResetActiveList();
            TeamSelection_ListBox.ItemsSource = teams;
            SearchedDisplayItems_ListBox.ItemsSource = _pictureSelectionViewModel.ActiveLoadedPictures;
            TeamSelection_ListBox.SelectedIndex = 0;

        }


        bool _wasCancel;
        public bool WasCancel
        {
            get { return _wasCancel; }
            set { _wasCancel = value; }
        }


        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
            ChosenColor = (TeamSelection_ListBox.SelectedItem as Team).TeamColor;
            this.Close();
        }

        Color _chosenColor = Colors.Gray;
        public Color ChosenColor 
        {
            get
            {
                return _chosenColor;
            }
            set
            {
                _chosenColor = value;
            }
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

        public Team ChosenTeam 
        {
            get
            {
                return (TeamSelection_ListBox.SelectedItem as Team);
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_pictureSelectionViewModel.AllLoadedPictures.Any(x => x.PictureName.ToLower().Contains(_baseName.ToLower())))
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
