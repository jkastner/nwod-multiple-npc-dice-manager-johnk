using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GameBoard;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    [KnownType(typeof (PathfinderCharacter_HP))]
    [KnownType(typeof (PathfinderCharacter_WoundsVigor))]
    [KnownType(typeof (NWoDCharacter))]
    [KnownType(typeof (NWoDVampire))]
    public abstract class CharacterSheet : INotifyPropertyChanged
    {
        internal static Random random = new Random();
        private int _curInitiative = -1;
        private bool _displayCharacter;
        private bool _hasAttacked;
        private bool _hasMoved;
        private bool _isSelected;
        private String _name;
        private List<NumericIntTrait> _numericTraits = new List<NumericIntTrait>();
        private Color _pieceColor;
        private List<StatusEffect> _statusEffects = new List<StatusEffect>();
        private String _statusesLine;
        private CharacterSheet _target;
        private List<Trait> _traits = new List<Trait>();
        private CharacterActionScript _currentCharacterActionScript = null;

        public CharacterSheet(string name, List<Trait> curTraits)
        {
            Name = name;
            _traits = curTraits;
            PopulateCombatTraits();
            UniqueCharacterID = Guid.NewGuid();
        }

        [DataMember]
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        [DataMember]
        public int InitiativeModifier { get; set; }

        [DataMember]
        public int Initiative { get; set; }

        [DataMember]
        public int CurInitiative
        {
            get { return _curInitiative; }
            set { _curInitiative = value; }
        }


        public NumericIntTrait HeightTrait
        {
            get { return NumericTraits.Where(x => x.TraitLabel.Equals("Height")).FirstOrDefault(); }
        }

        public NumericIntTrait SpeedTrait
        {
            get { return NumericTraits.Where(x => x.TraitLabel.Equals("Speed")).FirstOrDefault(); }
        }

        public virtual String Status
        {
            get
            {
                String status = "Status:\n";
                foreach (StatusEffect cur in StatusEffects)
                {
                    status = status + cur.Description + " {" + cur.DurationRemaining + "}\n";
                }
                return status;
            }
        }

        //TODO - change combat traits when individual traits are changed.

        [DataMember]
        public List<Trait> Traits
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public List<NumericIntTrait> NumericTraits
        {
            get { return Traits.Where(x => x is NumericIntTrait).Select(z => z as NumericIntTrait).ToList(); }
        }


        [DataMember]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (HasVisual)
                {
                    BoardsViewModel.Instance.ForeachBoard(x=>x.VisualsViewModel.MatchingVisual(UniqueCharacterID).IsSelected = value);
                }
                OnPropertyChanged("IsSelected");
            }
        }
        public bool HasVisual
        {
            get
            {
                return VisualsService.BoardsViewModel.HasAssociatedVisual(this.UniqueCharacterID);
            }
        }
        public MoveablePicture FirstVisual
        {
            get
            {
                return VisualsService.BoardsViewModel.AssociatedVisual(UniqueCharacterID);
            }
        }
        




        [DataMember]
        public CharacterSheet Target
        {
            get { return _target; }
            set
            {
                _target = value;
                OnPropertyChanged("Target");
            }
        }

        [DataMember]
        public String DamageType { get; set; }

        [DataMember]
        public String ChosenAttack { get; set; }

        [DataMember]
        public Guid UniqueCharacterID
        {
            get;
            private set;
        }



        /// <summary>
        ///     Other non-attack specific traits that can add to an attack.
        /// </summary>
        [DataMember]
        public List<string> OtherAttackTraits { get; set; }

        public string ChosenAttackString
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(ChosenAttack);
                for (int curIndex = 0; curIndex < OtherAttackTraits.Count(); curIndex++)
                {
                    sb.Append(", " + OtherAttackTraits[curIndex]);
                }
                return sb.ToString();
            }
        }

        [DataMember]
        public List<StatusEffect> StatusEffects
        {
            get { return _statusEffects; }
            set
            {
                _statusEffects = value;
                NotifyStatusChange();
            }
        }

        [DataMember]
        public int FinalAttackPool { get; set; }

        [DataMember]
        public bool DisplayCharacter
        {
            get { return _displayCharacter; }
            set
            {
                _displayCharacter = value;
                OnPropertyChanged("DisplayCharacter");
            }
        }


        

        [DataMember]
        public Team Team { get; set; }

        [DataMember]
        public bool HasAttacked
        {
            get { return _hasAttacked; }
            set
            {
                _hasAttacked = value;
                OnPropertyChanged("HasAttacked");
            }
        }

        [DataMember]
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

        public abstract SolidColorBrush StatusColor { get; }
        public abstract String RollResults { get; set; }
        public abstract String ChosenAttackValue { get; }
        public bool IsIncapacitated { get; private set; }
        internal abstract String HealthStatusLineDescription { get; }

        [DataMember]
        public String Ruleset { get; set; }

        public abstract void PopulateCombatTraits();

        public abstract void RollInitiative();

        internal abstract CharacterSheet Copy(string newName);

        internal abstract String DoDamage(int value, String descriptor);

        internal abstract void ResetHealth();

        internal abstract List<Damage> AttackTarget(int RollModifier);

        internal abstract DicePool RollBasePool(List<Trait> dicePools, int modifier);

        #endregion

        internal void Report(String newText)
        {
            TextReporter.Report(newText);
        }

        public NumericIntTrait FindNumericTrait(String targetName)
        {
            return NumericTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }

        public bool HasTrait(String targetName)
        {
            return Traits.Where(x => x.TraitLabel.Equals(targetName)).Count() > 0;
        }

        public override string ToString()
        {
            return Name;
        }

        internal virtual String NewRound()
        {
            for (int curIndex = StatusEffects.Count - 1; curIndex >= 0; curIndex--)
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


        internal void SetTarget(CharacterSheet target, List<String> otherTraits, string attackType, string damageType)
        {
            Target = target;
            OtherAttackTraits = otherTraits;
            DamageType = damageType;
            ChosenAttack = attackType;
        }

        public void NotifyStatusChange()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("StatusesLine");
            OnPropertyChanged("StatusEffects");
            OnPropertyChanged("StatusColor");
        }

        internal void AssignStatus(string description, int duration)
        {
            StatusEffects.Add(new StatusEffect(description, duration));
            NotifyStatusChange();
        }

        protected void SetIncapacitated(bool isDead)
        {
            StatusEffect incapEffect = StatusEffects.Where(x => x.Description.Equals("Incapacitated")).FirstOrDefault();

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

        public double DistanceTo(Point3D target)
        {
            if (!HasVisual)
            {
                return double.MaxValue;
            }
            return Helper3DCalcs.DistanceBetween(FirstVisual.LocationForSave, target);
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

        internal virtual void PerformAutomaticActions()
        {
            if (_currentCharacterActionScript == null)
            {
                _currentCharacterActionScript = new MoveAndMeleeAttackScript();
            }
            _currentCharacterActionScript.PerformAction(this);
        }

        public virtual String CharacterSheetDescription 
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name + "\n");
                foreach (var cur in Traits)
                {
                    sb.Append(cur.ToString()+"\n");
                }
                return sb.ToString();
            }
        }

        internal void ResetIDOfCopy()
        {
            UniqueCharacterID = Guid.NewGuid();
        }

        public String PictureFilePath
        {
            get
            {
                if (HasVisual)
                {
                    return FirstVisual.PictureFileAbsolutePath;
                }
                return String.Empty;
            }
        }
    }
}