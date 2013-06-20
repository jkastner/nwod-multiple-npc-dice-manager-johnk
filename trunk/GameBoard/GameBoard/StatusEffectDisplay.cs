using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    [DataContract(Namespace = "")]
    public class StatusEffectDisplay
    {
        [DataMember]
        public String Description { get; set; }
        [DataMember]
        public String TurnsRemaining { get; set; }
    }
}
