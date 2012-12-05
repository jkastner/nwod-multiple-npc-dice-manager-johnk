using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using XMLCharSheets.NWoD;

namespace XMLCharSheets
{
    class ReadCharacter
    {
        //private enum PersonType
        //{
        //    Person, Vampire
        //}
        //internal CharacterSheet OldRead(string fileName)
        //{
        //    CharacterSheet curChar = null;
        //    try
        //    {
        //        PersonType curType = PersonType.Person;
        //        XmlTextReader reader = new XmlTextReader(fileName);
        //        List<Trait> curTraits = new List<Trait>();
        //        while (reader.Read())
        //        {
        //            String curReader = reader.Name.ToLower().Trim();
        //            if (curReader.Equals("charactertype") && reader.NodeType.Equals(XmlNodeType.Element))
        //            {
        //                reader.Read();
        //                switch (reader.Value.ToLower().Trim())
        //                {
        //                    case "vampire":
        //                        curType = PersonType.Vampire;
        //                        break;
        //                    case "human":
        //                    default:
        //                        curType = PersonType.Person;
        //                        break;

        //                }
        //            }

        //            if (curReader.Equals("name") && reader.NodeType.Equals(XmlNodeType.Element))
        //            {
        //                reader.Read();
        //                if (curType == PersonType.Vampire)
        //                    curChar = new NWoDVampire(reader.Value, curTraits);
        //                else
        //                    curChar = new NWoDCharacter(reader.Value, curTraits);

        //            }
        //            if (curReader.Equals("trait"))
        //            {
        //                String label = reader.GetAttribute("label");
        //                int value = Convert.ToInt32(reader.GetAttribute("value"));
        //                String targetDefense = reader.GetAttribute("TargetDefense");
        //                String damageType = reader.GetAttribute("DamageType");
        //                if (targetDefense != null)
        //                {
        //                    AttackTrait curAttack = new AttackTrait(value, label, targetDefense, damageType);
        //                    curTraits.Add(curAttack);
        //                }
        //                else
        //                {
        //                    Trait curNumberedTrait = new Trait(value, label);
        //                    curTraits.Add(curNumberedTrait);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Error opening " + fileName);
        //        throw;
        //    }
        //    return curChar;
        //}

        internal CharacterSheet Read(string fileName)
        {
            XDocument theDoc = XDocument.Load(fileName);
            CharacterSheet newChar = null;
            var foundChars = theDoc.Elements("CharacterSheet");
            foreach(var curChar in foundChars)
            {
                var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                            select new
                            {
                                Ruleset = (String)item.Element("Ruleset"),
                            };
                foreach(var curQuery in query)
                {
                    switch(curQuery.Ruleset)
                    {
                        case "NWoD":
                            newChar = ReadNWoDChar(newChar, curChar);
                            break;
                        default:
                            throw new Exception("Not ready");
                    }

                }
            }
            return newChar;
        }

        private CharacterSheet ReadNWoDChar(CharacterSheet newChar, XElement curChar)
        {
            var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                        select new
                        {
                            Name = (String)item.Element("name"),
                            CharacterType = (String)item.Element("CharacterType"),
                            Traits = item.Descendants("traits"),
                        };
            foreach (var curQuery in query)
            {
                List<Trait> traits = new List<Trait>();
                switch (curQuery.CharacterType)
                {
                    case "Vampire":
                        newChar = new NWoDVampire(curQuery.Name, traits);
                        break;
                    default:
                        newChar = new NWoDCharacter(curQuery.Name, traits);
                        break;

                }
                PopulateNWoDTraits(curQuery.Traits, traits);
            }
            return newChar;
        }

        private void PopulateNWoDTraits(IEnumerable<XElement> traitsElements, List<Trait> traits)
        
        {
            var query = from item in traitsElements.Elements("trait")
                        select new
                            {
                                Label = (String) item.Attribute("label"),
                                Value = Int32.Parse((String) item.Attribute("value")),
                                ExplodesOn = (String)item.Attribute("ExplodesOn"),
                                SubtractsOn = (String)item.Attribute("SubtractsOn"),
                                AutomaticSuccesses = (String)item.Attribute("AutomaticSuccesses"),
                                TargetDefense = (String) item.Attribute("TargetDefense"),
                                DamageType = (String) item.Attribute("DamageType"),

                            };
            

            foreach (var curQuery in query)
            {
                int autoSuccesses = ExtractDefaultInt(curQuery.AutomaticSuccesses, 0);
                int subtractsOn = ExtractDefaultInt(curQuery.SubtractsOn, 0);
                int explodesOn = ExtractDefaultInt(curQuery.ExplodesOn, 10);
                if(curQuery.TargetDefense==null)
                    traits.Add(new NWoDTrait(curQuery.Value, curQuery.Label, explodesOn, subtractsOn, autoSuccesses));
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
            if(Int32.TryParse(curValue, out result))
            {
                return result;
            }
            return defaultValue;

        }

    }
}
