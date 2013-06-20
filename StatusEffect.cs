using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBoard;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract]
    public class StatusEffect
    {
        private String _description;
        [DataMember]
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
        [DataMember]
        public int DurationRemaining
        {
            get { return _durationRemaining; }
            set { _durationRemaining = value; }
        }

        /// <summary>
        /// Used to pass the info without a circular dependency.
        /// </summary>
        /// <returns></returns>
        public StatusEffectDisplay CreateStatusEffectDisplay()
        {
            return new StatusEffectDisplay()
            {
                Description = Description,
                TurnsRemaining = DurationRemaining.ToString(),
            };
        }

    }
}
