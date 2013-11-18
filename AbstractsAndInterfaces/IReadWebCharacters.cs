using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLCharSheets
{
    interface IReadWebCharacters
    {
        CharacterSheet ReadWebCharacter(ServerIntegration.TransferCharacter transferCharacter);
    }
}
