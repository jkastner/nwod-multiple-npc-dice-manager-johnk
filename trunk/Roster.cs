using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;

namespace XMLCharSheets
{
    public class Roster : ObservableCollection <Character>
    {


        internal new void Add(Character character)
        {
            throw new NotImplementedException();
        }
    }
}
