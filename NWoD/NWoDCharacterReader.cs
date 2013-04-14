using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace XMLCharSheets
{
    public class NWoDCharacterReader : IReadCharacters
    {

        #region IReadCharacters Members
        public CharacterSheet ReadCharacter(CharacterSheet newChar, XElement curChar)
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
            Type type = this.GetType();
            Assembly assembly = type.Assembly;
            UserControl mathUC = (UserControl)assembly.CreateInstance(string.Format("{0}.NWoDControl", type.Namespace));
            return mathUC;
        }

        private void PopulateNWoDTraits(IEnumerable<XElement> traitsElements, List<Trait> traits)
        {
            var query = from item in traitsElements.Elements("trait")
                        select new
                        {
                            Label = (String)item.Attribute("label"),
                            Value = Int32.Parse((String)item.Attribute("value")),
                            ExplodesOn = (String)item.Attribute("ExplodesOn"),
                            SubtractsOn = (String)item.Attribute("SubtractsOn"),
                            AutomaticSuccesses = (String)item.Attribute("AutomaticSuccesses"),
                            TargetDefense = (String)item.Attribute("TargetDefense"),
                            DamageType = (String)item.Attribute("DamageType"),
                            SucceedsOn = (String)item.Attribute("SucceedsOn"),
                        };

            var listit = query.ToList();
            foreach (var curQuery in query)
            {
                int autoSuccesses = ExtractDefaultInt(curQuery.AutomaticSuccesses, 0);
                int subtractsOn = ExtractDefaultInt(curQuery.SubtractsOn, 0);
                int explodesOn = ExtractDefaultInt(curQuery.ExplodesOn, 10);
                int succeedOn = ExtractDefaultInt(curQuery.SucceedsOn, 8);
                if (curQuery.TargetDefense == null)
                    traits.Add(new NWoDTrait(curQuery.Label, curQuery.Value, explodesOn, subtractsOn, autoSuccesses, succeedOn));
                else
                {
                    traits.Add(new NWoDAttackTrait(curQuery.Value, curQuery.Label, curQuery.TargetDefense, curQuery.DamageType,
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

        #endregion
    }
}
