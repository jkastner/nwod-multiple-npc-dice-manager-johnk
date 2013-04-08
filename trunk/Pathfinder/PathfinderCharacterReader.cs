using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLCharSheets
{
    public class PathfinderCharacterReader : IReadCharacters
    {
        #region IReadCharacters Members

        public  CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar)
        {
            var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                        select new
                        {
                            Name = (String)item.Element("name"),
                            CharacterType = (String)item.Element("CharacterType"),
                            Speed = (String)item.Element("Speed"),
                            Height = (String)item.Element("Height"),
                            Traits = item.Descendants("traits"),
                        };
            foreach (var curQuery in query)
            {
                List<Trait> traits = new List<Trait>();
                PopulatePathfinderTraits(curQuery.Traits, traits);
                switch (curQuery.CharacterType)
                {
                    default:
                        newChar = new PathfinderCharacter(curQuery.Name, traits);
                        break;

                }
            }
            return newChar;
        }


        private void PopulatePathfinderTraits(IEnumerable<XElement> enumerable, List<Trait> traits)
        {
            var query = from item in enumerable.Elements("trait")
                        select new
                        {
                            Label = (String)item.Attribute("label"),
                            Value = (String)item.Attribute("value"),
                            Descriptor = (String)item.Attribute("Descriptor"),
                            AttackBonus = (String)item.Attribute("AttackBonus"),
                            Damage = (String)item.Attribute("Damage"),
                            Triggers = (String)item.Attribute("Triggers"),
                            TriggerEffect = (String)item.Attribute("TriggerEffect"),
                            TargetDefense = (String)item.Attribute("TargetDefense"),
                            CritRange = (String)item.Attribute("CritRange"),
                            CritMultiplier = (String)item.Attribute("CritMultiplier"),
                        };

            var listit = query.ToList();
            foreach (var curQuery in query)
            {
                if (curQuery.AttackBonus == null)
                {
                    if (curQuery.Value != null)
                    {
                        traits.Add(new PathfinderNumericTrait(curQuery.Label, int.Parse(curQuery.Value), curQuery.Descriptor));
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
                    traits.Add(new PathfinderAttackTrait(curQuery.Label, firstBonusNumeric, curQuery.Descriptor, curQuery.AttackBonus, curQuery.Damage,
                        curQuery.Triggers, curQuery.TriggerEffect, curQuery.TargetDefense, curQuery.CritRange, curQuery.CritMultiplier));
                }
            }
        }
        #endregion
    }
}
