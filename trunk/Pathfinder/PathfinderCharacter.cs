using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XMLCharSheets
{
    public class PathfinderCharacter : CharacterSheet
    {
        public PathfinderCharacter(string name, List<Trait> curTraits):
            base(name, curTraits)
        {
        }

        public override void PopulateCombatTraits()
        {

        }

        public override void RollInitiative()
        {

        }


        public override SolidColorBrush StatusColor
        {
            get
            {
                return new SolidColorBrush(Colors.Blue);
            }
        }
        public override String RollResults
        {
            get;
            set;
        }

        internal override CharacterSheet Copy(string newName)
        {
            return null;
        }

        internal override void DoBashing()
        {

        }
        internal override void DoLethal()
        {

        }
        internal override void DoAggrivated()
        {

        }

        internal override void ResetHealth()
        {

        }

        public override String ChosenAttackValue
        {
            get
            {
                return "";
            }
        }

        internal override List<Damage> AttackTarget(int RollModifier)
        {
            return null;
        }

        internal override DicePool RollBasePool(List<Trait> dicePools, int modifier)
        {
            return null;
        }

        public override bool IsIncapacitated { get; set; }
    }
}
