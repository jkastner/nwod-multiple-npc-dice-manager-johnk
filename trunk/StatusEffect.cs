using System;
using System.Runtime.Serialization;
using GameBoard;

namespace XMLCharSheets
{
    [DataContract]
    public class StatusEffect
    {
        public StatusEffect(string description, int duration)
        {
            // TODO: Complete member initialization
            Description = description;
            DurationRemaining = duration;
        }

        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public int DurationRemaining { get; set; }

        /// <summary>
        ///     Used to pass the info without a circular dependency.
        /// </summary>
        /// <returns></returns>
        public StatusEffectDisplay CreateStatusEffectDisplay()
        {
            return new StatusEffectDisplay
                {
                    Description = Description,
                    TurnsRemaining = DurationRemaining.ToString(),
                };
        }
    }
}