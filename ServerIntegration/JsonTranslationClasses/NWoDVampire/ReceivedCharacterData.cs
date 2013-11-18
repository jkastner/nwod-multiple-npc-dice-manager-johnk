using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class CharacterData
    {
        private IDictionary<string, int> _attributes;
        public int id { get; set; }
        public int xp_total { get; set; }
        public string name { get; set; }
        public string vice { get; set; }
        public string virtue { get; set; }
        public int xp_diff { get; set; }
        public int initiative { get; set; }
        public int potency { get; set; }
        public int size { get; set; }
        public int morality { get; set; }
        public Dictionary<string, int> attributes;
        public IDictionary<string, int> skills { get; set; }

        public IDictionary<string, int> merits { get; set; }
        public IDictionary<string, IList<String>> specialties { get; set; }
        public IDictionary<string, IList<String>> derangements { get; set; }
        public IDictionary<string, int> health { get; set; }
        public IDictionary<string, int> disciplines { get; set; }
        public int blood { get; set; }
        


    }
}
