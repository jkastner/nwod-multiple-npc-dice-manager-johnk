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
                switch (curQuery.CharacterType)
                {
                    default:
                        newChar = new PathfinderCharacter(curQuery.Name, traits);
                        break;

                }
                PopulatePathfinderTraits(curQuery.Traits, traits);
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
                            OvercomeBy = (String)item.Attribute("OvercomeBy"),
                            Descriptor = (String)item.Attribute("Descriptor"),
                            AttackBonus = (String)item.Attribute("AttackBonus"),
                            Damage = (String)item.Attribute("Damage"),
                            DamageType = (String)item.Attribute("DamageType"),
                            Triggers = (String)item.Attribute("Triggers"),
                            TriggerEffect = (String)item.Attribute("TriggerEffect"),
                            TargetDefense = (String)item.Attribute("TargetDefense"),
                        };

            var listit = query.ToList();
            foreach (var curQuery in query)
            {
                if (curQuery.AttackBonus == null)
                    traits.Add(new PathfinderTrait(curQuery.Label, curQuery.Value, curQuery.OvercomeBy, curQuery.Descriptor));
                else
                {
                    //public PathfinderAttackTrait(int traitValue, String traitLabel, String defenseTarget, String damageType) :
                    traits.Add(new PathfinderAttackTrait(5, curQuery.Label, curQuery.Descriptor, curQuery.AttackBonus, curQuery.Damage, curQuery.DamageType, curQuery.Triggers, curQuery.TriggerEffect, curQuery.TargetDefense));
                }
            }
        }
        #endregion
    }
}
