using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CombatAutomationTheater
{
    /// <summary>
    ///     Interaction logic for StatusEffectWindow.xaml
    /// </summary>
    public partial class StatusEffectWindow : Window
    {
        public StatusEffectWindow()
        {
            InitializeComponent();
            StatusDescription.Focus();
        }

        public bool WasCancel { get; set; }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
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
            var regex = new Regex("[^0-9]"); //regex that matches disallowed text
            bool isGood = !regex.IsMatch(text);
            return isGood;
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
    }
}