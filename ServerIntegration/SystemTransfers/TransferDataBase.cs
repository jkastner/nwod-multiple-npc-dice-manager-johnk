using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerIntegration
{
    public abstract class TransferDataBase
    {
        public abstract int GameID { get; }
        public abstract int CharacterID { get; }
        public abstract String ImageURL { get; }
        public string DownloadImageFromURLToLocal()
        {
            string localFilename = @"ServerImages\" + CharacterName + "GID_" + GameID + "ID_" + CharacterID + "_web.png";
            if (!File.Exists(localFilename))
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(ImageURL, localFilename);
                    }
                }
                catch (Exception e)
                {
                    return string.Empty;
                }
            }
            return Directory.GetCurrentDirectory() + "\\"+localFilename;
        }
        public abstract String SystemName { get; }

        public abstract String CharacterName { get; }

    }
}
