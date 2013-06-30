﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace XMLCharSheets
{
    interface IReadCharacters
    {
        CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar);
        UserControl CustomControlItem();
        List<string> DamageList { get; }
    }
}
