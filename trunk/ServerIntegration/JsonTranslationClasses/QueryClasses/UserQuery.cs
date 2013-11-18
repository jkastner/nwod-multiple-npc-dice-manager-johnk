using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace ServerIntegration.JsonTranslationClasses.QueryClasses
{
    public class UserQuery
    {
        public class Game
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Body
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public List<Game> games { get; set; }
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
