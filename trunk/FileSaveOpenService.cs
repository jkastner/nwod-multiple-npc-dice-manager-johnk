using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        static String fileSaveOpenFilter = "xml files (*.xml)|*.xml";
        internal static void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory()+"\\Saves";
            openFileDialog.Filter = fileSaveOpenFilter;
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    var savedCombat = FileSaveOpenService.ReadFromXML(openFileDialog.FileName, typeof(Combat)) as Combat;
                    CombatService.RosterViewModel.OpenRoster(savedCombat.ActiveRoster);
                    var allVisuals = savedCombat.ActiveRoster.Select(x => x.Visual);
                    CombatService.VisualsViewModel.OpenVisuals(allVisuals);
                    CombatService.VisualsViewModel.OpenBoardInfo(savedCombat.BoardInfo);
                    _previousFileName = openFileDialog.FileName;
                    CombatService.RosterViewModel.ClearResultText();
                    CombatService.RosterViewModel.ResultText = savedCombat.OutputText;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            
            
        }
        static string _previousFileName = "";
        internal static void SaveFile()
        {
            if(!string.IsNullOrWhiteSpace(_previousFileName))
            {
                SaveFile(_previousFileName);
            }
            else
            {
                SaveFileAs();
            }
        }

        internal static void SaveFileAs()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = fileSaveOpenFilter;
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Saves";
            saveFileDialog1.Title = "Save the current combat";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                SaveFile(saveFileDialog1.FileName);
            }
        }

        private static void SaveFile(String fileName)
        {
            Combat currentCombat = new Combat(CombatService.RosterViewModel.ActiveRoster, CombatService.RosterViewModel.DeceasedRoster, CombatService.VisualsViewModel.CurrentBoardInfo, CombatService.RosterViewModel.ResultText);
            WriteToXML(currentCombat, fileName, typeof(Combat));
            _previousFileName = fileName;
        }
    }
}
