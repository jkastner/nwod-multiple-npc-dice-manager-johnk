using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerIntegration
{
    public class JsonContract
    {
        public class Name
        {
            public string name { get; set; }
            public string link { get; set; }
        }

        public class User
        {
            public string name { get; set; }
            public string link { get; set; }
            public int id { get; set; }
        }

        public class Game
        {
            public string name { get; set; }
            public string link { get; set; }
            public int id { get; set; }
        }

        public class Skills
        {
            public List<string> mental { get; set; }
            public List<string> physical { get; set; }
            public List<string> social { get; set; }
        }

        public class Meta
        {
            public string type { get; set; }
            public Skills skills { get; set; }
            public List<string> alternateDisplay { get; set; }
            public List<string> discounted { get; set; }
        }

        public class Character
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

        public class Data2
        {
            public string Gender { get; set; }
            public string Ethnicity { get; set; }
        }

        public class Data
        {
            public Meta meta { get; set; }
            public Character Character { get; set; }
            //public IDictionary<string, Character> Characters { get; set; }
            public Data2 data { get; set; }
        }

        public class Body
        {
            public int id { get; set; }
            public bool owned { get; set; }
            public bool master { get; set; }
            public Name name { get; set; }
            public User user { get; set; }
            public Game game { get; set; }
            public string picture { get; set; }
            public Data data { get; set; }
        }

        public class Success
        {
            public int id { get; set; }
            public Body body { get; set; }
        }

        public class RootObject
        {
            public int code { get; set; }
            public Success success { get; set; }
        }
    }
}
