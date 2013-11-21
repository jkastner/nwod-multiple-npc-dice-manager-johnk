using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombatAutomationTheater
{
    /// <summary>
    /// Interaction logic for AoEControl.xaml
    /// </summary>
    public partial class AoEControl : Window
    {
        public AoEControl()
        {
            InitializeComponent();
            Loaded += ActivateColumn;
        }

        private void ActivateColumn(object sender, RoutedEventArgs e)
        {
            MakeStatusEffectActive(false);
        }
        private bool _wasCancel;

        public bool WasCancel
        {
            get { return _wasCancel; }
            set { _wasCancel = value; }
        }

        private bool _wasStatusEffect;

        public bool WasStatusEffect
        {
            get { return _wasStatusEffect; }
            set { _wasStatusEffect = value; }
        }
        

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = false;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            WasCancel = true;
            this.Close();
        }

        private void StatusGroup_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)StatusGroup_RadioButton.IsChecked)
            {
                MakeStatusEffectActive(true);
            }
        }


        public String ChosenSave
        {
            get
            {
                if ((bool)Fort_RadioButton.IsChecked)
                {
                    return "Fortitude";
                }
                if ((bool)Ref_RadioButton.IsChecked)
                {
                    return "Reflex";
                }
                else
                {
                    return "Will";
                }
            }
        }

        public double ModOnSuccess
        {
            get
            {
                if ((bool)HalfDamageOnSuccess_RadioButton.IsChecked)
                {
                    return .5;
                }
                return 0;
            }
        }
        public double ModOnFail
        {
            get
            {
                if ((bool)HalfDamageOnFail_RadioButton.IsChecked)
                {
                    return .5;
                }
                return 1;
            }
        }


        private void MakeStatusEffectActive(bool isStatusEffect)
        {
            WasStatusEffect = isStatusEffect;
            Damage_StackPanel.IsEnabled = !isStatusEffect;
            StatusEffect_StackPanel.IsEnabled = isStatusEffect;
            if (isStatusEffect)
            {
                Damage_StackPanel.Background = Brushes.LightGray;
            }
            else
            {
                StatusEffect_StackPanel.Background = Brushes.LightGray;
            }
        }

        private void DamageGroup_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;
            if ((bool)DamageGroup_RadioButton.IsChecked)
            {
                MakeStatusEffectActive(false);
            }
        }




    }
}
