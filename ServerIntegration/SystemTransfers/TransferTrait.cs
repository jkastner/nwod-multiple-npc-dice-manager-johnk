using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class TransferTrait <T>
    {
        public T Contents { get; set; }
        public String Label { get; set; }
        public TransferTrait(String label, T contents)
        {
            Label = label;
            Contents = contents;
        }
        public override string ToString()
        {
            return Label + ": " + Contents.ToString();
        }
    }
}
