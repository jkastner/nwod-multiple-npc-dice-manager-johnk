using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for ActiveCharacterDisplay.xaml
    /// </summary>
    public partial class ActiveCharacterDisplay : UserControl
    {

        private List<CharacterSheet> activeList;
        private double _totalWidth = 0;
        public ActiveCharacterDisplay(List<CharacterSheet> activeList)
        {
            this.activeList = activeList;
            InitializeComponent();
            foreach (var cur in activeList)
            {
                double widthIncrease = FindWidthOfText(cur.Name);
                
                _totalWidth += widthIncrease;


                ActiveDisplayImagesAndName_ListBox.Items.Add(cur);
                
                JustnameGrid.Children.Add
                    (
                        MakeDisplay(cur, activeList.Count)
                    );
                //ActiveDisplayJustName_ListBox.Items.Add(cur);
            }
            Loaded += UILoaded;


        }

        private UIElement MakeDisplay(CharacterSheet cur, int totalamount)
        {
            double fontSize = 12;
            if (totalamount < 4)
            {
                fontSize = 18;
            }
            else if (totalamount < 8)
            {
                fontSize = 14;
            }
            else if (totalamount < 16)
            {
                fontSize = 12;
            }
            else
            {
                fontSize = 10;
            }
            Border b = new Border();
            b.BorderThickness = new Thickness(1,1,1,1);
            b.BorderBrush = new SolidColorBrush(Colors.Black);
            b.Child = new TextBlock() {Text = cur.Name, FontSize = fontSize, Margin = new Thickness(1,1,1,1)};
            
            return b;
        }

        private double FindWidthOfText(string text)
        {
            var baseFont = ActiveDisplayImagesAndName_ListBox.FontFamily.FamilyNames.First().Value;
            var baseFontSize = (float)(ActiveDisplayImagesAndName_ListBox.FontSize);
            System.Drawing.Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, new Font(baseFont, baseFontSize));
            double widthIncrease = textSize.Width;
            widthIncrease += 10;
            if (widthIncrease < 70)
            {
                widthIncrease = 70;
            }
            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();
            return widthIncrease;
        }

        private void UILoaded(object sender, RoutedEventArgs e)
        {
            if (_totalWidth > this.ActualWidth)
            {
                ActiveDisplayImagesAndName_ListBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                //ActiveDisplayJustName_ListBox.Visibility = Visibility.Collapsed;
                JustnameGrid.Visibility = Visibility.Collapsed;
            } 
        }
    }
}
