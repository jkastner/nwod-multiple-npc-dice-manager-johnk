using System;

namespace XMLCharSheets
{
    public class RulesetSelectedEventArgs : EventArgs
    {
        public RulesetSelectedEventArgs(String ruleset)
        {
            SelectedRuleset = ruleset;
        }

        public String SelectedRuleset { get; set; }
    }
}