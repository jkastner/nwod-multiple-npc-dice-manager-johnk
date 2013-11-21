using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatAutomationTheater
{
    internal class CharacterCreationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CharacterSheet> FilteredCharacters = new ObservableCollection<CharacterSheet>();
        public CharacterCreationViewModel()
        {
            ResetActiveList();
            CombatService.RosterViewModel.RulesetSelected += RulesetSelected;
        }

        private void RulesetSelected(object sender, EventArgs e)
        {
            var prevSelected = SelectedNewCharacter;
            ResetActiveList();
            if (FilteredCharacters.Contains(prevSelected))
            {
                SelectedNewCharacter = prevSelected;
            }
        }

        public CharacterSheet _selectedNewCharacter;
        public CharacterSheet SelectedNewCharacter
        {
            get { return _selectedNewCharacter; }
            set 
            { 
                _selectedNewCharacter = value;
                OnPropertyChanged("SelectedNewCharacter");
                CombatService.RosterViewModel.SelectedFullCharacter = value;
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
            if (IsDefaultName(CurrentCharacterName))
            {
                if (SelectedNewCharacter == null)
                {
                    
                    /* There was a bug where when the first character was selected, the ruleset would be set.
                     * Then the roster would be trimmed of non-usable characters, cleared, and added to available characters.
                     * The selectedcharacter was null during this
                     * And the 'name' was set to 'Name'. Once that changed, the image search was reset, and the 
                     * selected image would be set to null.
                     * Simply, don't change the name in this case.
                     * */
                    //CurrentCharacterName = "Name";
                }
                else
                {
                    CurrentCharacterName = SelectedNewCharacter.Name;
                }
            }
        }

        internal bool IsDefaultName(string CurrentCharacterName)
        {
            return CurrentCharacterName.Equals("Name") ||
                CurrentCharacterName.Equals("Image search...") ||
                String.IsNullOrWhiteSpace(CurrentCharacterName.Trim()) ||
                CombatService.RosterViewModel.FullRoster.Any(x => x.Name.Equals(CurrentCharacterName));
        }

    }
}
