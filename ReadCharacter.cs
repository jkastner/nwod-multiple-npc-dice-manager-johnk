using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

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
                PopulateTraits(curQuery.Traits, traits);
            }
            return newChar;
        }

        private void PopulateTraits(IEnumerable<XElement> traitsElements, List<Trait> traits)
        
        {
            var query = from item in traitsElements.Elements("trait")
                        select new
                            {
                                Label = (String) item.Attribute("label"),
                                Value = Int32.Parse((String) item.Attribute("value")),
                                TargetDefense = (String) item.Attribute("TargetDefense"),
                                DamageType = (String) item.Attribute("DamageType"),
                            };
            foreach (var curQuery in query)
            {
                if(curQuery.TargetDefense==null)
                    traits.Add(new Trait(curQuery.Value, curQuery.Label));
                else
                {
                    traits.Add(new AttackTrait(curQuery.Value, curQuery.Label, curQuery.TargetDefense, curQuery.DamageType));
                }
            }
        }

    }
}
