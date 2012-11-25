using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    class NWoDCharacter : CharacterSheet
    {


        private List<HealthBox> _healthTrack = new List<HealthBox>();
        public List<HealthBox> HealthTrack
        {
            get { return _healthTrack; }
            set { _healthTrack = value; }
        }


        public NWoDCharacter(string characterName, List<NumberedTrait> traits)
            :base(characterName, traits)
        {
        }
        
        
        protected override void PopulateCombatTraits()
        {
            foreach (NumberedTrait curTrait in NumberedTraits)
            {
                switch (curTrait.TraitLabel)
                {
                    case "Health":
                        InitializeHealthBoxes(curTrait.TraitValue);
                        break;
                    case "Initiative":
                        Initiative = curTrait.TraitValue;
                        break;
                }
            }

        }

        public override string Status
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Status:\n");
                sb.Append("Health: ");
                sb.Append(BuildHealthString());
                return sb.ToString();
            }
            set
            {
                //Do Nothing

            }
        }

        private string BuildHealthString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (HealthBox curBox in HealthTrack)
            {
                sb.Append(curBox.ToBoxString()+" ");
            }
            return sb.ToString().Trim();
        }
        

        private void InitializeHealthBoxes(int p)
        {
            for (int curIndex = 0; curIndex < p; curIndex++)
            {
                HealthTrack.Add(new HealthBox());
            }
        }

        internal override string Roll(int totalDice)
        {
            NWoDRollDice curPool = new NWoDRollDice(totalDice);
            curPool.Roll();
            return " {" + curPool.NumberOfDice + "}: " + curPool.CurrentSuccesses + " successes.\nResults: " + curPool.ResultDescription;
        }

        internal override void DoBashing()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.HealthBoxType.Bashing;
            AddDamageBox(newDamage);

        }
        internal override void DoLethal()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.HealthBoxType.Lethal;
            AddDamageBox(newDamage);
        }
        internal override void DoAggrivated()
        {
            HealthBox newDamage = new HealthBox();
            newDamage.Box = HealthBox.HealthBoxType.Aggrivated;
            AddDamageBox(newDamage);
        }

        internal override void ResetHealth()
        {
            foreach (HealthBox curBox in HealthTrack)
            {
                curBox.Box = HealthBox.HealthBoxType.Empty;
            }
            OnPropertyChanged("Status");
        }

        private void AddDamageBox(HealthBox newDamage)
        {
            for(int curIndex = 0;curIndex< HealthTrack.Count();curIndex++)
            {
                HealthBox curBox = HealthTrack[curIndex];
                if (curBox.Box < newDamage.Box)
                {
                    HealthTrack.Insert(curIndex, newDamage);
                    break;
                }
            }
            if (HealthTrack.Last().Box == HealthBox.HealthBoxType.Empty)
            {
                HealthTrack.Remove(HealthTrack.Last());
                OnPropertyChanged("Status");
            }
            else
            {

                var secondToLastBox = HealthTrack[HealthTrack.Count() - 2];
                var lastBox = HealthTrack.Last();
                HealthBox spilloverDamage = new HealthBox();
                spilloverDamage.Box = lastBox.Box + 1;
                HealthTrack.Last().Box = HealthBox.HealthBoxType.Empty;
                AddDamageBox(spilloverDamage);
            }
        }

        public override void RollInitiative()
        {
            int r = random.Next(10) + 1;
            CurInitiative = r + Initiative;
        }

    }
}
