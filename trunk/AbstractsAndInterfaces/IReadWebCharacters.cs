using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatAutomationTheater
{
    interface IReadWebCharacters
    {
        CharacterSheet ReadWebCharacter(ServerIntegration.TransferCharacter transferCharacter);
    }
}
