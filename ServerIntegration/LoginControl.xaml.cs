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
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        public bool WasCancel { get; set; }
        
        public struct LoginInfo
        {
            public String Username;
            public String TargetSite;
            public String Password;
        }
        public LoginInfo CurrentLoginInformation
        {
            get
            {
                return new LoginInfo()
                {
                    Username = UserName_TextBlock.Text,
                    Password = Password_TextBlock.Password,
                    TargetSite = LoginSite_TextBlock.Text,
                };
            }
        }
    }
}
