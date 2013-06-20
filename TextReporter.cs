using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
