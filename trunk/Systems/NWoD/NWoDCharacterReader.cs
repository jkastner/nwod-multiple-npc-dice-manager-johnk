using ServerIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;

namespace CombatAutomationTheater
{
    public class NWoDCharacterReader : IReadCharacters, IReadWebCharacters
    {
        #region IReadCharacters Members

        public CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar)
        {
            var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                        select new
                            {
                                Name = (String) item.Element("Name"),
                                CharacterType = (String) item.Element("CharacterType"),
                                Speed = (String) item.Element("Speed"),
                                Height = (String) item.Element("Height"),
                                Traits = item.Descendants("traits"),
                            };
            foreach (var curQuery in query)
            {
                var traits = new List<Trait>();
                PopulateNWoDTraits(curQuery.Traits, traits);
                switch (curQuery.CharacterType)
                {
                    case "Vampire":
                        newChar = new NWoDVampire(curQuery.Name, traits);
                        break;
                    default:
                        newChar = new NWoDCharacter(curQuery.Name, traits);
                        break;
                }
            }
            return newChar;
        }

        public UserControl CustomControlItem()
        {
            Type type = GetType();
            Assembly assembly = type.Assembly;
            var customControl = (UserControl) assembly.CreateInstance(string.Format("{0}.NWoDControl", type.Namespace));
            var temp = customControl as NWoDControl;
            return customControl;
        }

        public List<string> DamageList
        {
            get
            {
                return new List<string>
                    {
                        "Bashing",
                        "Lethal",
                        "Aggrivated"
                    };
            }
        }

        private void PopulateNWoDTraits(IEnumerable<XElement> traitsElements, List<Trait> traits)
        {
            var query = from item in traitsElements.Elements("trait")
                        select new
                            {
                                Label = (String) item.Attribute("label"),
                                Value = Int32.Parse((String) item.Attribute("value")),
                                ExplodesOn = (String) item.Attribute("ExplodesOn"),
                                SubtractsOn = (String) item.Attribute("SubtractsOn"),
                                AutomaticSuccesses = (String) item.Attribute("AutomaticSuccesses"),
                                TargetDefense = (String) item.Attribute("TargetDefense"),
                                DamageType = (String) item.Attribute("DamageType"),
                                SucceedsOn = (String) item.Attribute("SucceedsOn"),
                            };

            var listit = query.ToList();
            foreach (var curQuery in query)
            {
                int autoSuccesses = ExtractDefaultInt(curQuery.AutomaticSuccesses, 0);
                int subtractsOn = ExtractDefaultInt(curQuery.SubtractsOn, 0);
                int explodesOn = ExtractDefaultInt(curQuery.ExplodesOn, 10);
                int succeedOn = ExtractDefaultInt(curQuery.SucceedsOn, 8);
                if (curQuery.TargetDefense == null)
                    traits.Add(new NWoDTrait(curQuery.Label, curQuery.Value, explodesOn, subtractsOn, autoSuccesses,
                                             succeedOn));
                else
                {
                    traits.Add(new NWoDAttackTrait(curQuery.Value, curQuery.Label, curQuery.TargetDefense,
                                                   curQuery.DamageType,
                                                   explodesOn, subtractsOn, autoSuccesses));
                }
            }
        }

        private int ExtractDefaultInt(String curValue, int defaultValue)
        {
            int result = -1;
            if (Int32.TryParse(curValue, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public CharacterSheet ReadWebCharacter(TransferCharacter tc)
        {
            TransferCharacterNWoDVampire tcv = tc as TransferCharacterNWoDVampire;
            if (tcv == null)
            {
                return null;
            }
            var traitList = new List<Trait>();
            foreach (var cur in tcv.StringTraits)
            {
                traitList.Add(new NWoDStringTrait(cur.Label, cur.Contents));
            }
            foreach (var cur in tcv.NumberTraits)
            {
                traitList.Add(new NWoDTrait(cur.Label, cur.Contents, 10, 0, 0, 8));
            }
            var dex = tcv.Dexterity;
            var str = tcv.Strength;
                        
            var firearms = tcv.Firearms;
            var weaponry = tcv.Weaponry;
            var brawl = tcv.Brawl;
            if(dex!=null && firearms!= null)
            {
                traitList.Add
                (
                    MakeAttackTrait(dex, firearms, "Dexterity and firearms", NWoDConstants.RangedDefenseStatName, "Lethal")
                );
            }
            if (str != null && weaponry != null)
            {
                traitList.Add
                (
                    MakeAttackTrait(str, weaponry, "Strength and weaponry", NWoDConstants.MeleeDefenseStatName, "Lethal")
                );
            }
            if (str != null && brawl != null)
            {
                traitList.Add
                (
                    MakeAttackTrait(str, brawl, "Strength and brawl", NWoDConstants.MeleeDefenseStatName, "Bashing")
                );
            }

            traitList.Add(new NWoDTrait(NWoDConstants.HealthStatName, tcv.Health, 10, 0, 0, 8));
            traitList.Add(new NWoDTrait(NWoDConstants.MeleeDefenseStatName, tcv.MeleeDefense, 10, 0, 0, 8));
            traitList.Add(new NWoDTrait(CharacterSheet.SpeedTraitLabel, tcv.Speed, 10, 0, 0, 8));
            traitList.Add(new NWoDTrait(CharacterSheet.HeightTraitLabel, tcv.Height, 10, 0, 0, 8));
            traitList.Add(new NWoDTrait(NWoDConstants.RangedDefenseStatName, tcv.RangedDefense, 10, 0, 0, 8));
            var newChar = new NWoDVampire(tcv.Name, traitList);
            newChar.Ruleset = tcv.SystemLabel;
            return newChar;

        }

        private NWoDAttackTrait MakeAttackTrait(int a, int b, string description, string defenseTarget
            , string damageType)
        {
            return new NWoDAttackTrait(a+b, description, defenseTarget,
                               damageType,
                               10, 0, 0);
        }


        #endregion
    }
}