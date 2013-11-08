using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;

namespace XMLCharSheets
{
    public class PathfinderCharacterReader : IReadCharacters
    {
        #region IReadCharacters Members

        public CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar)
        {
            var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                        select new
                            {
                                Name = (String) item.Element("Name"),
                                CharacterType = (String) item.Element("CharacterType"),
                                Traits = item.Descendants("traits"),
                            };
            foreach (var curQuery in query)
            {
                var traits = new List<Trait>();
                PopulatePathfinderTraits(curQuery.Traits, traits);
                bool hasWoundsVigor = traits.Any(x=>x.TraitLabel.Equals("Vigor"));
                if (
                    String.IsNullOrEmpty(curQuery.Name) ||
                    String.IsNullOrEmpty(curQuery.CharacterType) ||
                    curQuery.Traits == null
                    )
                {
                    return null;
                }
                switch (curQuery.CharacterType)
                {
                    default:
                        if (hasWoundsVigor)
                            newChar = new PathfinderCharacter_WoundsVigor(curQuery.Name, traits);
                        else
                            newChar = new PathfinderCharacter_HP(curQuery.Name, traits);
                        break;
                }
            }
            return newChar;
        }


        public UserControl CustomControlItem()
        {
            Type type = GetType();
            Assembly assembly = type.Assembly;
            var customControl =
                (UserControl) assembly.CreateInstance(string.Format("{0}.PathfinderControl", type.Namespace));
            return customControl;
        }


        private void PopulatePathfinderTraits(IEnumerable<XElement> enumerable, List<Trait> traits)
        {
            var query = from item in enumerable.Elements("trait")
                        select new
                            {
                                Label = (String) item.Attribute("label"),
                                Value = (String) item.Attribute("value"),
                                Descriptor = (String) item.Attribute("Descriptor"),
                                AttackBonus = (String) item.Attribute("AttackBonus"),
                                Damage = (String) item.Attribute("Damage"),
                                Triggers = (String) item.Attribute("Triggers"),
                                TriggerEffect = (String) item.Attribute("TriggerEffect"),
                                TargetDefense = (String) item.Attribute("TargetDefense"),
                                CritRange = (String) item.Attribute("CritRange"),
                                CritMultiplier = (String) item.Attribute("CritMultiplier"),
                            };

            var listit = query.ToList();
            foreach (var curQuery in query)
            {
                if (curQuery.AttackBonus == null)
                {
                    int queryValue = 0;
                    if (curQuery.Value != null && int.TryParse(curQuery.Value, out queryValue))
                    {
                        traits.Add(new PathfinderNumericTrait(curQuery.Label, queryValue,
                                                              curQuery.Descriptor));
                    }
                    else
                    {
                        traits.Add(new PathfinderStringTrait(curQuery.Label, curQuery.Descriptor));
                    }
                }
                else
                {
                    //public PathfinderAttackTrait(int traitValue, String traitLabel, String defenseTarget, String damageType) :
                    //Attack bonus looks like this:
                    //AttackBonus='35/30/25/20' 
                    String firstBonus = curQuery.AttackBonus.Split('/').First();
                    int firstBonusNumeric = int.Parse(firstBonus);
                    String bonus = curQuery.AttackBonus;
                    traits.Add(new PathfinderAttackTrait(curQuery.Label, firstBonusNumeric, curQuery.Descriptor,
                                                         curQuery.AttackBonus, curQuery.Damage,
                                                         curQuery.Triggers, curQuery.TriggerEffect,
                                                         curQuery.TargetDefense, curQuery.CritRange,
                                                         curQuery.CritMultiplier));
                }
            }
        }

        #endregion

        public List<string> DamageList
        {
            get
            {
                return new List<string>
                    {
                        "Slashing",
                        "Bludgeoning",
                        "Piercing",
                        "Precision",
                        "Fire",
                        "Cold",
                        "Electricity",
                        "Acid",
                        "Sonic",
                        "Force",
                        "Holy",
                        "Unholy",
                        "Good",
                        "Evil",
                        "Law",
                        "Chaos",
                        "Magic",
                        "Divine"
                    };
            }
        }
    }
}