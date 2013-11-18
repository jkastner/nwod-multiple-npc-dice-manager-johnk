using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerIntegration
{
    public class SiteQuery
    {
        public static string loginSite = "http://strange-aeons.herokuapp.com/login";
        
        private WebClientEx client = new WebClientEx();
        private string Username;
        private string Password;
        private string LoginSite;
        private string CharacterQueryURL;
        private const string LoginSiteCharactersToRemove = "login";
        private const string CharacterQueryURLExtension = "get?Type=Character&id=";
        private string BaseURL;
        public bool LoginSuccessful { get; set; }
        //private string CharacterQueryURLExtension = "http://strange-aeons.herokuapp.com/get?Type=Character&name"=;
        

        public SiteQuery(string loginSite, string username, string password)
        {
            Username = username;
            LoginSite = loginSite;
            Password = password;
            BaseURL = loginSite.Replace(LoginSiteCharactersToRemove, "");
            CharacterQueryURL = loginSite.Replace(LoginSiteCharactersToRemove, CharacterQueryURLExtension);

        }

        public List<JsonTranslationClasses.NWoDVampire.RootObject> QueryAllCharacters()
        {
            var ids = GameIDsForUser();
            var characters = CharactersForGames(ids.ToList());
            return characters.ToList();
        }

        private IEnumerable<JsonTranslationClasses.NWoDVampire.RootObject> CharactersForGames(List<int> gameIDsList)
        {
            var characterSiteQuery = BaseURL + "get?Type=Game&id=";
            foreach (var cur in gameIDsList)
            {
                var gameQuery = characterSiteQuery+cur;
                var info = client.DownloadString(gameQuery);
                var json = ParseJsonForGameQuery(info);
                if (json.success != null)
                {
                    foreach (var curCharacter in json.success.body.characters)
                    {
                        yield return QueryCharacter(curCharacter.id);
                    }
                }

            }
        }

        private IEnumerable<int> GameIDsForUser()
        {
            var userQueryString = BaseURL + "get?Type=User&name=" + Username;
            var info = client.DownloadString(userQueryString);
            var json = ParseJsonForUserQuery(info);
            if (json.success != null)
            {
                foreach (var cur in json.success.body.games)
                {
                    yield return cur.id;
                }
            }
        }

        private JsonTranslationClasses.QueryClasses.UserQuery.RootObject ParseJsonForUserQuery(string info)
        {
            var json = JsonConvert.DeserializeObject
                <JsonTranslationClasses.QueryClasses.UserQuery.RootObject>(info);
            //if (json.Success == null)
            //    return null;
            return json;
        }


        private JsonTranslationClasses.QueryClasses.GameQuery.RootObject ParseJsonForGameQuery(string info)
        {
            var json = JsonConvert.DeserializeObject
                <JsonTranslationClasses.QueryClasses.GameQuery.RootObject>(info);
            //if (json.Success == null)
            //    return null;
            return json;
        }
        public void Login()
        {
            var values = new NameValueCollection
                {
                    {"email", Username},
                    {"password", Password},
                };
            try
            {
                // Authenticate
                var info = client.UploadValues(loginSite, values);
                LoginSuccessful = true;
            }
            catch (WebException e)
            {
                LoginSuccessful = false;
            }
        }

        private JsonTranslationClasses.NWoDVampire.RootObject QueryCharacter(int targetCharacterID)
        {
            // Download desired page
            string characterSiteQuery = CharacterQueryURL + targetCharacterID;
            var info = client.DownloadString(characterSiteQuery);
            return ParseJsonForVampire(info);

        }

        private JsonTranslationClasses.NWoDVampire.RootObject ParseJsonForVampire(string info)
        {
            var json = JsonConvert.DeserializeObject<JsonTranslationClasses.NWoDVampire.RootObject>(info);
            if (json.success == null)
                return null;
            return json;
        }

        
    }
}
