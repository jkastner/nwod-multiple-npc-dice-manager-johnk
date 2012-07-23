using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Windows;

namespace XMLCharSheets
{
    public class Character
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _description;

        public Character(string name, ObservableCollection<NumberedTrait> curTraits)
        {
            _name = name;
            _numberedTraits = curTraits;

        }

        private ObservableCollection<NumberedTrait> _numberedTraits;

        public ObservableCollection<NumberedTrait> NumberedTraits
        {
            get { return _numberedTraits; }
            set { _numberedTraits = value; }
        }

        public NumberedTrait FindTrait(String targetName)
        {
            return (NumberedTrait) NumberedTraits.Where(x => x.TraitLabel.Equals(targetName)).FirstOrDefault();
        }
        
        public static Character MakeChar(String fileName, Traits allTraits)
        {
            Character curChar = null;
            try
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                ObservableCollection<NumberedTrait> curCharNumberedTraits = new ObservableCollection<NumberedTrait>();
                while (reader.Read())
                {

                    if (reader.Name.ToLower().Trim().Equals("name") && reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        reader.Read();
                        curChar = new Character(reader.Value, curCharNumberedTraits);
                    }
                    if (reader.Name.ToLower().Trim().Equals("trait"))
                    {
                        String label = reader.GetAttribute("label");
                        int value = Convert.ToInt32(reader.GetAttribute("value"));
                        Trait baseTrait = new Trait(label);
                        NumberedTrait curnNumberedTrait = new NumberedTrait(value, baseTrait);
                        allTraits.AddIfNew(label);
                        curCharNumberedTraits.Add(curnNumberedTrait);
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error opening " + fileName);
                throw;
            }
            return curChar;
        }
    }
}
