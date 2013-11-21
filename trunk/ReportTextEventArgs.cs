using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace CombatAutomationTheater
{
    public class ReportTextEventArgs : EventArgs
    {
        public String Message { get; set; }
        public Brush DisplayColor { get; set; }
        public double FontSize { get; set; }
        public ReportTextEventArgs(string message, Brush solidColorBrush, double fontSize)
        {
            Message = message;
            DisplayColor = solidColorBrush;
            FontSize = fontSize;
        }
    }
}
