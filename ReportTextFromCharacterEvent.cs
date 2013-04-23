using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class ReportTextFromCharacterEvent : EventArgs
    {
        private String _reportedText;
        public String ReportedText
        {
            get { return _reportedText; }
            set { _reportedText = value; }
        }

        public ReportTextFromCharacterEvent(String text)
        {
            _reportedText = text;
        }
        
    }
}
