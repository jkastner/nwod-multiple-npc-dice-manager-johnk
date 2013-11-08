using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace XMLCharSheets
{
    internal class CharacterReader
    {
        private readonly Dictionary<String, IReadCharacters> _readers = new Dictionary<string, IReadCharacters>();
        internal CharacterSheet Read(string fileName)
        {
            try
            {
                XDocument theDoc = XDocument.Load(fileName);
                CharacterSheet newChar = null;
                IEnumerable<XElement> foundChars = theDoc.Elements("CharacterSheet");
                foreach (XElement curChar in foundChars)
                {
                    var query = from item in curChar.DescendantsAndSelf("CharacterSheet")
                                select new
                                    {
                                        Ruleset = (String)item.Element("Ruleset"),
                                    };
                    foreach (var curQuery in query)
                    {
                        if (_readers.ContainsKey(curQuery.Ruleset))
                        {
                            try
                            {
                                newChar = _readers[curQuery.Ruleset].ReadCharacter(newChar, curChar);
                                if(newChar!=null)
                                    newChar.Ruleset = curQuery.Ruleset;
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown ruleset " + curQuery.Ruleset + " presented.");
                        }
                    }
                }
                return newChar;
            }
            catch (Exception e)
            {
                CombatService.RosterViewModel.LoadingErrors.Add(new Tuple<string,string>(fileName, e.Message));
            }
            return null;
        }

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