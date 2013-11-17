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

namespace ServerIntegration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            
        }
        public static string username = "johnathan.kastner@gmail.com";
        public static string password = "zS29OYrYgS";
        private static string site = "http://strange-aeons.herokuapp.com/";
        public static string loginSite = "http://strange-aeons.herokuapp.com/login";
        public static string characterQueryURL = "http://strange-aeons.herokuapp.com/get?Type=Character&name=";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var readCharacter = SiteQuery.RetrieveCharacter("TestCharacter", loginSite, characterQueryURL, username, password);
            var converted = SheetConverter.Convert(readCharacter);
        }
    }
}
