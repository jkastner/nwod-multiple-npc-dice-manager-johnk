using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class StatusEffect
    {
        private String _description;
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public StatusEffect(string description, int duration)
        {
            // TODO: Complete member initialization
            this.Description = description;
            this.DurationRemaining = duration;
        }

        private int _durationRemaining;
        public int DurationRemaining
        {
            get { return _durationRemaining; }
            set { _durationRemaining = value; }
        }


    }
}
