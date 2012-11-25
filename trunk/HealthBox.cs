using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    class HealthBox
    {
        public enum HealthBoxType
        {
            Empty, Bashing, Lethal, Aggrivated, Grievous
        }
        private HealthBoxType _box;

        public HealthBoxType Box
        {
            get { return _box; }
            set { _box = value; }
        }


        public HealthBox()
        {
            this.Box = HealthBoxType.Empty;
        }

        public override string ToString()
        {
            return Box.ToString();
        }


        internal String ToBoxString()
        {
            switch (Box)
            {
                case HealthBoxType.Empty:
                    return "_";
                case HealthBoxType.Bashing:
                    return "\\";
                case HealthBoxType.Lethal:
                    return "X";
                case HealthBoxType.Aggrivated:
                    return "*";
                case HealthBoxType.Grievous:
                    return "█";
            }
            return "?";
        }
    }
}
