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
        
        private int FindNumericTrait(string label)
        {
            return NumberTraits.FirstOrDefault(x => x.Label.ToLower().Equals(label.ToLower())).Contents;
        }

        public int Strength
        {
            get
            {
                return FindNumericTrait(
                    "Strength"
                    );
            }
        }

      
        public int Dexterity
        {
            get
            {
                return FindNumericTrait(
                    "Dexterity"
                    );
            }
        }

        public int Firearms
        {
            get
            {
                return FindNumericTrait(
                    "Firearms"
                    );
            }
        }
        public int Weaponry
        {
            get
            {
                return FindNumericTrait(
                    "Weaponry"
                    );
            }
        }
        public int Brawl
        {
            get
            {
                return FindNumericTrait(
                    "Brawl"
                    );
            }
        }
        public int Stamina
        {
            get
            {
                return FindNumericTrait(
                    "Stamina"
                    );
            }
        }
        public int Health
        {
            get
            {
                var stat1 = FindNumericTrait(
                    "Size"
                    );
                var stat2 = FindNumericTrait(
                    "Stamina"
                    );
                return stat1 + stat2;
            }
        }
        public int MeleeDefense
        {
            get
            {
                var stat1 = FindNumericTrait(
                    "Wits"
                    );
                if (stat1 < Dexterity)
                    return stat1;
                return Dexterity;
            }
        }

        public int Speed
        {
            get
            {
                var stat1 = FindNumericTrait(
                    "Size"
                    );
                return Strength + Dexterity + stat1;
            }
        }

        public int Height
        {
            get
            {
                return 5;
            }
        }

    }
}
