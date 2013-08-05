using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GameBoard;

namespace XMLCharSheets
{
    public class RosterViewModel : INotifyPropertyChanged
    {
        private readonly CharacterReader _characterReader = new CharacterReader();
        private ObservableCollection<CharacterSheet> _activeRoster = new ObservableCollection<CharacterSheet>();
        private ObservableCollection<String> _currentTraits = new ObservableCollection<String>();
        private ObservableCollection<String> _damageTypes = new ObservableCollection<String>();
        private ObservableCollection<CharacterSheet> _deceasedRoster = new ObservableCollection<CharacterSheet>();


        private ObservableCollection<CharacterSheet> _fullRoster = new ObservableCollection<CharacterSheet>();
        private String _resultText;
        private int _rollModifier;
        private bool _ruleSetChosen;
        private CharacterSheet _selectedActiveCharacter;
        private CharacterSheet _selectedCharacter;
        private CharacterSheet _selectedDeceasedCharacter;
        private CharacterSheet _selectedFullCharacter;
        private ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        private String lineBreak = "\n-----------\n";

        public RosterViewModel()
        {
            RegisterReaders();
            MakeTeams();
        }

        public ObservableCollection<CharacterSheet> FullRoster
        {
            get { return _fullRoster; }
            set { _fullRoster = value; }
        }

        public ObservableCollection<CharacterSheet> ActiveRoster
        {
            get { return _activeRoster; }
            set { _activeRoster = value; }
        }

        public ObservableCollection<CharacterSheet> DeceasedRoster
        {
            get { return _deceasedRoster; }
            set { _deceasedRoster = value; }
        }

        public ObservableCollection<Team> Teams
        {
            get { return _teams; }
            set { _teams = value; }
        }


        public ObservableCollection<String> CurrentTraits
        {
            get { return _currentTraits; }
            set { _currentTraits = value; }
        }

        public ObservableCollection<String> DamageTypes
        {
            get { return _damageTypes; }
            set { _damageTypes = value; }
        }


        public CharacterSheet SelectedFullCharacter
        {
            get { return _selectedFullCharacter; }
            set { SetActiveCharacter(value, null, null); }
        }

        public CharacterSheet SelectedActiveCharacter
        {
            get { return _selectedActiveCharacter; }
            set { SetActiveCharacter(null, value, null); }
        }

        public CharacterSheet SelectedDeceasedCharacter
        {
            get { return _selectedDeceasedCharacter; }
            set { SetActiveCharacter(null, null, value); }
        }

        public CharacterSheet SelectedCharacter
        {
            get { return _selectedCharacter; }
        }

        public int RollModifier
        {
            get { return _rollModifier; }
            set
            {
                _rollModifier = value;
                OnPropertyChanged("RollModifier");
            }
        }

        public String ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = _resultText + value;
                OnPropertyChanged("ResultText");
            }
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
                    CharacterSheet matchingChar = ActiveRoster.Where(x => x.Visual != null &&
                                                                          x.Visual.Equals(pieceEvent.Mover))
                                                              .FirstOrDefault();
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
                    CharacterSheet matchingChar = ActiveRoster.Where(x => x.Visual != null &&
                                                                          x.Visual.Equals(pieceEvent.SelectedPiece))
                                                              .FirstOrDefault();
                    if (matchingChar != null)
                    {
                        SelectedActiveCharacter = matchingChar;
                    }
                }
            }
        }

        private void SetActiveCharacter(CharacterSheet fullChar, CharacterSheet activeChar,
                                        CharacterSheet deceasedCharacter)
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


        public void ReportText(String newText)
        {
            ResultText = newText;
        }


        public CharacterSheet ReadCharacterFromFile(String fileName)
        {
            return _characterReader.Read(fileName);
        }

        internal void RollCharacters(IList characters, IList selectedTraits)
        {
            ResultText = lineBreak;
            var involvedTraits = new List<String>();
            foreach (object curTraitItem in selectedTraits)
            {
                String curTrait = curTraitItem.ToString();
                involvedTraits.Add(curTrait);
            }
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                if (CanRoll(curChar, involvedTraits))
                    RollCharacter(curChar, involvedTraits);
            }
        }

        private bool CanRoll(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            bool canRoll = true;
            var missingTraits = new List<String>();
            var charTraits = new List<Trait>();
            foreach (string curTrait in involvedTraits)
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
                var result = new StringBuilder();
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
                ResultText = "\n" + result + "\n";
            }
            return canRoll;
        }

        private void RollCharacter(CharacterSheet involvedCharacter, List<string> involvedTraits)
        {
            String allTraits = "";
            for (int curIndex = 0; curIndex < involvedTraits.Count; curIndex++)
            {
                allTraits = allTraits + involvedTraits[curIndex];
                if (curIndex != involvedTraits.Count - 1)
                {
                    allTraits = allTraits + ", ";
                }
            }

            ResultText = involvedCharacter.Name + " rolled: (" + allTraits + ")";
            int totalDice = 0;
            var charTraits = new List<Trait>();
            foreach (string cur in involvedTraits)
            {
                charTraits.Add(involvedCharacter.FindNumericTrait(cur));
            }
            String result = involvedCharacter.RollBasePool(charTraits, RollModifier).ResultDescription;
            ResultText = "\n" + result;
        }

        internal void DoDamage(IList characters, int value, String damageType)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.DoDamage(value, damageType);
            }
        }


        internal void ResetHealth(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
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
            foreach (CharacterSheet curChar in ActiveRoster)
            {
                curChar.RollInitiative();
            }
            List<CharacterSheet> q = ActiveRoster.OrderByDescending(x => x.CurInitiative).ToList();
            ActiveRoster.Clear();
            foreach (CharacterSheet curChar in q)
            {
                ActiveRoster.Add(curChar);
            }
        }

        internal void RemoveCharactersFromRosters(IList characters)
        {
            for (int curIndex = characters.Count - 1; curIndex >= 0; curIndex--)
            {
                var curChar = characters[curIndex] as CharacterSheet;
                if(ActiveRoster.Contains(curChar))
                {
                    ActiveRoster.Remove(curChar);
                }
                if (DeceasedRoster.Contains(curChar))
                {
                    DeceasedRoster.Remove(curChar);
                }
                foreach (Team cur in Teams)
                {
                    if (cur.TeamMembers.Contains(curChar))
                        cur.TeamMembers.Remove(curChar);
                }
                foreach (CharacterSheet cur in ActiveRoster)
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

        internal void PerformAutomaticActions(IList actors)
        {
            List<CharacterSheet> activeActors = new List<CharacterSheet>();
            foreach (object curItem in actors)
            {
                var curChar = curItem as CharacterSheet;
                activeActors.Add(curChar);
            }
            activeActors = activeActors.OrderByDescending(x => x.CurInitiative).ToList();
            foreach (var cur in activeActors)
            {
                cur.PerformAutomaticActions();
            }
        }

        internal void RollAttackTarget(IList attackers)
        {
            ResultText = lineBreak;
            var targetToDamage = new Dictionary<CharacterSheet, List<Damage>>();
            foreach (object curItem in attackers)
            {
                var curChar = curItem as CharacterSheet;
                var attackName = new List<String>();
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
                                 + curChar.ChosenAttackString + "} on " + curChar.Target.Name + "\n";

                    List<Damage> damageResult = curChar.AttackTarget(RollModifier);
                    if (!targetToDamage.ContainsKey(curChar.Target))
                    {
                        targetToDamage.Add(curChar.Target, new List<Damage>());
                    }
                    if (curChar is NWoDCharacter)
                    {
                        ResultText = "\nFinal pool: " + curChar.FinalAttackPool;
                    }
                    targetToDamage[curChar.Target].AddRange(damageResult);
                    ResultText = "\n" + curChar.RollResults + "\n";
                }
                if (curChar.Visual != null && curChar.Target.Visual != null)
                {
                    CombatService.VisualsViewModel.DrawAttack(curChar.Visual, curChar.Target.Visual, curChar.PieceColor,
                                                              new Duration(new TimeSpan(0, 0, 0, 5)));
                }
            }

            SummarizeDamage(targetToDamage);
        }


        private void SummarizeDamage(Dictionary<CharacterSheet, List<Damage>> targetToDamage)
        {
            ResultText = "\n\nDamage summary:";
            foreach (var curTargetPair in targetToDamage)
            {
                var damageResultsTotal = new Dictionary<string, int>();
                int summedDamage = 0;
                foreach (Damage cur in curTargetPair.Value)
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
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.PopulateCombatTraits();
            }
        }

        internal void SetTargets(IList attackers, IList otherTraits, CharacterSheet target, string attackType,
                                 List<String> otherAttacks, string damageType)
        {
            var allOtherAttackTraits = new List<string>();
            foreach (object cur in otherTraits)
            {
                allOtherAttackTraits.Add(cur.ToString());
            }
            foreach (string cur in otherAttacks)
            {
                allOtherAttackTraits.Add(cur);
            }
            foreach (object curItem in attackers)
            {
                var curChar = curItem as CharacterSheet;
                curChar.SetTarget(target, allOtherAttackTraits, attackType, damageType);
            }
        }


        internal void AssignStatus(IList characters, int duration, string description)
        {
            foreach (object curItem in characters)
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
            for (int curIndex = ActiveRoster.Count() - 1; curIndex >= 0; curIndex--)
            {
                if (ActiveRoster[curIndex].IsIncapacitated)
                {
                    if (ActiveRoster[curIndex].Visual != null)
                    {
                        CombatService.VisualsViewModel.RemovePiece(ActiveRoster[curIndex].Visual);
                    }
                    foreach (CharacterSheet cur in ActiveRoster.Where(x => x.Target == ActiveRoster[curIndex]))
                    {
                        cur.Target = null;
                    }
                    MoveCharacterToDeceasedRoster(ActiveRoster[curIndex]);
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

        private void MoveCharacterToDeceasedRoster(CharacterSheet characterSheet)
        {
            DeceasedRoster.Add(characterSheet);
            ActiveRoster.Remove(characterSheet);
        }

        internal void SetVisualActive(IList selectedObjects)
        {
            List<CharacterSheet> selectedCharacters = GetListOfCharacters(selectedObjects);
            foreach (CharacterSheet cur in _activeRoster)
            {
                CharacterSheet curCharacter = cur;
                if (curCharacter.Visual != null)
                {
                    CombatService.VisualsViewModel.SetInactive(curCharacter.Visual);
                }
            }
            foreach (CharacterSheet curCharacter in selectedCharacters)
            {
                if (curCharacter.Visual != null)
                {
                    CombatService.VisualsViewModel.SetActive(curCharacter.Visual, curCharacter.PieceColor,
                                                             selectedCharacters.Count == 1,
                                                             curCharacter.SpeedTrait.TraitValue,
                                                             MakeStatusList(curCharacter.StatusEffects));
                }
            }
            if (selectedCharacters.Count(x => x.Visual != null) > 1)
            {
                CombatService.VisualsViewModel.DrawGroupMovementCircle();
            }
        }

        private List<CharacterSheet> GetListOfCharacters(IList selectedObjects)
        {
            var values = new List<CharacterSheet>();
            foreach (object cur in selectedObjects)
            {
                values.Add(cur as CharacterSheet);
            }
            return values;
        }

        internal void AddVisualToCharacters(IList characters, PictureFileInfo pictureInfo, Color pieceColor,
                                            Team chosenTeam)
        {
            foreach (object curItem in characters)
            {
                var curCharacter = curItem as CharacterSheet;
                if (curCharacter.Team != null)
                {
                    curCharacter.Team.TeamMembers.Remove(curCharacter);
                    curCharacter.Team = null;
                }
                curCharacter.Team = chosenTeam;
                chosenTeam.TeamMembers.Add(curCharacter);

                var baseOrigin = new Point3D(0, 0, 0);
                if (curCharacter.Visual != null)
                {
                    baseOrigin = curCharacter.Visual.Location;
                    CombatService.VisualsViewModel.RemovePiece(curCharacter.Visual);
                }
                //public MoveablePicture 
                //AddImagePieceToMap(String charImageFile, Color pieceColor, String name, int speed, int height, Point3D location, String additionalInfo)
                MoveablePicture createdVisual =
                    CombatService.VisualsViewModel.AddImagePieceToMap(pictureInfo.PictureFile, pieceColor,
                                                                      curCharacter.Name,
                                                                      curCharacter.HeightTrait.TraitValue, baseOrigin,
                                                                      MakeStatusList(curCharacter.StatusEffects),
                                                                      curCharacter.SpeedTrait.TraitValue);
                curCharacter.Visual = createdVisual;
                curCharacter.PieceColor = pieceColor;
            }
        }

        private List<StatusEffectDisplay> MakeStatusList(List<StatusEffect> list)
        {
            var description = new List<StatusEffectDisplay>();
            foreach (StatusEffect cur in list)
            {
                description.Add(cur.CreateStatusEffectDisplay());
            }
            return description;
        }

        internal void RemoveVisuals(IList characters)
        {
            foreach (object curItem in characters)
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
            foreach (CharacterSheet cur in DeceasedRoster)
            {
                ActiveRoster.Add(cur);
            }
            DeceasedRoster.Clear();
        }

        internal void SetMode(string rulesetName)
        {
            for (int curIndex = FullRoster.Count - 1; curIndex >= 0; curIndex--)
            {
                CharacterSheet curChar = FullRoster[curIndex];
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
            if (!_ruleSetChosen)
            {
                SetMode(newInstance.Ruleset);
                OnRulesetSelected(new RulesetSelectedEventArgs(newInstance.Ruleset));
                _ruleSetChosen = true;
            }
        }

        public event EventHandler RulesetSelected;

        protected virtual void OnRulesetSelected(RulesetSelectedEventArgs e)
        {
            EventHandler handler = RulesetSelected;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal void PathfinderSingleAttack(IList attackers)
        {
            foreach (object cur in attackers)
            {
                var curChar = cur as PathfinderCharacter;
                if (curChar != null)
                {
                    curChar.SingleAttackOnly = true;
                }
            }
            RollAttackTarget(attackers);
            foreach (object cur in attackers)
            {
                var curChar = cur as PathfinderCharacter;
                if (curChar != null)
                {
                    curChar.SingleAttackOnly = false;
                }
            }
        }

        internal void OpenActiveRoster(IList<CharacterSheet> newRoster)
        {
            var oldRoster = new List<CharacterSheet>();
            oldRoster.AddRange(ActiveRoster);
            RemoveCharactersFromRosters(oldRoster);
            foreach (CharacterSheet curNew in newRoster)
            {
                RegisterNewCharacter(curNew);
            }
        }

        internal void OpenDeceasedRoster(IList<CharacterSheet> deceasedRoster)
        {
            var oldRoster = new List<CharacterSheet>();
            oldRoster.AddRange(DeceasedRoster);
            RemoveCharactersFromRosters(oldRoster);
            foreach (CharacterSheet curNew in deceasedRoster)
            {
                RegisterNewCharacter(curNew);
                MoveCharacterToDeceasedRoster(curNew);
            }
        }

        internal void OpenTeams(IList<Team> newteams)
        {
            Teams.Clear();
            foreach (Team curNew in newteams)
            {
                Teams.Add(curNew);
            }
        }

        internal void ClearResultText()
        {
            _resultText = "";
            OnPropertyChanged("ResultText");
        }

        internal void LoadDamageFor(string rulesetName)
        {
            List<string> types = _characterReader.LoadDamageFor(rulesetName);
            foreach (string cur in types)
            {
                DamageTypes.Add(cur);
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




    }
}