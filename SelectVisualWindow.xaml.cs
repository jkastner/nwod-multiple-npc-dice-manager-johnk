﻿using System;
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
        public SelectVisualWindow(PictureSelectionViewModel pictureSelectionViewModel, ObservableCollection<Team> teams)
        {
            this._pictureSelectionViewModel = pictureSelectionViewModel;
            InitializeComponent();
            DataContext = pictureSelectionViewModel;
            _pictureSelectionViewModel.ResetActiveList();
            TeamSelection_ListBox.ItemsSource = teams;
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
    }
}