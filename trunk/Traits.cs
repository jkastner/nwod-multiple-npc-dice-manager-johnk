using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace XMLCharSheets
{
    public class Traits : ObservableCollection<Trait>
    {
        public Trait AddIfNew(String traitName)
        {
            Trait newTrait = new Trait(traitName);
            if(!this.Contains(newTrait))
            {
                this.Add(newTrait);
            }
            else
            {
                newTrait = this[IndexOf(newTrait)];
            }
            return newTrait;
        }
    }
}
