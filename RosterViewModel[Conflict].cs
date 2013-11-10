﻿using System;
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
            set 
            {
                SetActiveCharacter(value, null, null);
            }
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

        private void RegisterReaders()
        {
            _characterReader.RegisterReader("NWoD", new NWoDCharacterReader());
            _characterReader.RegisterReader("Pathfinder", new PathfinderCharacterReader());
        }

        public const string UnassignedTeamName = "Unassigned";
        public static readonly Team UnassignedTeam = new Team(UnassignedTeamName, Colors.Beige); 
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
            Teams.Add(UnassignedTeam);
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

        public List<Tuple<String, String>> LoadingErrors = new List<Tuple<string, string>>();
        public CharacterSheet ReadCharacterFromFile(String fileName)
        {
            return _characterReader.Read(fileName);
        }

        internal void RollCharacters(IList characters, IList selectedTraits)
        {
            TextReporter.Report(lineBreak);
            var involvedTraits = new List<String>();
            foreach (object cur in selectedTraits)
            {
                var curTrait = cur as Trait;
                String curTraitLabel = curTrait.TraitLabel;
                involvedTraits.Add(curTraitLabel);
            }
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                if (CanRoll(curChar, involvedTraits))
                {
                    RollCharacter(curChar, involvedTraits);
                    TextReporter.Report("\n");
                }

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
                TextReporter.Report("\n" + result + "\n");
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

            TextReporter.Report(involvedCharacter.Name + " rolled (" + allTraits + ")");
            int totalDice = 0;
            var charTraits = new List<Trait>();
            foreach (string cur in involvedTraits)
            {
                charTraits.Add(involvedCharacter.FindNumericTrait(cur));
            }
            String result = involvedCharacter.RollBasePool(charTraits, RollModifier).ResultDescription;
            TextReporter.Report(": " + result);
        }

        internal void DoDamage(IList characters, int value, String damageType)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.DoDamage(value, damageType);
                TextReporter.Report("\n"+curChar.Name+" took ");
                TextReporter.Report(value+" ", TextReporter.DamageBrush);
                TextReporter.Report(damageType + "\n" + curChar.Name+": "+curChar.HealthStatusLineDescription);
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
                if (curChar.HasVisual)
                {
                    var statuses = MakeStatusList(curChar.StatusEffects);
                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.RemakeInfoTextFor(curChar.UniqueCharacterID, statuses));
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
                if (curChar.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.RemovePiece(curChar.UniqueCharacterID));
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
            TextReporter.Report(lineBreak);
            var targetToDamage = new Dictionary<CharacterSheet, List<Damage>>();
            foreach (object curItem in attackers)
            {
                var curChar = curItem as CharacterSheet;
                var attackName = new List<String>();
                if (curChar.Target == null)
                {
                    TextReporter.Report(curChar.Name + " has no target.\n");
                    continue;
                }
                curChar.HasAttacked = true;
                attackName.Add(curChar.ChosenAttack);
                if (CanRoll(curChar, attackName))
                {
                    TextReporter.Report(curChar.Name + " rolled attack {" + curChar.ChosenAttackValue + " - "
                                 + curChar.ChosenAttackString + "} on " + curChar.Target.Name + "\n");

                    List<Damage> damageResult = curChar.AttackTarget(RollModifier);
                    if (!targetToDamage.ContainsKey(curChar.Target))
                    {
                        targetToDamage.Add(curChar.Target, new List<Damage>());
                    }
                    if (curChar is NWoDCharacter)
                    {
                        TextReporter.Report("\nFinal pool: " + curChar.FinalAttackPool);
                    }
                    targetToDamage[curChar.Target].AddRange(damageResult);
                    TextReporter.Report("\n" + curChar.RollResults + "\n");
                }
                if (curChar.HasVisual && curChar.Target.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.DrawAttack(curChar.UniqueCharacterID, curChar.Target.UniqueCharacterID, curChar.Team.TeamColor,
                                                              new Duration(new TimeSpan(0, 0, 0, 5))));
                }
            }

            SummarizeDamage(targetToDamage);
        }


        private void SummarizeDamage(Dictionary<CharacterSheet, List<Damage>> targetToDamage)
        {
            TextReporter.Report("\n\nDamage summary:");
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
                TextReporter.Report("\n\t" + curTargetPair.Key.Name + " took ");
                if (summedDamage > 0)
                {
                    TextReporter.Report(summedDamage + " total (");
                }
                foreach (var curDamage in damageResultsTotal)
                {
                    TextReporter.Report(curDamage.Key + " " + curDamage.Value + " ", TextReporter.DamageBrush);
                }
                if (summedDamage > 0)
                {
                    TextReporter.Report(")");
                }
                TextReporter.Report("\t Current Status: " + curTargetPair.Key.HealthStatusLineDescription);
            }
        }


        internal void NewRound()
        {
            foreach (CharacterSheet curCharacter in ActiveRoster)
            {
                String info = curCharacter.NewRound();
                if (!String.IsNullOrWhiteSpace(info))
                {
                    TextReporter.Report(info);
                }
            }
            if (AutoSaveEachTurn)
            {
                FileSaveOpenService.AutoSave("R_"+CurrentRound.ToString());
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
                if (curCharacter.HasVisual)
                {
                    var statusList = MakeStatusList(curCharacter.StatusEffects);
                    VisualsService.BoardsViewModel.ForeachBoard(
                        x=>x.VisualsViewModel.RemakeInfoTextFor(curCharacter.UniqueCharacterID, statusList));
                }
            }
        }

        internal void MarkCharactersAsDeceased()
        {
            for (int curIndex = ActiveRoster.Count() - 1; curIndex >= 0; curIndex--)
            {
                if (ActiveRoster[curIndex].IsIncapacitated)
                {
                    if (ActiveRoster[curIndex].HasVisual)
                    {
                        var curId = ActiveRoster[curIndex].UniqueCharacterID;
                        VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.RemovePiece(curId));
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

        internal void SetActive(IList selectedObjects)
        {
            List<CharacterSheet> selectedCharacters = GetListOfCharacters(selectedObjects);
            foreach (var cur in selectedCharacters)
            {
                cur.IsSelected = true;
            }
            foreach (CharacterSheet cur in _activeRoster)
            {
                CharacterSheet curCharacter = cur;
                if (curCharacter.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetInactive(curCharacter.UniqueCharacterID));
                }
            }
            foreach (CharacterSheet curCharacter in selectedCharacters)
            {
                if (curCharacter.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.SetActive(curCharacter.UniqueCharacterID, 
                        curCharacter.Team.TeamColor,
                        selectedCharacters.Count == 1,
                        curCharacter.SpeedTrait.TraitValue,
                        MakeStatusList(curCharacter.StatusEffects)));
                }
            }
            if (selectedCharacters.Count(x => x.HasVisual) > 1)
            {
                VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.DrawGroupMovementCircle());
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

        internal void AddVisualToCharacters(IList characters, PictureFileInfo pictureInfo, Team chosenTeam)
        {
            foreach (object curItem in characters)
            {
                var curCharacter = curItem as CharacterSheet;
                if (curCharacter.Team != null)
                {
                    curCharacter.Team.TeamMembers.Remove(curCharacter);
                    curCharacter.Team = null;
                }
                RegisterTeamMemberOnTeam(curCharacter, chosenTeam);

                var baseOrigin = new Point3D(0, 0, 0);
                if (curCharacter.HasVisual)
                {
                    baseOrigin = curCharacter.FirstVisual.Location;
                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.RemovePiece(curCharacter.UniqueCharacterID));
                }
                //public MoveablePicture 
                //AddImagePieceToMap(String charImageFile, Color pieceColor, String name, int speed, int height, Point3D location, String additionalInfo)
                VisualsService.BoardsViewModel.ForeachBoard(x=>x.VisualsViewModel.AddImagePieceToMap(pictureInfo.PictureFile, chosenTeam.TeamColor,
                                                                      curCharacter.Name,
                                                                      curCharacter.HeightTrait.TraitValue, baseOrigin,
                                                                      MakeStatusList(curCharacter.StatusEffects),
                                                                      curCharacter.SpeedTrait.TraitValue, curCharacter.UniqueCharacterID));
            }
        }

        internal void RegisterTeamMemberOnTeam(CharacterSheet curCharacter, Team chosenTeam)
        {
            curCharacter.Team = chosenTeam;
            chosenTeam.TeamMembers.Add(curCharacter);
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
                if (curCharacter.HasVisual)
                {
                    VisualsService.BoardsViewModel.ForeachBoard(x => x.VisualsViewModel.RemovePiece(curCharacter.UniqueCharacterID));
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

        internal void SetRulesetMode(string rulesetName)
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
                SetRulesetMode(newInstance.Ruleset);
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


        internal void ReportText(ReportTextEventArgs reportTextEventArgs)
        {
            OnReportTextEvent(reportTextEventArgs);
        }
        
        public event EventHandler ReportTextEvent;
        private void OnReportTextEvent(ReportTextEventArgs e)
        {
            EventHandler handler = ReportTextEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ClearReportTextEvent;
        private void OnClearReportTextEvent(EventArgs e)
        {
            EventHandler handler = ClearReportTextEvent;
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
            OnClearReportTextEvent(null);
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


        internal void ShowErrors()
        {
            if (LoadingErrors.Any())
            {
                LoadingErrorsWindow lew = new LoadingErrorsWindow();
                lew.SetErrors(LoadingErrors);
                lew.ShowDialog();
            }
        }


        private bool _autoSaveEachTurn;

        public bool AutoSaveEachTurn
        {
            get
            {
                return _autoSaveEachTurn;
            }
            set
            {
                _autoSaveEachTurn = value;
                OnPropertyChanged("AutoSaveEachTurn");
            }
        }


        private int _currentRound = 0;
        public int CurrentRound
        {
            get { return _currentRound; }
            set { _currentRound = value; }
        }
        
    }
}