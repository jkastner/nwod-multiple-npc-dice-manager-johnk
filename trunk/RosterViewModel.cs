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

        private ObservableCollection<CharacterSheet> _deceasedRoster = new ObservableCollection<CharacterSheet>();
        public ObservableCollection<CharacterSheet> DeceasedRoster
        {
            get { return _deceasedRoster; }
            set
            {
                _deceasedRoster = value;
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

        private ObservableCollection<String> _damageTypes = new ObservableCollection<String>();
        public ObservableCollection<String> DamageTypes
        {
            get { return _damageTypes; }
            set
            {
                _damageTypes = value;
            }
        }


        private CharacterSheet _selectedFullCharacter;
        public CharacterSheet SelectedFullCharacter
        {
            get { return _selectedFullCharacter; }
            set
            {
                SetActiveCharacter(value, null, null);
                OnPropertyChanged("SelectedFullCharacter");
            }
        }

        private CharacterSheet _selectedActiveCharacter;
        public CharacterSheet SelectedActiveCharacter
        {
            get { return _selectedActiveCharacter; }
            set
            {
                SetActiveCharacter(null, value, null);
            }
        }

        private CharacterSheet _selectedDeceasedCharacter;
        public CharacterSheet SelectedDeceasedCharacter
        {
            get { return _selectedDeceasedCharacter; }
            set
            {
                SetActiveCharacter(null, null, value);
            }
        }

        private CharacterSheet _selectedCharacter;
        public CharacterSheet SelectedCharacter
        {
            get { return _selectedCharacter; }
        }

        private void SetActiveCharacter(CharacterSheet fullChar, CharacterSheet activeChar, CharacterSheet deceasedCharacter)
        {
            if (activeChar != null)
            {
                _selectedCharacter = activeChar;
            }
            if (fullChar != null)
            {
                _selectedCharacter = fullChar;
            }
            if (deceasedCharacter != null)
            {
                _selectedCharacter = deceasedCharacter;
            }
            _selectedActiveCharacter = activeChar;
            _selectedFullCharacter = fullChar;
            _selectedDeceasedCharacter = deceasedCharacter;
            OnPropertyChanged("SelectedFullCharacter");
            OnPropertyChanged("SelectedActiveCharacter");
            OnPropertyChanged("SelectedDeceasedCharacter");
            OnPropertyChanged("SelectedCharacter");
        }
        
        
        
        internal void PopulateCharacters(String sourceDir)
        {
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(sourceDir);
            foreach (string fileName in fileEntries)
            {
                FullRoster.Add(ReadCharacterFromFile(fileName));
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




        public CharacterSheet ReadCharacterFromFile(String fileName)
        {
            ReadCharacter rc = new ReadCharacter();
            return rc.Read(fileName);
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

        internal void RollCharacters(IList characters, IList selectedTraits)
        {
            ResultText = lineBreak;
            List<String> involvedTraits = new List<String>();
            foreach (var curTraitItem in selectedTraits)
            {
                String curTrait = curTraitItem.ToString();
                involvedTraits.Add(curTrait);
            }
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                if(CanRoll(curChar, involvedTraits))
                    RollCharacter(curChar, involvedTraits);
            }
            
        }

        private bool CanRoll(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            bool canRoll = true;
            List<String> missingTraits = new List<String>();
            List<Trait> charTraits = new List<Trait>();
            foreach (var curTrait in involvedTraits)
            {
                bool hasTrait = involvedCharacter.HasTrait(curTrait);
                if (!hasTrait)
                {
                    missingTraits.Add(curTrait);
                }
                else
                {
                    charTraits.Add(involvedCharacter.FindTrait(curTrait));
                }
                canRoll &= hasTrait;
            }
            if (!canRoll)
            {
                StringBuilder result = new StringBuilder();
                result.Append(involvedCharacter.Name + " was missing traits:");
                for (int curIndex = 0; curIndex < missingTraits.Count(); curIndex++)
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
                ResultText = "\n" + result.ToString();

            }
            return canRoll;
        }

        private void RollCharacter(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            ResultText = involvedCharacter.Name + " rolled: ";
            int totalDice = 0;
            List<Trait> charTraits = new List<Trait>();
            foreach(var cur in involvedTraits)
            {
                charTraits.Add(involvedCharacter.FindTrait(cur));
            }
            String result = involvedCharacter.RollBasePool(charTraits, RollModifier).ResultDescription;
            ResultText = "\n" + result;
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
                curChar.StatusEffects.Clear();
                curChar.NotifyStatusChange();

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

        private String _rollResults;
        public String ResultText
        {
            get { return _rollResults; }
            set {
                _rollResults = _rollResults + value;
                OnPropertyChanged("ResultText");
            }
        }

        String lineBreak = "\n-----------\n";
        internal void RollAttackTarget(IList attackers)
        {
            ResultText = lineBreak;
            
            foreach (var curItem in attackers)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                List<String> attackName = new List<String>();
                if (curChar.Target == null)
                {
                    ResultText = curChar.Name + " has no target.\n";
                    continue;
                }
                attackName.Add(curChar.ChosenAttack);
                if (CanRoll(curChar, attackName))
                {
                    curChar.AttackTarget(RollModifier);
                    ResultText = curChar.Name + " rolled attack {" + curChar.ChosenAttackValue + " - " 
                        + curChar.ChosenAttackString + "} on " + curChar.Target.Name + 
                        "\nFinal pool: "+curChar.FinalAttackPool+"\n"+ curChar.RollResults + "\n";
                    
                }
                if (curChar.Visual != null&&curChar.Target.Visual!=null)
                {
                    curChar.Visual.DrawAttack(curChar.Target.Visual);

                }
            }
        }

        internal void NewRound()
        {
            foreach (CharacterSheet curCharacter in ActiveRoster)
            {
                String info = curCharacter.NewRound();
                if (!String.IsNullOrWhiteSpace(info))
                {
                    ResultText = info;
                }
            }
        }

        internal void RecalculateCombatStats(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.PopulateCombatTraits();
            }
        }

        internal void BloodHeal(IList characters)
        {
            foreach (var curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    var regularChar = curItem as CharacterSheet;
                    ResultText = regularChar.Name + " is not a vampire.";
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                    {
                        if (curVampire.HasHealableWounds())
                            curVampire.BloodHeal();
                        else
                        {
                            ResultText = curVampire.Name + " did not have wounds that could be healed.";
                        }
                    }
                    else
                        ResultText = curVampire.Name + " did not have enough Vitae.";
                }
            }
        }

        internal void BloodBuff(IList characters)
        {
            foreach (var curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    CharacterSheet regularChar = curItem as CharacterSheet;
                    ResultText = regularChar.Name + " is not a vampire.";
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                        curVampire.BloodBuff();
                    else
                        ResultText = curVampire.Name + " did not have enough Vitae.";
                }
            }
        }


        internal void SetTargets(IList attackers, IList otherTraits, CharacterSheet target, string attackType, string damageType)
        {
            List <String> otherAttackTraits = new List<string>();
            foreach(var cur in otherTraits)
            {
                otherAttackTraits.Add(cur.ToString());
            }
            foreach (var curItem in attackers)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.SetTarget(target, otherAttackTraits, attackType, damageType);
            }
        }

        internal void RefillVitae(IList characters)
        {
            foreach (var curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    CharacterSheet regularChar = curItem as CharacterSheet;
                    ResultText = regularChar.Name + " is not a vampire.";
                }
                else
                {
                    curVampire.ResetVitae();
                }
            }
        }

        internal void AssignStatus(IList characters, int duration, string description)
        {
            foreach (var curItem in characters)
            {
                var curCharacer = curItem as CharacterSheet;
                curCharacer.AssignStatus(description, duration);
            }
        }

        internal void MarkCharactersAsDeceased()
        {
            for (int curIndex = ActiveRoster.Count()-1; curIndex >= 0; curIndex--)
            {
                if (ActiveRoster[curIndex].IsIncapacitated)
                {
                    DeceasedRoster.Add(ActiveRoster[curIndex]);
                    ActiveRoster.Remove(ActiveRoster[curIndex]);
                }
            }
            for (int curIndex = DeceasedRoster.Count() - 1; curIndex >= 0; curIndex--)
            {
                if (!DeceasedRoster[curIndex].IsIncapacitated)
                {
                    ActiveRoster.Add(DeceasedRoster[curIndex]);            
                    DeceasedRoster.Remove(DeceasedRoster[curIndex]);
                }
            }
        }

        internal void SetVisualActive(IList characters)
        {
            foreach (var cur in _activeRoster)
            {
                if (cur.Visual != null)
                {
                    cur.Visual.SetActive(false);
                }
            }
            foreach (var curItem in characters)
            {
                var curCharacer = curItem as CharacterSheet;
                curCharacer.Visual.SetActive(true);
            }
        }
    }
}
