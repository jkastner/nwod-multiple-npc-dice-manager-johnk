using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ServerIntegration
{
    public class RootObject : TransferDataNWoDVampire
    {
        public int code { get; set; }
        public ServerIntegration.JsonContract.Success success { get; set; }

        internal CharacterData GetCharacter()
        {
            return success.body.data.Character;
        }

        public override string CharacterName
        {
            get { return success.body.name.name; }
        }

        public override int CharacterID
        {
            get
            {
                return success.body.data.Character.id;
            }
        }
        public override int GameID
        {
            get { return success.body.game.id; }
        }
        public override string ImageURL
        {
            get 
            {
                var site = success.body.picture;
                return site; 
            }
        }
        public override string SystemName
        {
            //Todo - where does Alex store this?
            get { return TransferCharacter.NWoDSystemLabel; }
        }

        public override List<TransferTrait<string>> GetAllStringTraits()
        {
            List<TransferTrait<String>> traits = new List<TransferTrait<string>>();

            foreach (var cur in success.body.data.Character.derangements.Values)
            {
                foreach (var curDerangement in cur)
                    traits.Add(new TransferTrait<String>("Derangement", curDerangement));
            }

            foreach (var curList in success.body.data.Character.specialties)
            {
                foreach (var curValue in curList.Value)
                    traits.Add(new TransferTrait<String>("Speciality under " + curList.Key, curValue));
            }

            var vice = success.body.data.Character.vice;
            traits.Add(new TransferTrait<string>("Vice", vice));
            var virtue = success.body.data.Character.virtue;
            traits.Add(new TransferTrait<string>("Virtue", virtue));
            return traits;


        }

        public override List<TransferTrait<int>> GetAllIntTraits()
        {
            List<TransferTrait<int>> traits = new List<TransferTrait<int>>();
            AddIntTrait(traits, "Total XP", success.body.data.Character.xp_total);
            AddIntTrait(traits, "XP diff", success.body.data.Character.xp_diff);
            AddIntTrait(traits, "Initiative", success.body.data.Character.initiative);
            AddIntTrait(traits, "Blood Potency", success.body.data.Character.potency);
            AddIntTrait(traits, "Size", success.body.data.Character.size);
            AddIntTrait(traits, "Morality", success.body.data.Character.morality);
            AddIntTrait(traits, "Vitae", success.body.data.Character.blood);
            foreach (var cur in success.body.data.Character.attributes)
            {
                AddIntTrait(traits, cur.Key, cur.Value);
            }
            foreach (var cur in success.body.data.Character.skills)
            {
                AddIntTrait(traits, cur.Key, cur.Value);
            }
            foreach (var cur in success.body.data.Character.merits)
            {
                AddIntTrait(traits, cur.Key, cur.Value);
            }
            foreach (var cur in success.body.data.Character.health)
            {
                AddIntTrait(traits, cur.Key, cur.Value);
            }
            foreach (var cur in success.body.data.Character.disciplines)
            {
                AddIntTrait(traits, cur.Key, cur.Value);
            }
            return traits;
        }

        private void AddIntTrait(List<TransferTrait<int>> traits, String label, int value)
        {
            traits.Add(new TransferTrait<int>(label, value));
        }
    }
}
