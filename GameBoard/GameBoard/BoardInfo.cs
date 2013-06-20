using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameBoard
{
    [DataContract]
    public class BoardInfo
    {
        [DataMember]
        internal string BoardImageName;
        [DataMember]
        internal double DefinedBoardHeight;
        [DataMember]
        internal double DefinedBoardWidth;
        [DataMember]
        internal bool MaintainRatio;

        public BoardInfo(string newImage, double definedBoardHeight, double definedBoardWidth, bool maintainRatio)
        {
            this.BoardImageName = newImage;
            this.DefinedBoardHeight = definedBoardHeight;
            this.DefinedBoardWidth = definedBoardWidth;
            this.MaintainRatio = maintainRatio;
        }

    }
}
