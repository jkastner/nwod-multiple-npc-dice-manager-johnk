using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace XMLCharSheets
{
    public class Character
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _description;

        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public Character(string name, string description)
        {
            _name = name;
            _description = description;
            _numberedTraits = new ObservableCollection<NumberedTrait>();

        }

        private ObservableCollection<NumberedTrait> _numberedTraits;

        public ObservableCollection<NumberedTrait> NumberedTraits
        {
            get { return _numberedTraits; }
            set { _numberedTraits = value; }
        }
        
        public NumberedTrait FindTrait(String targetName)
        {
            return (NumberedTrait) NumberedTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }
    }
}
