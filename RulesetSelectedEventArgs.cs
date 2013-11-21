using System;

namespace CombatAutomationTheater
{
    public class RulesetSelectedEventArgs : EventArgs
    {
        public RulesetSelectedEventArgs(String ruleset)
        {
            SelectedRuleset = ruleset;
        }

        public String SelectedRuleset { get; set; }

        public static string ClearRulesetString 
        { 
            get
            {
                return "ClearRulesetOnOpenecb2fb89-80cf-4d21-a6fd-7f2a51bfa34f";
            }
        }
    }
}