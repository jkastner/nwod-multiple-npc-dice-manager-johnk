using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace XMLCharSheets
{
    public class RosterViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CharacterSheet> _fullRoster = new ObservableCollection<CharacterSheet>();

        public ObservableCollection<CharacterSheet> FullRoster
        {
            get { return _fullRoster; }
            set 
            {
                _fullRoster = value;
            }
        }


        private ObservableCollection<CharacterSheet> _activeRoster = new ObservableCollection<CharacterSheet>();
        public ObservableCollection<CharacterSheet> ActiveRoster
        {
            get { return _activeRoster; }
            set
            {
                _activeRoster = value;
            }
        }

        private ObservableCollection<String> _currentTraits = new ObservableCollection<String>();
        public ObservableCollection<String> CurrentTraits
        {
            get { return _currentTraits; }
            set
            {
                _currentTraits = value;
            }
        }

        private CharacterSheet _selectedFullCharacter;
        public CharacterSheet SelectedFullCharacter
        {
            get { return _selectedFullCharacter; }
            set
            {
                _selectedFullCharacter = value;
                OnPropertyChanged("SelectedFullCharacter");
            }
        }

        private CharacterSheet _selectedActiveCharacter;
        public CharacterSheet SelectedActiveCharacter
        {
            get { return _selectedActiveCharacter; }
            set
            {
                _selectedActiveCharacter = value;
                OnPropertyChanged("SelectedActiveCharacter");
            }
        }


        
        
        
        
        internal void PopulateCharacters(String sourceDir)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(sourceDir);
            foreach (string fileName in fileEntries)
            {
                FullRoster.Add(MakeChar(fileName));
            }


            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            foreach (string subdir in subdirEntries)
            {
                PopulateCharacters(subdir);
            }
        }




        public CharacterSheet MakeChar(String fileName)
        {
            CharacterSheet curChar = null;
            try
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                ObservableCollection<NumberedTrait> curCharNumberedTraits = new ObservableCollection<NumberedTrait>();
                while (reader.Read())
                {

                    if (reader.Name.ToLower().Trim().Equals("name") && reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        reader.Read();
                        curChar = new CharacterSheet(reader.Value, curCharNumberedTraits);
                    }
                    if (reader.Name.ToLower().Trim().Equals("trait"))
                    {
                        String label = reader.GetAttribute("label");
                        int value = Convert.ToInt32(reader.GetAttribute("value"));
                        NumberedTrait curNumberedTrait = new NumberedTrait(value, label);
                        curCharNumberedTraits.Add(curNumberedTrait);
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error opening " + fileName);
                throw;
            }
            return curChar;
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
    }
}
