using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for StatusEffectWindow.xaml
    /// </summary>
    public partial class StatusEffectWindow : Window
    {
        public StatusEffectWindow()
        {
            InitializeComponent();
            StatusDescription.Focus();
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
            this.Close();
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

        private void StatusEffect_KeyDown(object sender, KeyEventArgs e)
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


        private void Modifier_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isGood = IsTextNumeric(e.Text);
            e.Handled = !isGood;

        }
        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            Regex regex = new Regex("[^0-9]"); //regex that matches disallowed text
            bool isGood = !regex.IsMatch(text);
            return isGood;
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
    }
}
