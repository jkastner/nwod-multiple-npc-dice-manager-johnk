using System;
using System.Collections;
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
                SetActiveCharacter(value, null);
                OnPropertyChanged("SelectedFullCharacter");
            }
        }

        private CharacterSheet _selectedActiveCharacter;
        public CharacterSheet SelectedActiveCharacter
        {
            get { return _selectedActiveCharacter; }
            set
            {
                SetActiveCharacter(null, value);
            }
        }

        private CharacterSheet _selectedCharacter;
        public CharacterSheet SelectedCharacter
        {
            get { return _selectedCharacter; }
        }

        private void SetActiveCharacter(CharacterSheet fullChar, CharacterSheet activeChar)
        {
            if (activeChar != null)
            {
                _selectedCharacter = activeChar;
            }
            if (fullChar != null)
            {
                _selectedCharacter = fullChar;
            }
            _selectedActiveCharacter = activeChar;
            _selectedFullCharacter = fullChar;
            OnPropertyChanged("SelectedFullCharacter");
            OnPropertyChanged("SelectedActiveCharacter");
            OnPropertyChanged("SelectedCharacter");
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


        private int _rollModifier;
        public int RollModifier
        {
            get { return _rollModifier; }
            set 
            { 
                _rollModifier = value;
                OnPropertyChanged("RollModifier");
            }
        }




        public CharacterSheet MakeChar(String fileName)
        {
            CharacterSheet curChar = null;
            try
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                List<NumberedTrait> curCharNumberedTraits = new List<NumberedTrait>();
                while (reader.Read())
                {

                    if (reader.Name.ToLower().Trim().Equals("name") && reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        reader.Read();
                        curChar = new NWoDCharacter(reader.Value, curCharNumberedTraits);
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

        internal IEnumerable<string> RollCharacters(IList characters, IList selectedTraits)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                bool canRoll = true;
                List<String> missingTraits = new List<String>();
                List<NumberedTrait> charTraits = new List<NumberedTrait>();
                foreach (var curTraitItem in selectedTraits)
                {
                    String curTrait = curTraitItem.ToString();
                    bool hasTrait = curChar.HasTrait(curTrait);
                    if (!hasTrait)
                    {
                        missingTraits.Add(curTrait);
                    }
                    else
                    {
                        charTraits.Add(curChar.FindTrait(curTrait));
                    }
                    canRoll &= hasTrait;
                }
                if (canRoll)
                {
                    int totalDice = 0;
                    foreach (NumberedTrait curTrait in charTraits)
                    {
                        totalDice += curTrait.TraitValue;
                    }
                    totalDice += RollModifier;
                    String result = curChar.Roll(totalDice);
                    yield return curChar.Name + result;
                }
                else
                {
                    StringBuilder result = new StringBuilder();
                    result.Append(curChar.Name + " was missing traits:");
                    for (int curIndex = 0; curIndex < missingTraits.Count();curIndex++ )
                    {
                        if (curIndex == missingTraits.Count() - 1)
                        {
                            result.Append(" " + missingTraits[curIndex]);
                        }
                        else
                        {
                            result.Append(", " + missingTraits[curIndex]);
                        }
                    }
                    yield return result.ToString();
                }
            }
        }

        internal void DoBashing(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoBashing();
            }
        }

        internal void DoLethal(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoLethal();
            }
        }

        internal void DoAggrivated(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoAggrivated();
            }
        }

        internal void ResetHealth(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.ResetHealth();
            }
        }

        internal void RollInitiative()
        {
            foreach (var curChar in ActiveRoster)
            {
                curChar.RollInitiative();
            }
            var q = ActiveRoster.OrderByDescending(x=> x.CurInitiative).ToList();
            ActiveRoster.Clear();
            foreach (var curChar in q)
            {
                ActiveRoster.Add(curChar);
            }
        }

        internal void RemoveActiveCharacters(IList characters)
        {
            for(int curIndex = characters.Count-1;curIndex>=0;curIndex--)
            {
                CharacterSheet curChar = characters[curIndex] as CharacterSheet;
                ActiveRoster.Remove(curChar);
            }
        }
    }
}
