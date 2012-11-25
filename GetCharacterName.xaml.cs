using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// Interaction logic for GetCharacterName.xaml
    /// </summary>
    public partial class GetCharacterName : Window, INotifyPropertyChanged
    {
        private String _providedName;
        public String ProvidedName
        {
            get { return _providedName; }
            set
            {
                _providedName = value;
                OnPropertyChanged("ProvidedName");
            }
        }

        private bool _wasCancel;

        public bool WasCancel
        {
            get { return _wasCancel; }
            set { _wasCancel = value; }
        }
        

        public GetCharacterName(string baseName, ICollection<CharacterSheet> activeCharacters)
        {
            bool foundName = false;
            int numPrev = 1;
            ProvidedName = baseName;
            while (!foundName)
            {
                foundName = true;
                foreach (CharacterSheet curCharacter in activeCharacters)
                {
                    foundName &= !curCharacter.Name.Equals(ProvidedName);
                    if (curCharacter.Name.Equals(ProvidedName))
                    {
                        ProvidedName = baseName + "_" + numPrev;
                        numPrev++;
                        break;
                    }
                }
            }
            DataContext = this;
            InitializeComponent();

        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

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

        private void GetName_KeyDown(object sender, KeyEventArgs e)
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
    }
}
