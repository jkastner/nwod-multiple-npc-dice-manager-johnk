using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration.JsonTranslationClasses.QueryClasses
{
    public class GameQuery
    {
        public class Character
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Body
        {
            public int id { get; set; }
            public string name { get; set; }
            public string system { get; set; }
            public List<Character> characters { get; set; }
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
