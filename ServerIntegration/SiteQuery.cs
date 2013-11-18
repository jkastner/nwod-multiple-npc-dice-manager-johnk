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
        private const string CharacterQueryURLExtension = "get?Type=Character&name=";
        public bool LoginSuccessful { get; set; }
        //private string CharacterQueryURLExtension = "http://strange-aeons.herokuapp.com/get?Type=Character&name=";
        

        public SiteQuery(string loginSite, string username, string password)
        {
            Username = username;
            LoginSite = loginSite;
            Password = password;
            CharacterQueryURL = loginSite.Replace(LoginSiteCharactersToRemove, CharacterQueryURLExtension);

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

        public RootObject GetDataFromSite(String targetCharacter)
        {
            // Download desired page
            string characterSiteQuery = CharacterQueryURL + targetCharacter;
            var info = client.DownloadString(characterSiteQuery);
            return ParseJson(info);

        }

        private RootObject ParseJson(string info)
        {
            var json = JsonConvert.DeserializeObject<RootObject>(info);
            if (json.success == null)
                return null;
            return json;
        }

        
    }
}
