using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XMLCharSheets
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


        private void DoDamage()
        {
            int sliderValue = -(int)DamageSlider.Value;
            _viewModel.DoDamage(ActiveList(), sliderValue, DamageDescriptor_TextBox.Text);
        }

        private void DamageBox_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                DoDamage();
            }
            int newVal = 0;
            if (int.TryParse(DamageValue_TextBox.Text, out newVal))
            {
                DamageSlider.Value = newVal;
            }
        }

        private void Pathfinder_SingleAttack_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PathfinderSingleAttack(ActiveList());
        }

        private void RollDice_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var createdPool = PathfinderDicePool.ParseString(RollDice_TextBox.Text);
                if (createdPool == null)
                {
                    RollDice_TextBox.Background = new SolidColorBrush(Colors.Red);
                    RollDice_TextBox.ToolTip = "Format must be as in \"quantity d dieType + modifier\" format - '1d8+2'";
                    return;
                }
                ResetRollDiceTextBoxError();
                createdPool.Roll();
                DamageValue_TextBox.Text = "-" + createdPool.TotalValue;
                TextReporter.Report("\n" + createdPool.PoolDescription + ": " + createdPool.ResultDescription + "\n");
            }
            else
            {
                ResetRollDiceTextBoxError();
            }
        }

        private void ResetRollDiceTextBoxError()
        {
            RollDice_TextBox.Background = new SolidColorBrush(Colors.White);
            RollDice_TextBox.ToolTip = "";
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
                PathfinderDicePool savePool = new PathfinderDicePool(1, 20, curTrait.TraitValue);
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
                bool madeSave = savePool.TotalValue >= dc;
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
