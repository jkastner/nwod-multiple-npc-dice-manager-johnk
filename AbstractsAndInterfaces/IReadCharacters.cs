using ServerIntegration;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Linq;

namespace CombatAutomationTheater
{
    internal interface IReadCharacters
    {
        List<string> DamageList { get; }
        CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar);
        UserControl CustomControlItem();
    }
}