 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;
using GameBoard;
using Microsoft.Win32;

namespace CombatAutomationTheater
{
    public class FileSaveOpenService
    {
        private static String fileSaveOpenFilter = "xml files (*.xml)|*.xml";
        private static string _previousFileName = "";
        private static string _previousFileFullPath = "";

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

        internal static IList<Board> OpenFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\Saves";
            openFileDialog.Filter = fileSaveOpenFilter;
            if ((bool) openFileDialog.ShowDialog())
            {
                try
                {
                    CombatService.RosterViewModel.ResetOnOpen();

                    var savedCombat = ReadFromXML(openFileDialog.FileName, typeof (Combat)) as Combat;
                    for (int curIndex = 0; curIndex < savedCombat.Boards.Count(); curIndex++)
                    {
                        var curBoard = savedCombat.Boards[curIndex];
                        BoardsViewModel.Instance.ImportBoardFromSave(curBoard);
                    }

                    CombatService.RosterViewModel.OpenActiveRoster(savedCombat.ActiveRoster);
                    CombatService.RosterViewModel.OpenDeceasedRoster(savedCombat.DeceasedRoster);
                    CombatService.RosterViewModel.OpenTeams(savedCombat.Teams);


                    SetCurrentSaveFileName(openFileDialog.FileName);
                    CombatService.RosterViewModel.ClearResultText();
                    return savedCombat.Boards;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                return null;
            }
            return null;
        }

        private static void SetCurrentSaveFileName(string fullPath)
        {
 	        _previousFileFullPath = fullPath;
            _previousFileName = Path.GetFileNameWithoutExtension(fullPath);
        }

        internal static void SaveFile()
        {
            if (!string.IsNullOrWhiteSpace(_previousFileFullPath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(_previousFileFullPath)))
                {
                    SaveFileAs();
                    return;
                }
                Save(_previousFileFullPath);
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
                Save(saveFileDialog1.FileName);
                SetCurrentSaveFileName(saveFileDialog1.FileName);
            }
        }

        private static void Save(String fileName)
        {
            var currentCombat = new Combat(CombatService.RosterViewModel.ActiveRoster,
                                           CombatService.RosterViewModel.DeceasedRoster,
                                           CombatService.RosterViewModel.Teams,
                                           VisualsService.BoardsViewModel.Boards,
                                           String.Empty);
            WriteToXML(currentCombat, fileName, typeof (Combat));
        }

        internal static void AutoSave(string currentRound)
        {
            if (Directory.Exists(@"Saves\Autosaves"))
            {
                string fileName = string.Format("Autosave-{0:yyyy-MM-dd_hh-mm-ss-tt}.xml", DateTime.Now);
                if (!String.IsNullOrWhiteSpace(_previousFileName))
                {
                    fileName = _previousFileName + currentRound;
                }
                Save(@"Saves\Autosaves\" + fileName+".xml");
            }
            else
            {
                MessageBox.Show("Could not autosave - directory Saves\\Autosaves not found");
            }
        }
    }
}