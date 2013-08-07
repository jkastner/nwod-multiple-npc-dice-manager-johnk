using System.Collections;

namespace XMLCharSheets.NWoD
{
    internal class NWoDRosterViewModel
    {
        internal void BloodBuff(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    var regularChar = curItem as CharacterSheet;
                    TextReporter.Report(regularChar.Name + " is not a vampire.");
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                        curVampire.BloodBuff();
                    else
                        TextReporter.Report(curVampire.Name + " did not have enough Vitae.");
                }
            }
        }

        internal void BloodHeal(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    var regularChar = curItem as CharacterSheet;
                    TextReporter.Report(regularChar.Name + " is not a vampire.");
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                    {
                        if (curVampire.HasHealableWounds())
                            curVampire.BloodHeal();
                        else
                        {
                            TextReporter.Report(curVampire.Name + " did not have wounds that could be healed.");
                        }
                    }
                    else
                        TextReporter.Report(curVampire.Name + " did not have enough Vitae.");
                }
            }
        }

        internal void DoLethal(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Lethal");
            }
        }

        internal void DoAggrivated(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Aggrivated");
            }
        }

        internal void DoBashing(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as CharacterSheet;
                curChar.DoDamage(1, "Bashing");
            }
        }

        internal void RefillVitae(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    var regularChar = curItem as CharacterSheet;
                    TextReporter.Report(regularChar.Name + " is not a vampire.");
                }
                else
                {
                    curVampire.ResetVitae();
                }
            }
        }
    }
}