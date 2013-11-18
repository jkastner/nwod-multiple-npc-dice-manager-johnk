using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public abstract class TransferDataNWoDVampire : TransferDataBase
    {
        public abstract List<TransferTrait<String>> GetAllStringTraits();
        public abstract List<TransferTrait<int>> GetAllIntTraits();
        
    }
}
