using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for PositiveIntegerTextBox.xaml
    /// </summary>
    public partial class PositiveIntegerTextBox : TextBox
    {
        public PositiveIntegerTextBox()
        {
            InitializeComponent();
        }

        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            var regex = new Regex("[^0-9]"); //regex that matches disallowed text
            bool isGood = !regex.IsMatch(text);
            return isGood;
        }

        private void PositiveIntegerTextBox_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isGood = IsTextNumeric(e.Text);
            e.Handled = !isGood;
        }

        // Use the DataObject.Pasting Handler 
        private void PositiveIntegerTextBox_TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                var text = (String)e.DataObject.GetData(typeof(String));
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
