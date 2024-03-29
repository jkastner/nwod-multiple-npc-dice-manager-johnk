﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CombatAutomationTheater
{
    /// <summary>
    ///     Interaction logic for PathfinderControl.xaml
    /// </summary>
    public partial class PathfinderControl : UserControl
    {
        private readonly RosterViewModel _viewModel;

        public PathfinderControl()
        {
            InitializeComponent();
            _viewModel = CombatService.RosterViewModel;
            DataContext = _viewModel;
        }

        private void WillSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> { "Will" });
        }


        private void ReflexSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> { "Reflex" });
        }

        private void FortitudeSave_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RollCharacters(ActiveList(), new List<String> { "Fortitude" });
        }

        protected IList ActiveList()
        {
            var parentWindow = Window.GetWindow(this) as MainWindow;
            return parentWindow.ActiveList();
        }


        int _lastSuccessfullyParsedDamage = 0;
        private void DoDamage(int amount)
        {
            _viewModel.DoDamage(ActiveList(), amount, DamageDescriptor_SelectAllTextBox.Text);
        }

        private void DamageBox_SelectAllTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DoDamage(_lastSuccessfullyParsedDamage);
            }
            int newVal = 0;
            if (int.TryParse(DamageValue_SelectAllTextBox.Text, out newVal))
            {
                _lastSuccessfullyParsedDamage = newVal;
            }
        }

        private void DoDamage_ButtonClick(object sender, RoutedEventArgs e)
        {
            DoDamage(_lastSuccessfullyParsedDamage);
        }

        private void HealDamage_ButtonClick(object sender, RoutedEventArgs e)
        {
            DoDamage(-_lastSuccessfullyParsedDamage);
        }



        private static bool IsTextNumeric(string text)
        {
            text = text.Trim();
            var regex = new Regex("[^0-9]"); //regex that matches disallowed text
            bool isGood = !regex.IsMatch(text);
            return isGood;
        }


        private void DamageValue_SelectAllTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isGood = IsTextNumeric(e.Text);
            e.Handled = !isGood;
        }

        // Use the DataObject.Pasting Handler 
        private void DamageValue_SelectAllTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                var text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Pathfinder_SingleAttack_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PathfinderSingleAttack(ActiveList());
        }


        private void RollPool_Button_Click(object sender, RoutedEventArgs e)
        {
            RollPoolFromTextBox();
        }

        private void RollDice_SelectAllTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RollPoolFromTextBox();
            }
            else
            {
                ResetRollDiceTextBoxError();
            }
        }

        private void RollPoolFromTextBox()
        {
            var createdPool = PathfinderDicePool.ParseString(RollDice_SelectAllTextBox.Text);
            if (createdPool == null)
            {
                RollDice_SelectAllTextBox.Background = new SolidColorBrush(Colors.Red);
                RollDice_SelectAllTextBox.ToolTip = "Format must be as in \"quantity d dieType + modifier\" format - '1d8+2'";
                return;
            }
            ResetRollDiceTextBoxError();
            createdPool.Roll();
            DamageValue_SelectAllTextBox.Text = createdPool.TotalValue.ToString();
            _lastSuccessfullyParsedDamage = createdPool.TotalValue;
            TextReporter.Report("\n" + createdPool.PoolDescription + ": " + createdPool.ResultDescription + "\n");
        }

        private void ResetRollDiceTextBoxError()
        {
            RollDice_SelectAllTextBox.Background = new SolidColorBrush(Colors.White);
            RollDice_SelectAllTextBox.ToolTip = "";
        }

        private void AreaOfEffect_Button_Click(object sender, RoutedEventArgs e)
        {
            AoEControl w = new AoEControl();
            w.ShowDialog();
            if (w.WasCancel)
            {
                return;
            }
            List<PathfinderCharacter> _chosenCharacters = new List<PathfinderCharacter>();
            foreach (var cur in ActiveList())
            {
                _chosenCharacters.Add(cur as PathfinderCharacter);
            }
            var chosenSave = w.ChosenSave;
            int dc, originalDamage;
            bool validDamage = true;
            double modOnSuccess = w.ModOnSuccess;
            double modOnFail = w.ModOnFail;
            validDamage &= int.TryParse(w.DC_TextBox.Text, out dc);
            validDamage &= int.TryParse(w.Damage_TextBox.Text, out originalDamage);
            int durationRounds = 0;
            String desc = w.StatusDescription.Text;
            String duration = w.StatusDuration.Text;
            bool validStatus = int.TryParse(duration, out durationRounds);


            foreach (var cur in _chosenCharacters)
            {
                double damageDouble = originalDamage;
                var curTrait = cur.FindNumericTrait(chosenSave);
                PathfinderDicePool savePool = new PathfinderDicePool(1, 20, 0);
                savePool.Roll();
                if (!validDamage && !w.WasStatusEffect)
                {
                    continue;
                }
                if (!validStatus && w.WasStatusEffect)
                {
                    continue;
                }
                TextReporter.Report(cur.Name + " rolled " + chosenSave + ": " + savePool.TotalValue + " VS DC: " + dc);
                bool madeSave = savePool.TotalValue+curTrait.TraitValue >= dc;
                if (savePool.TotalValue == 20)
                {
                    madeSave = true;
                }
                if (savePool.TotalValue == 1)
                {
                    madeSave = false;
                }
                if (madeSave)
                {
                    TextReporter.Report("--Success--", Brushes.Green);
                }
                else
                {
                    TextReporter.Report("--Failure--", Brushes.Red);
                }
                if (w.WasStatusEffect && !madeSave)
                {
                    cur.AssignStatus(desc, durationRounds);
                }
                if(!w.WasStatusEffect)
                {

                    int damageInt;
                    if (madeSave)
                    {
                        damageDouble = Math.Floor(damageDouble * modOnSuccess);
                        damageInt = (int)(damageDouble);
                        TextReporter.Report(damageInt + " damage.");

                    }
                    else
                    {
                        damageDouble = Math.Floor(damageDouble * modOnFail);
                        damageInt = (int)(damageDouble);
                        TextReporter.Report(damageInt + " damage.");
                    }
                    cur.DoDamage(damageInt, "Area Damage");
                }
                TextReporter.Report("\n");

            }
        }



    }
}
