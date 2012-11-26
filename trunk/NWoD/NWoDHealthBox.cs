using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public class HealthBox
    {
        public enum DamageType
        {
            Empty, Bashing, Lethal, Aggrivated, Grievous
        }
        private DamageType _box;

        public DamageType Box
        {
            get { return _box; }
            set { _box = value; }
        }


        public HealthBox()
        {
            this.Box = DamageType.Empty;
        }

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
