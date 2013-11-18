using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class TransferCharacterNWoDVampire : TransferCharacter
    {
        public TransferCharacterNWoDVampire(TransferDataNWoDVampire data)
        {
            _name = data.CharacterName;
            _characterImageLocation = data.DownloadImageFromURLToLocal();
            GameID = data.GameID;
            ID = data.CharacterID;
            StringTraits = data.GetAllStringTraits();
            NumberTraits = data.GetAllIntTraits();
        }
        string _name;
        public override string Name
        {
            get
            {
                return _name;
            }
        }
        private String _characterImageLocation;
        public override string CharacterImageLocation
        {
            get { return _characterImageLocation; }
        }
        public override string SystemLabel
        {
            get { return TransferCharacter.NWoDSystemLabel; }
        } 

        private List<TransferTrait<String>> _stringTraits;
        public List<TransferTrait<String>> StringTraits
        {
            get { return _stringTraits; }
            set { _stringTraits = value; }
        }

        public override string SheetDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var cur in StringTraits)
                {
                    sb.AppendLine(cur.ToString());
                }
                foreach (var cur in NumberTraits)
                {
                    sb.AppendLine(cur.ToString());
                }
                return sb.ToString();
            }
        }

        private List<TransferTrait<int>> _numberTraits;
        public List<TransferTrait<int>> NumberTraits
        {
            get { return _numberTraits; }
            set { _numberTraits = value; }
        }
    }
}
