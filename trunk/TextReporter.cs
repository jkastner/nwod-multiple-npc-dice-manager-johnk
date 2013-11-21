using System;
using System.Windows.Media;

namespace CombatAutomationTheater
{
    public static class TextReporter
    {
        static Brush defaultBrush = Brushes.Black;
        static double defaultFont = 12;
        public static Brush DamageBrush = Brushes.Red;
        public static void Report(String newText)
        {
            CombatService.RosterViewModel.ReportText(new ReportTextEventArgs(newText, defaultBrush, defaultFont));
        }

        public static void Report(String newText, Brush reportColor)
        {
            CombatService.RosterViewModel.ReportText(new ReportTextEventArgs(newText, reportColor, 12));
        }

    }
}