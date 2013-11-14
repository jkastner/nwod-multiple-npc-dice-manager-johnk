using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerIntegration
{
    public class SiteQuery
    {

        static WebClientEx client = new WebClientEx();

        public static JsonContract.Character RetrieveCharacter(string targetCharacter, string loginSite, string characterQueryURL, string username, string password)
        {
            var values = new NameValueCollection
                {
                    {"email", username},
                    {"password", password},
                };
            // Authenticate
            client.UploadValues(loginSite, values);
            // Download desired page
            string characterSiteQuery = characterQueryURL + targetCharacter;
            var info = client.DownloadString(characterSiteQuery);
            return ParseJson(info);
        }

        private static JsonContract.Character ParseJson(string info)
        {
            var json = JsonConvert.DeserializeObject<JsonContract.RootObject>(info);
            if (json.success == null)
                return null;
            return json.success.body.data.Character;
        }
    }
}
