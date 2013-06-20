using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class RulesetSelectedEventArgs : EventArgs
    {
        public String SelectedRuleset { get; set; }
        public RulesetSelectedEventArgs(String ruleset)
        {
            SelectedRuleset = ruleset;
        }
    }

}
