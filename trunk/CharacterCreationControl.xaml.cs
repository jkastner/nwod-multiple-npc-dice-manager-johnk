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
        public CharacterCreationControl()
        {
            InitializeComponent();
            DataContext = _characterCreationViewModel;
            AvailableNPCS_ListBox.ItemsSource = _characterCreationViewModel.FilteredCharacters;
            
            
            
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
            RunSearch();
        }

        private void RunSearch()
        {
            var searchText = CharacterSheetSearcher_TextBox.Text.Trim().ToLower();
            if(searchText.Equals("search..."))
                return;
            ImageSearch_TextBox.Text = CharacterSheetSearcher_TextBox.Text;
            _characterCreationViewModel.SearchForText(searchText);
            if (_characterCreationViewModel.FilteredCharacters.Count >= 1)
            {
                AvailableNPCS_ListBox.SelectedIndex = 0;
            }

        }

        private void CharacterCreationControl_Loaded(object sender, RoutedEventArgs e)
        {
            _characterCreationViewModel.ResetActiveList();
            RunSearch();
        }


    }
}
