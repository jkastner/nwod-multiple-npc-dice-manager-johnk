using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class CharacterDownloadedReporter
    {
        public event EventHandler WebCharacterCreated;
        private void OnWebCharacterCreated(WebCharacterCreatedEventArgs e)
        {
            EventHandler handler = WebCharacterCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        public event EventHandler CloseRequested;
        private void OnRequestClose(EventArgs e)
        {
            EventHandler handler = CloseRequested;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        internal void Report(TransferCharacter transferCharacter, string fileName)
        {
            OnWebCharacterCreated(new WebCharacterCreatedEventArgs(transferCharacter, fileName);
        }

        internal void RequestClose()
        {
            OnRequestClose(new EventArgs());
        }
    }
}
