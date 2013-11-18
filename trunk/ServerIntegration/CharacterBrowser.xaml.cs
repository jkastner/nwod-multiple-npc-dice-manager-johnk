using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerIntegration
{
    /// <summary>
    /// Interaction logic for CharacterBrowser.xaml
    /// </summary>
    public partial class CharacterBrowser : UserControl
    {

        private SiteQuery _site;
        private CharacterDownloadedReporter _characterDownloadedReporter;
        public CharacterBrowser()
        {
            InitializeComponent();
        }

        public void SetSiteQuery(SiteQuery site)
        {
            _site = site;
        }

        public void SetCharacterDownloadedReporter(CharacterDownloadedReporter cdr)
        {
            _characterDownloadedReporter = cdr;
        }

        private void ConductSearch_Button_Click(object sender, RoutedEventArgs e)
        {
            SearchResults_TextBlock.Text = "Searching...";
            RootObject foundData = _site.GetDataFromSite(NameQuery_SelectAllTextBox.Text);
            if (foundData != null)
            {
                SearchResults_TextBlock.Text = "Found " + foundData.CharacterName + " with ID " + foundData.CharacterID;
                AddData(foundData);
            }
            else
            {
                SearchResults_TextBlock.Text = "No results matching " + NameQuery_SelectAllTextBox.Text;
            }
        }

        private void AddData(RootObject foundData)
        {
            TransferCharacter readCharacter = null;
            if(foundData is TransferDataNWoDVampire)
            {
                readCharacter = new TransferCharacterNWoDVampire(foundData);
            }
            if (readCharacter != null && !AlreadyPresent(readCharacter))
            {
                AvailableCharacters_ListBox.Items.Add(readCharacter);
            }
        }

        private bool AlreadyPresent(TransferCharacter foundData)
        {
            foreach (var cur in AvailableCharacters_ListBox.Items)
            {
                var curTransfer = cur as TransferCharacter;
                if (curTransfer != null)
                {
                    if (curTransfer.Equals(foundData))
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        private void NameQuery_SelectAllTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConductSearch_Button_Click(sender, e);
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if(AvailableCharacters_ListBox.SelectedItem!=null)
            {
                SaveDefault(AvailableCharacters_ListBox.SelectedItem);
            }
        }

        private void SaveDefault(object cur)
        {
            var curChar = cur as TransferCharacter;
            var fileName = Directory.GetCurrentDirectory()+@"\"+curChar.Name + "_web.xml";
            Save(curChar, fileName);
        }

        private void Save(TransferCharacter transferCharacter, string fileName)
        {
            _characterDownloadedReporter.Report(transferCharacter, fileName));
        }

        private static String fileSaveOpenFilter = "xml files (*.xml)|*.xml";
        private void SaveAs_Button_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableCharacters_ListBox.SelectedItem == null)
            {
                return;
            }
            var curChar = AvailableCharacters_ListBox.SelectedItem as TransferCharacter;
            var saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = fileSaveOpenFilter;
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\Sheets";
            saveFileDialog1.Title = "Save the current character";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                Save(curChar, saveFileDialog1.FileName);
            }
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            _characterDownloadedReporter.RequestClose();
        }

        private void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableCharacters_ListBox.SelectedItem == null)
            {
                return;
            }
            var curChar = AvailableCharacters_ListBox.SelectedItem as TransferCharacter;
            _characterDownloadedReporter.Report(curChar, "");
        }


    }
}
