using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class WebCharacterCreatedEventArgs : EventArgs
    {
        public TransferCharacter TransferCharacter
        {
            get;
            set;
        }
        public String FileName { get; set; }
        public WebCharacterCreatedEventArgs(TransferCharacter tc, String fileName)
        {
            TransferCharacter = tc;
            FileName = fileName;
        }
    }
}
