 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using GameBoard;
using Microsoft.Win32;

namespace XMLCharSheets
{
    public class FileSaveOpenService
    {
        private static String fileSaveOpenFilter = "xml files (*.xml)|*.xml";
        private static string _previousFileName = "";

        public static void WriteToXML(Object someObject, String fileName, Type theType)
        {
            var settings = new XmlWriterSettings {Indent = true};
            var ser = new DataContractSerializer(theType, null, int.MaxValue, false, true, null);
            using (XmlWriter w = XmlWriter.Create(fileName, settings))
                ser.WriteObject(w, someObject);
        }

        public static Object ReadFromXML(string p0, Type theType)
        {
            using (var reader = new FileStream(p0, FileMode.Open, FileAccess.Read))
            {
                var ser = new DataContractSerializer(theType);
                return ser.ReadObject(reader);
            }
        }

        internal static void OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\Saves";
            openFileDialog.Filter = fileSaveOpenFilter;
            if ((bool) openFileDialog.ShowDialog())
            {
                try
                {
                    var savedCombat = ReadFromXML(openFileDialog.FileName, typeof (Combat)) as Combat;
                    CombatService.RosterViewModel.OpenActiveRoster(savedCombat.ActiveRoster);
                    CombatService.RosterViewModel.OpenDeceasedRoster(savedCombat.DeceasedRoster);
                    CombatService.RosterViewModel.OpenTeams(savedCombat.Teams);
                    BoardsViewModel.Instance.ClearAllBoards();
                    foreach (var cur in savedCombat.Boards)
                    {
                        BoardsViewModel.Instance.ImportBoardFromSave(cur);
                    }
                    _previousFileName = openFileDialog.FileName;
                    CombatService.RosterViewModel.ClearResultText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        internal static void SaveFile()
        {
            if (!string.IsNullOrWhiteSpace(_previousFileName))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_previousFileName)))
                {
                    SaveFileAs();
                    return;
                }
                SaveFile(_previousFileName);
            }
            else
            {
                SaveFileAs();
            }
        }

        internal static void SaveFileAs()
        {
            var saveFileDialog1 = new SaveFileDialog();
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
            var currentCombat = new Combat(CombatService.RosterViewModel.ActiveRoster,
                                           CombatService.RosterViewModel.DeceasedRoster,
                                           CombatService.RosterViewModel.Teams,
                                           VisualsService.BoardsViewModel.Boards,
                                           String.Empty);
            WriteToXML(currentCombat, fileName, typeof (Combat));
            _previousFileName = fileName;
        }
    }
}