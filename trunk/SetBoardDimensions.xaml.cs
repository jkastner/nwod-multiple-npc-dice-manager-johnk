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
using System.Windows.Shapes;

namespace XMLCharSheets
{
    /// <summary>
    /// Interaction logic for SetBoardDimensions.xaml
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
            get
            {
                return double.Parse(BoardHeight_TextBox.Text);
            }
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
            get
            {
                return double.Parse(BoardWidth_TextBox.Text);
            }
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


    }

}