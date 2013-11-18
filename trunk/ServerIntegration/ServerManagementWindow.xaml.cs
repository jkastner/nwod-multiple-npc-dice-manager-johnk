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
    /// Interaction logic for ServerManagementWindow.xaml
    /// </summary>
    public partial class ServerManagementWindow : Window
    {
        private CharacterDownloadedReporter _characterDownloadedReporter;
        private SiteQuery _sq;
        public ServerManagementWindow()
        {
            InitializeComponent();
            _characterDownloadedReporter = new CharacterDownloadedReporter();
            CurrentLoginControl.Cancel_Button.Click += Cancel_Button_Click;
            CurrentLoginControl.Login_Button.Click += Login_Button_Click;
            CurrentCharacterBrowser.Visibility = Visibility.Collapsed;
            _characterDownloadedReporter.CloseRequested += CloseRequested;
        }


        internal void CloseRequested(object sender, EventArgs e)
        {
            _sq = null;
            this.Close();
        }


        public CharacterDownloadedReporter CharacterDownloadedReporter
        {
            get
            {
                return _characterDownloadedReporter;
            }
        }
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            var loginInfo = CurrentLoginControl.CurrentLoginInformation;
            _sq = new SiteQuery(loginInfo.TargetSite, loginInfo.Username, loginInfo.Password);
            _sq.Login();
            if (!_sq.LoginSuccessful)
            {
                LoginMessage_TextBlock.Text = "Error connecting";
            }
            else
            {
                LoginMessage_TextBlock.Visibility = Visibility.Collapsed;
                CurrentLoginControl.Visibility = Visibility.Collapsed;
                CurrentCharacterBrowser.SetSiteQuery(_sq);
                CurrentCharacterBrowser.SetCharacterDownloadedReporter(_characterDownloadedReporter);                
                CurrentCharacterBrowser.Visibility = Visibility.Visible;

            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        public static string username = "johnathan.kastner@gmail.com";
        public static string password = "zS29OYrYgS";
        private static string site = "http://strange-aeons.herokuapp.com/";
        public static string loginSite = "http://strange-aeons.herokuapp.com/login";
        public static string characterQueryURL = "http://strange-aeons.herokuapp.com/get?Type=Character&name=";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var readCharacter = SiteQuery.RetrieveCharacter("TestCharacter", loginSite, characterQueryURL, username, password);
            //var converted = SheetConverter.Convert(readCharacter);
        }


    }
}
