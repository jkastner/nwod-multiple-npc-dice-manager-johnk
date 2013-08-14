using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    internal class CharacterCreationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CharacterSheet> FilteredCharacters = new ObservableCollection<CharacterSheet>();
        public CharacterCreationViewModel()
        {
            ResetActiveList();
        }
        public CharacterSheet _selectedNewCharacter;
        public CharacterSheet SelectedNewCharacter
        {
            get { return _selectedNewCharacter; }
            set 
            { 
                _selectedNewCharacter = value;
                OnPropertyChanged("SelectedNewCharacter");
                CheckForNameChange();
            }
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

        internal void SearchForText(string targetCharacterSheet)
        {
            if (String.IsNullOrWhiteSpace(targetCharacterSheet))
            {
                ResetActiveList();
                return;
            }
            FilteredCharacters.Clear();
            var matchingList = CombatService.RosterViewModel.FullRoster.Where(x => x.Name.ToLower().Contains(targetCharacterSheet));
            foreach (var cur in matchingList)
            {
                FilteredCharacters.Add(cur);
            }
        }

        public void ResetActiveList()
        {
            if (CombatService.RosterViewModel.FullRoster.Count() != FilteredCharacters.Count())
            {
                FilteredCharacters.Clear();
                foreach (var cur in CombatService.RosterViewModel.FullRoster)
                {
                    FilteredCharacters.Add(cur);
                }
            }
        }
        private String _currentCharacterName = "Name";
        public string CurrentCharacterName
        {
            get
            {
                if (SelectedNewCharacter == null)
                {
                    return _currentCharacterName;
                }
                return _currentCharacterName;
            }
            set 
            {
                _currentCharacterName = value;
                OnPropertyChanged("CurrentCharacterName");
            }
        }

        private void CheckForNameChange()
        {
            
            //Set the current name to the default character name if it equals 'name' or if it's empty or if the name matches an existing character name.
            if (CurrentCharacterName.Equals("Name")||
                String.IsNullOrWhiteSpace(CurrentCharacterName.Trim()) ||
                CombatService.RosterViewModel.FullRoster.Any(x => x.Name.Equals(CurrentCharacterName)))
            {
                if (SelectedNewCharacter == null)
                {
                    CurrentCharacterName = "Name";
                }
                else
                {
                    CurrentCharacterName = SelectedNewCharacter.Name;
                }
            }
        }

    }
}
