using System.Windows;

namespace CombatAutomationTheater
{
    /// <summary>
    ///     Interaction logic for SetBoardDimensions.xaml
    /// </summary>
    public partial class SetBoardDimensions : Window
    {
        public SetBoardDimensions(double height, double width)
        {
            InitializeComponent();
            BoardHeight_TextBox.Text = height.ToString();
            BoardWidth_TextBox.Text = width.ToString();
        }

        public bool HasBoardHeight
        {
            get
            {
                double Num = 0;
                return double.TryParse(BoardHeight_TextBox.Text, out Num);
            }
        }

        public double BoardHeight
        {
            get { return double.Parse(BoardHeight_TextBox.Text); }
        }

        public bool MaintainRatio
        {
            get { return (bool) Maintain_Ratio_CheckBox.IsChecked; }
        }


        public bool HasBoardWidth
        {
            get
            {
                double Num = 0;
                return double.TryParse(BoardWidth_TextBox.Text, out Num);
            }
        }

        public double BoardWidth
        {
            get { return double.Parse(BoardWidth_TextBox.Text); }
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

        private void Maintain_Ratio_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var isChecked = (bool)Maintain_Ratio_CheckBox.IsChecked;
            if (isChecked)
            {
                BoardWidth_TextBox.IsEnabled = false;
            }
            else
            {
                BoardWidth_TextBox.IsEnabled = true;
            }
        }
    }
}