using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace XMLCharSheets
{
    class CharacterReader
    {

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
                    if (_readers.ContainsKey(curQuery.Ruleset))
                    {
                        try
                        {
                            newChar = _readers[curQuery.Ruleset].ReadCharacter(newChar, curChar);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show("Failed to read " + fileName);
                        }
                        newChar.Ruleset = curQuery.Ruleset;
                    }
                    else
                    {
                        throw new Exception("Unknown ruleset "+curQuery.Ruleset+" presented.");
                    }
                    

                }
            }
            return newChar;
        }

        private Dictionary<String, IReadCharacters> _readers = new Dictionary<string, IReadCharacters>();
        public void RegisterReader(String RulesetName, IReadCharacters reader)
        {
            _readers.Add(RulesetName, reader);
        }




        internal UIElement FindControl(string rulesetName)
        {
            if (_readers.ContainsKey(rulesetName))
            {
                return _readers[rulesetName].CustomControlItem();
            }
            return null;
        }

        internal List<String> LoadDamageFor(string rulesetName)
        {
            if (_readers.ContainsKey(rulesetName))
            {
                return _readers[rulesetName].DamageList;
            }
            return null;
        }
    }
}
