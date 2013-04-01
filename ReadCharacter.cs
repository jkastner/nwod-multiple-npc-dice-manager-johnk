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
                        newChar = _readers[curQuery.Ruleset].ReadCharacter(newChar, curChar);
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




    }
}
