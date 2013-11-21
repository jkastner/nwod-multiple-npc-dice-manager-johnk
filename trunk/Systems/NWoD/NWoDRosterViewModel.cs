using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace CombatAutomationTheater.NWoD
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
                    TextReporter.Report(regularChar.Name + " is not a vampire.\n");
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                        curVampire.BloodBuff();
                    else
                        TextReporter.Report(curVampire.Name + " did not have enough Vitae.\n");
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
                    TextReporter.Report(regularChar.Name + " is not a vampire.\n");
                }
                else
                {
                    if (curVampire.CurrentVitae > 0)
                    {
                        if (curVampire.HasHealableWounds())
                            curVampire.BloodHeal();
                        else
                        {
                            TextReporter.Report(curVampire.Name + " did not have wounds that could be healed.\n");
                        }
                    }
                    else
                        TextReporter.Report(curVampire.Name + " did not have enough Vitae.\n");
                }
            }
        }

        internal void DoLethal(IList characters)
        {
            CombatService.RosterViewModel.DoDamage(characters, 1, "Lethal");
        }

        internal void DoAggrivated(IList characters)
        {
            CombatService.RosterViewModel.DoDamage(characters, 1, "Aggrivated");
        }

        internal void DoBashing(IList characters)
        {
            CombatService.RosterViewModel.DoDamage(characters, 1, "Bashing");
        }

        internal void RefillVitae(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curVampire = curItem as NWoDVampire;
                if (curVampire == null)
                {
                    var regularChar = curItem as CharacterSheet;
                    TextReporter.Report(regularChar.Name + " is not a vampire.\n");
                }
                else
                {
                    curVampire.ResetVitae();
                }
            }
        }

        internal void Roll_Composure_and_Resistance(IList characters)
        {
            RollResistitanceTraits("Composure", characters);
        }

        internal void Roll_Resolve_and_Resistance(IList characters)
        {
            RollResistitanceTraits("Resolve", characters);
        }
        private void RollResistitanceTraits(string traitOther, IList characters)
        {
            Dictionary<Type, List<NWoDCharacter>> _charactersByType = new Dictionary<Type, List<NWoDCharacter>>();
            foreach (object curItem in characters)
            {
                var curChar = curItem as NWoDCharacter;
                if (curChar != null)
                {
                    if(!_charactersByType.ContainsKey(curChar.GetType()))
                    {
                        _charactersByType.Add(curChar.GetType(), new List<NWoDCharacter>());
                    }
                    _charactersByType[curChar.GetType()].Add(curChar);
                }
            }
            foreach (var curPair in _charactersByType)
            {
                var someChar = curPair.Value[0];
                List<String> traits = new List<String>();
                traits.Add(someChar.FindNumericTrait(traitOther).TraitLabel);
                if (someChar.ResistanceTrait() != null)
                    traits.Add(someChar.ResistanceTrait().TraitLabel);
                CombatService.RosterViewModel.RollCharacters(curPair.Value, traits);

            }

            
        }




        internal void Roll_Resolve_And_Composure(IList characters)
        {
            foreach (object curItem in characters)
            {
                var curChar = curItem as NWoDCharacter;
                if (curChar != null)
                {
                    List<String> traits = new List<String>();
                    traits.Add("Composure");
                    traits.Add("Resolve");
                    CombatService.RosterViewModel.RollCharacters(characters, traits);
                }
            }
        }
    }
}