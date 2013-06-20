using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;
using GameBoard;

namespace XMLCharSheets
{
    public class RosterViewModel : INotifyPropertyChanged
    {
        
        CharacterReader _characterReader = new CharacterReader();

        public RosterViewModel()
        {
            RegisterReaders();
            MakeTeams();
        }

        public void RegisterVisualsViewModel(VisualsViewModel vm)
        {
            vm.PieceMoved += OnVisualPieceMoved;
            vm.PieceSelected += OnVisualPieceSelected;
        }



        private void RegisterReaders()
        {
            _characterReader.RegisterReader("NWoD", new NWoDCharacterReader());
            _characterReader.RegisterReader("Pathfinder", new PathfinderCharacterReader());
        }

        private void OnVisualPieceMoved(object sender, EventArgs e)
        {
            var pieceEvent = e as PieceMovedEventsArg;
            if (pieceEvent != null)
            {
                if (pieceEvent.Mover != null)
                {
                    var matchingChar = ActiveRoster.Where(x => x.Visual != null &&
                        x.Visual.Equals(pieceEvent.Mover)).FirstOrDefault();
                    if (matchingChar != null)
                    {
                        matchingChar.HasMoved = true;
                    }
                }
            }
        }

        private void MakeTeams()
        {
 	        Teams.Add(new Team("Team 1", Colors.Red));
            Teams.Add(new Team("Team 2", Colors.Blue));
            Teams.Add(new Team("Team 3", Colors.White));
            Teams.Add(new Team("Team 4", Colors.Black));
         
            Teams.Add(new Team("Team 5", Colors.Green));
            Teams.Add(new Team("Team 6", Colors.Yellow));
            Teams.Add(new Team("Team 7", Colors.Teal));
            Teams.Add(new Team("Team 8", Colors.Purple));
        }

        private void OnVisualPieceSelected(object sender, EventArgs e)
        {
            var pieceEvent = e as PieceSelectedEventArgs;
            if (pieceEvent != null)
            {
                if (pieceEvent.SelectedPiece != null)
                {
                    var matchingChar = ActiveRoster.Where(x => x.Visual != null &&
                        x.Visual.Equals(pieceEvent.SelectedPiece)).FirstOrDefault();
                    if (matchingChar != null)
                    {
                        SelectedActiveCharacter = matchingChar;
                    }
                }
            }
        }



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

        private ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        public ObservableCollection<Team> Teams
        {
            get { return _teams; }
            set { _teams = value; }
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

        public void ReportText(String newText)
        {
            ResultText = newText;
        }


        public CharacterSheet ReadCharacterFromFile(String fileName)
        {
            return _characterReader.Read(fileName);
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
                    charTraits.Add(involvedCharacter.FindNumericTrait(curTrait));
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
                ResultText = "\n" + result.ToString()+"\n";

            }
            return canRoll;
        }

        private void RollCharacter(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            String allTraits = "";
            for(int curIndex = 0;curIndex<involvedTraits.Count;curIndex++)
            {
                allTraits = allTraits + involvedTraits[curIndex];
                if(curIndex!=involvedTraits.Count-1)
                {
                    allTraits = allTraits +", ";
                }
            }
            
            ResultText = involvedCharacter.Name + " rolled: ("+allTraits+")";
            int totalDice = 0;
            List<Trait> charTraits = new List<Trait>();
            foreach(var cur in involvedTraits)
            {
                charTraits.Add(involvedCharacter.FindNumericTrait(cur));
            }
            String result = involvedCharacter.RollBasePool(charTraits, RollModifier).ResultDescription;
            ResultText = "\n" + result;
        }
    

        internal void DoBashing(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Bashing");
            }
        }


        internal void DoDamage(IList characters, int value, String damageType)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoDamage(value, damageType);
            }
        }

        internal void DoLethal(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Lethal");
            }
        }

        internal void DoAggrivated(IList characters)
        {
            foreach (var curItem in characters)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Aggrivated");
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
                if (curChar.Visual != null)
                {
                    curChar.Visual.RemakeInfoText(MakeStatusList(curChar.StatusEffects));
                }

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
                foreach (var cur in Teams)
                {
                    if (cur.TeamMembers.Contains(curChar))
                        cur.TeamMembers.Remove(curChar);
                }
                foreach (var cur in ActiveRoster)
                {
                    if (cur.Target == curChar)
                    {
                        cur.Target = null;
                    }
                }
                if (curChar.Visual != null)
                {
                    CombatService.VisualsViewModel.RemovePiece(curChar.Visual);
                }
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
            Dictionary<CharacterSheet, List<Damage>> targetToDamage = new Dictionary<CharacterSheet, List<Damage>>();
            foreach (var curItem in attackers)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                List<String> attackName = new List<String>();
                if (curChar.Target == null)
                {
                    ResultText = curChar.Name + " has no target.\n";
                    continue;
                }
                curChar.HasAttacked = true;
                attackName.Add(curChar.ChosenAttack);
                if (CanRoll(curChar, attackName))
                {
                    ResultText = curChar.Name + " rolled attack {" + curChar.ChosenAttackValue + " - "
                       + curChar.ChosenAttackString + "} on " + curChar.Target.Name+"\n";
                       
                    var damageResult = curChar.AttackTarget(RollModifier);
                    if (!targetToDamage.ContainsKey(curChar.Target))
                    {
                        targetToDamage.Add(curChar.Target, new List<Damage>());
                    }
                    if (curChar is NWoDCharacter)
                    {
                        ResultText = "\nFinal pool: " + curChar.FinalAttackPool;
                    }
                    targetToDamage[curChar.Target].AddRange(damageResult);
                   ResultText = "\n"+ curChar.RollResults + "\n";
                }
                if (curChar.Visual != null&&curChar.Target.Visual!=null)
                {
                    CombatService.VisualsViewModel.DrawAttack(curChar.Visual, curChar.Target.Visual, curChar.PieceColor, new Duration(new TimeSpan(0,0,0,5)));
                }
            }

            SummarizeDamage(targetToDamage);
        }

        
        private void SummarizeDamage(Dictionary<CharacterSheet, List<Damage>> targetToDamage)
        {
            ResultText = "\n\nDamage summary:";
            foreach (var curTargetPair in targetToDamage)
            {
                Dictionary<String, int> damageResultsTotal = new Dictionary<string, int>();
                int summedDamage = 0;
                foreach (var cur in curTargetPair.Value)
                {
                    if (!damageResultsTotal.ContainsKey(cur.DamageDescriptor))
                    {
                        damageResultsTotal.Add(cur.DamageDescriptor, 0);
                    }
                    damageResultsTotal[cur.DamageDescriptor] += cur.DamageValue;
                    if (cur.CanBeSummed())
                    {
                        summedDamage += cur.DamageValue;
                    }
                }
                ResultText = "\n\t" + curTargetPair.Key.Name + " took ";
                if (summedDamage > 0)
                {
                    ResultText = summedDamage + " total (";
                }
                foreach (var curDamage in damageResultsTotal)
                {
                    ResultText = curDamage.Key + " " + curDamage.Value + " ";
                }
                if (summedDamage > 0)
                {
                    ResultText = ")";
                }
                ResultText = "\t Current Status: " + curTargetPair.Key.HealthStatusLineDescription;
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


        internal void SetTargets(IList attackers, IList otherTraits, CharacterSheet target, string attackType, List<String> otherAttacks, string damageType)
        {
            List <String> allOtherAttackTraits = new List<string>();
            foreach(var cur in otherTraits)
            {
                allOtherAttackTraits.Add(cur.ToString());
            }
            foreach (var cur in otherAttacks)
            {
                allOtherAttackTraits.Add(cur.ToString());
            }
            foreach (var curItem in attackers)
            {
                CharacterSheet curChar = curItem as CharacterSheet;
                curChar.SetTarget(target, allOtherAttackTraits, attackType, damageType);
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
                var curCharacter = curItem as CharacterSheet;
                curCharacter.AssignStatus(description, duration);
                if (curCharacter.Visual != null)
                {
                    curCharacter.Visual.RemakeInfoText(MakeStatusList(curCharacter.StatusEffects));
                }
            }
        }

        internal void MarkCharactersAsDeceased()
        {
            for (int curIndex = ActiveRoster.Count()-1; curIndex >= 0; curIndex--)
            {
                if (ActiveRoster[curIndex].IsIncapacitated)
                {
                    if (ActiveRoster[curIndex].Visual != null)
                    {
                        CombatService.VisualsViewModel.RemovePiece(ActiveRoster[curIndex].Visual);
                    }
                    foreach (var cur in ActiveRoster.Where(x => x.Target == ActiveRoster[curIndex]))
                    {
                        cur.Target = null;
                    }
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

        internal void SetVisualActive(IList selectedObjects)
        {
            List<CharacterSheet> selectedCharacters = GetListOfCharacters(selectedObjects);
            foreach (var cur in _activeRoster)
            {
                var curCharacter = cur as CharacterSheet;
                if (curCharacter.Visual != null)
                {
                    CombatService.VisualsViewModel.SetInactive(curCharacter.Visual);
                }
            }
            foreach (var curCharacter in selectedCharacters)
            {
                if (curCharacter.Visual != null)
                {
                    CombatService.VisualsViewModel.SetActive(curCharacter.Visual, curCharacter.PieceColor, selectedCharacters.Count == 1, curCharacter.SpeedTrait.TraitValue, MakeStatusList(curCharacter.StatusEffects));
                }
            }
            if (selectedCharacters.Count(x => x.Visual!=null) > 1)
            {
                CombatService.VisualsViewModel.DrawGroupMovementCircle();
            }
        }

        private List<CharacterSheet> GetListOfCharacters(IList selectedObjects)
        {
            List<CharacterSheet> values = new List<CharacterSheet>();
            foreach (var cur in selectedObjects)
            {
                values.Add(cur as CharacterSheet);
            }
            return values;
        }

        internal void AddVisualToCharacters(IList characters, PictureFileInfo pictureInfo, System.Windows.Media.Color pieceColor, Team chosenTeam)
        {
            foreach (var curItem in characters)
            {
                var curCharacter = curItem as CharacterSheet;
                if (curCharacter.Team != null)
                {
                    curCharacter.Team.TeamMembers.Remove(curCharacter);
                    curCharacter.Team = null;
                }
                curCharacter.Team = chosenTeam;
                chosenTeam.TeamMembers.Add(curCharacter);

                Point3D baseOrigin = new Point3D(0, 0, 0);
                if (curCharacter.Visual != null)
                {
                    baseOrigin = curCharacter.Visual.Location;
                    CombatService.VisualsViewModel.RemovePiece(curCharacter.Visual);
                }
                //public MoveablePicture 
                //AddImagePieceToMap(String charImageFile, Color pieceColor, String name, int speed, int height, Point3D location, String additionalInfo)
                var createdVisual = CombatService.VisualsViewModel.AddImagePieceToMap(pictureInfo.PictureFile, pieceColor,
                    curCharacter.Name, curCharacter.HeightTrait.TraitValue, baseOrigin, MakeStatusList(curCharacter.StatusEffects), curCharacter.SpeedTrait.TraitValue);
                curCharacter.Visual = createdVisual;
                curCharacter.PieceColor = pieceColor;
            }
        }

        private List<StatusEffectDisplay> MakeStatusList(List<StatusEffect> list)
        {
            List<StatusEffectDisplay> description = new List<StatusEffectDisplay>();
            foreach (var cur in list)
            {
                description.Add(cur.CreateStatusEffectDisplay());
            }
            return description;
        }

        internal void RemoveVisuals(IList characters)
        {
            foreach (var curItem in characters)
            {
                var curCharacter = curItem as CharacterSheet;
                if (curCharacter.Visual != null)
                {
                    CombatService.VisualsViewModel.RemovePiece(curCharacter.Visual);
                    curCharacter.Visual = null;
                }
            }
        }

        internal void MoveDeceasedToActive()
        {
            foreach (var cur in DeceasedRoster)
            {
                ActiveRoster.Add(cur);
            }
            DeceasedRoster.Clear();
        }

        internal void SetMode(string rulesetName)
        {
            for(int curIndex=FullRoster.Count-1;curIndex>=0;curIndex--)
            {
                var curChar = FullRoster[curIndex];
                if (!curChar.Ruleset.Equals(rulesetName))
                {
                    FullRoster.Remove(curChar);
                }
            }
        }

        internal UIElement ControlFor(string rulesetName)
        {
            return _characterReader.FindControl(rulesetName);
        }

        internal void RegisterNewCharacter(CharacterSheet newInstance)
        {
            ActiveRoster.Add(newInstance);
        }

        internal void PathfinderSingleAttack(IList attackers)
        {
            foreach (var cur in attackers)
            {
                PathfinderCharacter curChar = cur as PathfinderCharacter;
                if (curChar != null)
                {
                    curChar.SingleAttackOnly = true;
                }
            }
            RollAttackTarget(attackers);
            foreach (var cur in attackers)
            {
                PathfinderCharacter curChar = cur as PathfinderCharacter;
                if (curChar != null)
                {
                    curChar.SingleAttackOnly = false;
                }
            }
        }

        internal void OpenRoster(IList<CharacterSheet> newRoster)
        {
            List<CharacterSheet> oldRoster = new List<CharacterSheet>();
            oldRoster.AddRange(ActiveRoster);
            RemoveActiveCharacters(oldRoster);
            foreach (var curNew in newRoster)
            {
                RegisterNewCharacter(curNew);
            }
        }
    }
}
