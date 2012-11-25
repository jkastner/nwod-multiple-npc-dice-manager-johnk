using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;

namespace XMLCharSheets
{
    public class CharacterSheet
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _description;

        public CharacterSheet(string name, ObservableCollection<NumberedTrait> curTraits)
        {
            _name = name;
            _numberedTraits = curTraits;
        }

        public CharacterSheet()
        {
        }

        private ObservableCollection<NumberedTrait> _numberedTraits = new ObservableCollection<NumberedTrait>();

        public ObservableCollection<NumberedTrait> NumberedTraits
        {
            get { return _numberedTraits; }
            set { _numberedTraits = value; }
        }
        
        public NumberedTrait FindTrait(String targetName)
        {
            return (NumberedTrait) NumberedTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }


        public override string ToString()
        {
            return Name;
        }


        internal static CharacterSheet Copy(CharacterSheet character, string newName)
        {
            CharacterSheet copy = new CharacterSheet();
            copy.Name = newName;
            foreach(NumberedTrait curTrait in character.NumberedTraits)
            {
                NumberedTrait newTrait = new NumberedTrait(curTrait.TraitValue, curTrait.TraitLabel);
                copy.NumberedTraits.Add(newTrait);
            }
            return copy;
        }
    }
}
