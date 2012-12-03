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



        private enum PersonType
        {
            Person, Vampire
        }
        public CharacterSheet ReadCharacterFromFile(String fileName)
        {
            CharacterSheet curChar = null;
            try
            {
                PersonType curType = PersonType.Person;
                XmlTextReader reader = new XmlTextReader(fileName);
                List<Trait> curTraits = new List<Trait>();
                while (reader.Read())
                {
                    String curReader = reader.Name.ToLower().Trim();
                    if (curReader.Equals("charactertype") && reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        reader.Read();
                        switch(reader.Value.ToLower().Trim())
                        {
                            case "vampire":
                                curType = PersonType.Vampire;
                                break;
                            case "human":
                            default:
                                curType = PersonType.Person;
                                break;

                        }
                    }

                    if (curReader.Equals("name") && reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        reader.Read();
                        if(curType == PersonType.Vampire)
                            curChar = new NWoDVampire(reader.Value, curTraits);
                        else
                            curChar = new NWoDCharacter(reader.Value, curTraits);

                    }
                    if (curReader.Equals("trait"))
                    {
                        String label = reader.GetAttribute("label");
                        int value = Convert.ToInt32(reader.GetAttribute("value"));
                        String targetDefense = reader.GetAttribute("TargetDefense");
                        String damageType = reader.GetAttribute("DamageType");
                        if (targetDefense != null)
                        {
                            AttackTrait curAttack = new AttackTrait(value, label, targetDefense, damageType);
                            curTraits.Add(curAttack);
                        }
                        else
                        {
                            NumberedTrait curNumberedTrait = new NumberedTrait(value, label);
                            curTraits.Add(curNumberedTrait);
                        }
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

        internal void RollCharacters(IList characters, IList selectedTraits)
        {
            RollResults = lineBreak;
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
                RollResults = "\n" + result.ToString();

            }
            return canRoll;
        }

        private void RollCharacter(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            RollResults = involvedCharacter.Name + " rolled {";
            int totalDice = 0;
            List<Trait> charTraits = new List<Trait>();
            foreach(var cur in involvedTraits)
            {
                charTraits.Add(involvedCharacter.FindTrait(cur));
            }
            foreach (Trait curTrait in charTraits)
            {
                totalDice += curTrait.TraitValue;
                RollResults = "{" + curTrait.TraitLabel + "}" + " ";
            }
            totalDice += RollModifier;
            String result = involvedCharacter.RollBasePool(totalDice);
            RollResults = "}\n" + result;
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

        public String RollResults
        {
            get { return _rollResults; }
            set {
                _rollResults = _rollResults + value;
                OnPropertyChanged("RollResults");
            }
        }

        String lineBreak = "\n-----------\n";
        internal void RollAttackTarget(IList attackers)
        {
            RollResults = lineBreak;
            foreach (var curItem in attackers)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                List<String> attackName = new List<String>();
                if (curChar.Target == null)
                {
                    RollResults = curChar.Name + " has no target.\n";
                    continue;
                }
                attackName.Add(curChar.ChosenAttack);
                if (CanRoll(curChar, attackName))
                {
                    curChar.AttackTarget(RollModifier);
                    RollResults = curChar.Name + " rolled attack {" + curChar.ChosenAttackString + "}: " + curChar.RollResults + "\n";
                }
            }
        }

        internal void NewRound()
        {
            foreach (CharacterSheet curCharacter in ActiveRoster)
            {
                curCharacter.NewRound();
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
                    RollResults = regularChar.Name + " is not a vampire.";
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                    {
                        if (curVampire.HasHealableWounds())
                            curVampire.BloodHeal();
                        else
                        {
                            RollResults = curVampire.Name + " did not have wounds that could be healed.";
                        }
                    }
                    else
                        RollResults = curVampire.Name + " did not have enough Vitae.";
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
                    RollResults = regularChar.Name + " is not a vampire.";
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                        curVampire.BloodBuff();
                    else
                        RollResults = curVampire.Name + " did not have enough Vitae.";
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
                    RollResults = regularChar.Name + " is not a vampire.";
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
    }
}
