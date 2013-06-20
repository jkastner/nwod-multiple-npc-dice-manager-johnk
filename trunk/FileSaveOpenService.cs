using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLCharSheets
{
    public class FileSaveOpenService
    {
        public static void WriteToXML(Object someObject, String fileName, Type theType)
        {

            var settings = new XmlWriterSettings { Indent = true };
            DataContractSerializer ser = new DataContractSerializer(theType, null, int.MaxValue, false, true, null);
            using (var w = XmlWriter.Create(fileName, settings))
                ser.WriteObject(w, someObject);
        }

        public static Object ReadFromXML(string p0, Type theType)
        {
            using (FileStream reader = new FileStream(p0, FileMode.Open, FileAccess.Read))
            {
                DataContractSerializer ser = new DataContractSerializer(theType);
                return ser.ReadObject(reader);
            }
        }

        internal static void OpenSaveFile()
        {
            var newRoster = FileSaveOpenService.ReadFromXML("ActiveCharacters.xml", typeof(ObservableCollection<CharacterSheet>)) as ObservableCollection<CharacterSheet>;
            CombatService.CurrentCombat.RosterViewModel.OpenRoster(newRoster);
            var allVisuals = newRoster.Select(x => x.Visual);
            CombatService.CurrentCombat.VisualsViewModel.OpenVisuals(allVisuals);
        }
    }
}
