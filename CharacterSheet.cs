using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using System.ComponentModel;

namespace XMLCharSheets
{
    public abstract class CharacterSheet : INotifyPropertyChanged
    {
        internal static Random random = new Random();
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _initiativeModifier;

        public int InitiativeModifier
        {
            get { return _initiativeModifier; }
            set { _initiativeModifier = value; }
        }

        private int _initiative;
        public int Initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }       
        
        private int _curInitiative = -1;
        public int CurInitiative
        {
            get { return _curInitiative; }
            set { _curInitiative = value; }
        }

        public abstract void RollInitiative();
        

        public abstract String Status
        {
            get;
            set;
        }



        private String _description;

        public CharacterSheet(string name, List<NumberedTrait> curTraits)
        {
            Name = name;
            _numberedTraits = curTraits;
            PopulateCombatTraits();
            
        }

        protected abstract void PopulateCombatTraits();



        private List<NumberedTrait> _numberedTraits = new List<NumberedTrait>();
        public List<NumberedTrait> NumberedTraits
        {
            get { return _numberedTraits; }
            set { _numberedTraits = value; }
        }
        
        public NumberedTrait FindTrait(String targetName)
        {
            return (NumberedTrait) NumberedTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }
        public bool HasTrait(String targetName)
        {
            return NumberedTraits.Where(x => x.TraitLabel.Equals(targetName)).Count()>0;
        }

        public override string ToString()
        {
            if(CurInitiative == -1)
                return Name;
            return Name + " - {" + CurInitiative+"}";
        }


        internal static CharacterSheet Copy(CharacterSheet character, string newName)
        {
            
            List<NumberedTrait> copyTraits = new List<NumberedTrait>();
            foreach(NumberedTrait curTrait in character.NumberedTraits)
            {
                NumberedTrait newTrait = new NumberedTrait(curTrait.TraitValue, curTrait.TraitLabel);
                copyTraits.Add(newTrait);
            }
            if (character is NWoDCharacter)
            {
                CharacterSheet copy = new NWoDCharacter(newName, copyTraits);
                return copy;
            }
            return null;
        }

        internal abstract string Roll(int totalDice);

        internal abstract void DoBashing();
        internal abstract void DoLethal();
        internal abstract void DoAggrivated();

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

        internal abstract void ResetHealth();

    }
}
