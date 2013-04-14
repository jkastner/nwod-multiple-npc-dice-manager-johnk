using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using GameBoard;
using System.Windows.Media.Media3D;

namespace XMLCharSheets
{
    public abstract class CharacterSheet : INotifyPropertyChanged
    {
        internal static Random random = new Random();
        private String _name;

        public String Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                OnPropertyChanged("Name");
            }
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

        public NumericTrait HeightTrait
        {
            get
            {
                return NumericTraits.Where(x => x.TraitLabel.Equals("Height")).FirstOrDefault();
            }
        }
        public NumericTrait SpeedTrait
        {
            get
            {
                return NumericTraits.Where(x => x.TraitLabel.Equals("Speed")).FirstOrDefault();
            }
        }

        public virtual String Status
        {
            get
            {
                String status = "Status:\n";
                foreach (StatusEffect cur in StatusEffects)
                {
                    status = status + cur.Description +" {"+cur.DurationRemaining+"}\n";
                }
                return status;
            }
        }

        private String _additionalReportText;
        public virtual String AdditionalReportText
        {
            get { return _additionalReportText; }
            set { _additionalReportText = value; }
        }



        private String _description;
        //TODO - change combat traits when individual traits are changed.
        public CharacterSheet(string name, List<Trait> curTraits)
        {
            Name = name;
            _traits = curTraits;
            PopulateCombatTraits();
            
        }

        private List<Trait> _traits = new List<Trait>();
        public List<Trait> Traits
        {
            get { return _traits; }
            set 
            { 
                _traits = value; 
            }
        }

        private List<NumericTrait> _numericTraits = new List<NumericTrait>();
        public List<NumericTrait> NumericTraits
        {
            get
            {
                return Traits.Where(x => x is NumericTrait).Select(z => z as NumericTrait).ToList();
            }
        }


        public NumericTrait FindNumericTrait(String targetName)
        {
            return NumericTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }
        public bool HasTrait(String targetName)
        {
            return Traits.Where(x => x.TraitLabel.Equals(targetName)).Count()>0;
        }

        public override string ToString()
        {
            return Name;
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



        private bool _isSelected;
        public bool IsSelected
        {
            get 
            {
                if (Visual != null)
                {
                    return Visual.IsSelected;
                }
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (Visual != null)
                {
                    Visual.IsSelected = value;
                }
                OnPropertyChanged("IsSelected");
            }
        }

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


        internal virtual String NewRound()
        {
            for(int curIndex = StatusEffects.Count-1;curIndex>=0;curIndex--)
            {
                StatusEffect cur = StatusEffects[curIndex];
                cur.DurationRemaining--;
                if (cur.DurationRemaining <= 0)
                {
                    StatusEffects.Remove(cur);
                }
            }
            HasAttacked = false;
            HasMoved = false;
            NotifyStatusChange();
            return String.Empty;
        }




        internal void SetTarget(CharacterSheet target, List <String> otherTraits, string attackType, string damageType)
        {
            Target = target;
            OtherAttackTraits = otherTraits;
            DamageType = damageType;
            ChosenAttack = attackType;
        }


        private List<string> _otherAttackTraits;
        /// <summary>
        /// Other non-attack specific traits that can add to an attack.
        /// </summary>
        public List<string> OtherAttackTraits
        {
            get
            {
                return _otherAttackTraits;
            }
            set
            {
                _otherAttackTraits = value;
            }
        }

        public string ChosenAttackString
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ChosenAttack);
                for (int curIndex = 0; curIndex < OtherAttackTraits.Count(); curIndex++)
                {
                    sb.Append(", " + OtherAttackTraits[curIndex]);
                }
                return sb.ToString();
            }
        }

        private List<StatusEffect> _statusEffects = new List<StatusEffect>();

        public List<StatusEffect> StatusEffects
        {
            get { return _statusEffects; }
            set 
            {
                _statusEffects = value;
                NotifyStatusChange();
            }
        }

        public void NotifyStatusChange()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusesLine");
            OnPropertyChanged("StatusEffects");
            OnPropertyChanged("StatusColor");
        }

        private String _statusesLine;
        public String StatusesLine
        {
            get 
            {
                
                String statusLine = "";
                foreach (StatusEffect cur in StatusEffects)
                {
                    statusLine = statusLine + " " + cur.Description;
                }
                _statusesLine = statusLine.Trim();
                return _statusesLine;
            }
            set { _statusesLine = value; }
        }
        
        


        internal void AssignStatus(string description, int duration)
        {
            StatusEffects.Add(new StatusEffect(description, duration));
            NotifyStatusChange();
        }

        

        public int FinalAttackPool { get; set; }

        private bool _displayCharacter;

        public bool DisplayCharacter
        {
            get { return _displayCharacter; }
            set 
            { 
                _displayCharacter = value;
                OnPropertyChanged("DisplayCharacter");
            }
        }

        protected void SetIncapacitated(bool isDead)
        {
            var incapEffect = StatusEffects.Where(x => x.Description.Equals("Incapacitated")).FirstOrDefault();
            
            if (isDead)
            {
                if (incapEffect == null)
                {
                    StatusEffects.Add(new StatusEffect("Incapacitated", 500));
                }
            }
            else
            {
                if (incapEffect != null)
                {
                    StatusEffects.Remove(incapEffect);
                }
            }
            IsIncapacitated = isDead;
            NotifyStatusChange();
        }


        public MoveablePicture Visual { get; set; }

        private Color _pieceColor;
        public Color PieceColor
        {
            get { return _pieceColor; }
            set
            {
                _pieceColor = value;
                OnPropertyChanged("PieceColor");
            }
        }

        public double DistanceTo(Point3D target)
        {
            if (Visual == null)
            {
                return double.MaxValue;
            }
            return Helper3DCalcs.DistanceBetween(Visual.Location, target); 
        }

        public Team Team { get; set; }

        private bool _hasAttacked = false;
        public bool HasAttacked
        {
            get { return _hasAttacked; }
            set 
            {
                _hasAttacked = value;
                OnPropertyChanged("HasAttacked");
            }
        }

        private bool _hasMoved = false;
        public bool HasMoved
        {
            get { return _hasMoved; }
            set
            {
                _hasMoved = value;
                OnPropertyChanged("HasMoved");
            }
        }

        #region Abstracts
        public abstract void PopulateCombatTraits();

        public abstract void RollInitiative();


        public abstract SolidColorBrush StatusColor
        {
            get;
        }
        public abstract String RollResults
        {
            get;
            set;
        }

        internal abstract CharacterSheet Copy(string newName);

        internal abstract String DoDamage(int value, String descriptor);

        internal abstract void ResetHealth();

        public abstract String ChosenAttackValue
        {
            get;
        }

        internal abstract List<Damage> AttackTarget(int RollModifier);

        internal abstract DicePool RollBasePool(List<Trait> dicePools, int modifier);

        public bool IsIncapacitated { get; private set; }
        #endregion

    }
}
