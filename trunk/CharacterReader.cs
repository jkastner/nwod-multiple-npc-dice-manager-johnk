using ServerIntegration;
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
        private readonly Dictionary<String, IReadWebCharacters> _webReaders = new Dictionary<string, IReadWebCharacters>();
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
                                CheckForValidProperties(newChar);
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

        private void CheckForValidProperties(CharacterSheet newChar)
        {
            String missingProperties = "";
            if (!newChar.HasNecessaryProperties(out missingProperties))
            {
                throw new Exception(newChar.Name + " did not have property " + missingProperties);
            }
        }

        public void RegisterReader(String RulesetName, IReadCharacters reader)
        {
            _readers.Add(RulesetName, reader);
        }
        public void RegisterWebReader(String RulesetName, IReadWebCharacters reader)
        {
            _webReaders.Add(RulesetName, reader);
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


        internal CharacterSheet ReadWebCharacter(TransferCharacter transferCharacter)
        {
            if (!_readers.ContainsKey(transferCharacter.SystemLabel))
            {
                TextReporter.Report("No reader for system " + transferCharacter.SystemLabel);
                return null;
            }
            var newSheet = _webReaders[transferCharacter.SystemLabel].ReadWebCharacter(transferCharacter);
            CheckForValidProperties(newSheet);
            return newSheet;
        }
    }
}