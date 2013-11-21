using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatAutomationTheater
{
    public abstract class CharacterActionScript
    {
        public abstract void PerformAction(CharacterSheet activeCharacter);
    }
}
