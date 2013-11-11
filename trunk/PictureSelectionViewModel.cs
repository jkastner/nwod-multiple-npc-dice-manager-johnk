using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace XMLCharSheets
{
    public class PictureSelectionViewModel
    {
        private string picTarget = Directory.GetCurrentDirectory() + @"\PiecePictures";

        private ObservableCollection<PictureFileInfo> _activeLoadedPictures =
            new ObservableCollection<PictureFileInfo>();

        private ObservableCollection<PictureFileInfo> _loadedPictures = new ObservableCollection<PictureFileInfo>();

        public PictureSelectionViewModel()
        {
            LoadPictures();
            ResetActiveList();
        }

        public ObservableCollection<PictureFileInfo> AllLoadedPictures
        {
            get { return _loadedPictures; }
            set { _loadedPictures = value; }
        }

        public ObservableCollection<PictureFileInfo> ActiveLoadedPictures
        {
            get { return _activeLoadedPictures; }
            set { _activeLoadedPictures = value; }
        }

        private void LoadPictures()
        {
            string[] pictureFiles = Directory.GetFiles(picTarget, "*.*", SearchOption.AllDirectories);
            foreach (string cur in pictureFiles)
            {
                string currentDir = Directory.GetCurrentDirectory() + "\\";
                string correctedPath = cur.Replace(currentDir, "");
                AllLoadedPictures.Add(new PictureFileInfo(correctedPath, Path.GetFileNameWithoutExtension(cur)));
            }
        }

        internal void AdjustList(string searchTerm)
        {
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                ResetActiveList();
                return;
            }
            ActiveLoadedPictures.Clear();
            IEnumerable<PictureFileInfo> matches =
                AllLoadedPictures.Where(x => x.PictureName.ToLower().Contains(searchTerm));
            foreach (PictureFileInfo curMatch in matches)
                ActiveLoadedPictures.Add(curMatch);
        }

        public void ResetActiveList()
        {
            if (AllLoadedPictures.Count() != ActiveLoadedPictures.Count())
            {
                ActiveLoadedPictures.Clear();
                foreach (PictureFileInfo cur in AllLoadedPictures)
                {
                    ActiveLoadedPictures.Add(cur);
                }
            }
        }
    }

    public class PictureFileInfo
    {
        private String _pictureFile = "";
        private String _pictureName = "";

        public PictureFileInfo(string filePath, string fileName)
        {
            PictureName = fileName;
            PictureFile = filePath;
        }

        public String PictureName
        {
            get { return _pictureName; }
            set { _pictureName = value; }
        }

        public String PictureFile
        {
            get { return _pictureFile; }
            set { _pictureFile = value; }
        }

        public String PictureFileAbsolutePath
        {
            get
            {
                string val = Directory.GetCurrentDirectory() + "\\" + PictureFile;
                return val;
            }
        }
    }
}