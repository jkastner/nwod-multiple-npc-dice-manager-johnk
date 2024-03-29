﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

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
                    TextReporter.Report(regularChar.Name + " is not a vampire.");
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
                List<Trait> traits = new List<Trait>();
                traits.Add(someChar.FindNumericTrait(traitOther));
                if (someChar.ResistanceTrait() != null)
                    traits.Add(someChar.ResistanceTrait());
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
                    List<Trait> traits = new List<Trait>();
                    traits.Add(curChar.FindNumericTrait("Composure"));
                    traits.Add(curChar.FindNumericTrait("Resolve"));
                    CombatService.RosterViewModel.RollCharacters(characters, traits);
                }
            }
        }
    }
}