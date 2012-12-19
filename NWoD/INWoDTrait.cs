using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    public interface INWoDTrait
    {
        int TraitValue
        {
            get;
            set;
        }
        int SucceedsOn
        { get; set; }
        int ExplodesOn
        { get; set; }
        int SubtractsOn
        { get; set; }
        int AutomaticSuccesses
        { get; set; }
        void AddAndChangeFromDefaults(INWoDTrait nextTrait);
  
    }
}
