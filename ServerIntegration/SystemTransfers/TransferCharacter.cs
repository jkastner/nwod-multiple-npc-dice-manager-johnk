using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public abstract class TransferCharacter
    {
        public abstract String Name { get; }
        public abstract String CharacterImageLocation { get; }
        public abstract String SheetDescription { get; }
        public abstract String SystemLabel { get; }
        public const String NWoDSystemLabel = "NWoD";
        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; }
        }
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public override bool Equals(object obj)
        {
            var curTransfer = obj as TransferCharacter;
            if (curTransfer == null)
                return false;
            if (curTransfer.GameID == GameID &&
                curTransfer.ID == ID)
            {
                return true;
            }
            return false;

        }

    }
}
