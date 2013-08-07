using System;
using System.Runtime.Serialization;

namespace XMLCharSheets
{
    [DataContract(Namespace = "")]
    public class HealthBox
    {
        [DataContract(Name = "DamageType")]
        public enum DamageType
        {
            [EnumMember] Empty,
            [EnumMember] Bashing,
            [EnumMember] Lethal,
            [EnumMember] Aggrivated,
            [EnumMember] Grievous
        }


        public HealthBox()
        {
            Box = DamageType.Empty;
        }

        [DataMember]
        public DamageType Box { get; set; }

        public override string ToString()
        {
            return Box.ToString();
        }


        internal String ToBoxString()
        {
            switch (Box)
            {
                case DamageType.Empty:
                    return "_";
                case DamageType.Bashing:
                    return "\\";
                case DamageType.Lethal:
                    return "X";
                case DamageType.Aggrivated:
                    return "*";
                case DamageType.Grievous:
                default:
                    return "█";
            }
        }
    }
}