using System;

namespace XMLCharSheets
{
    public static class TextReporter
    {
        public static void Report(String newText)
        {
            CombatService.RosterViewModel.ReportText(newText);
        }
    }
}