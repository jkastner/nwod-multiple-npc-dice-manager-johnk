using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;

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

        public CharacterSheet(string name, List<Trait> curTraits)
        {
            Name = name;
            _traits = curTraits;
            PopulateCombatTraits();
            
        }

        protected abstract void PopulateCombatTraits();



        private List<Trait> _traits = new List<Trait>();
        public List<Trait> Traits
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public Trait FindTrait(String targetName)
        {
            return (Trait)Traits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }
        public bool HasTrait(String targetName)
        {
            return Traits.Where(x => x.TraitLabel.Equals(targetName)).Count()>0;
        }

        public override string ToString()
        {
            return Name;
        }
        public abstract SolidColorBrush StatusColor
        {
            get;
        }
        public abstract String RollResults
        {
            get;
        }

        internal static CharacterSheet Copy(CharacterSheet character, string newName)
        {

            List<Trait> copyTraits = new List<Trait>();

            foreach(Trait curTrait in character.Traits)
            {
                Trait copiedTrait = curTrait.CopyTrait();
                copyTraits.Add(copiedTrait);
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

        private CharacterSheet _target;
        public CharacterSheet Target
        {
            get { return _target; }
            set {
                
                _target = value;
                OnPropertyChanged("Target");
            }
        }

        private String _damageType;
        public String DamageType
        {
            get { return _damageType; }
            set { _damageType = value; }
        }
        private String _chosenAttack;

        public String ChosenAttack
        {
            get { return _chosenAttack; }
            set { _chosenAttack = value; }
        }
        
        internal void SetTarget(CharacterSheet target, String attackType, string damageType)
        {
            Target = target;
            DamageType = damageType;
            ChosenAttack = attackType;
        }



        internal abstract void AttackTarget();



        internal abstract void NewRound();
    }
}
