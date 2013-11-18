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

        private static PictureSelectionViewModel _instance;
        public static PictureSelectionViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PictureSelectionViewModel();
                }
                return _instance;
            }
        }


        
        private PictureSelectionViewModel()
        {
            LoadPictures();
            ResetActiveList();
        }

        public void AddImage(String imagePath)
        {
            PictureFileInfo newinfo = MakePictureInfoFromPath(imagePath);
            if (!AllLoadedPictures.Contains(newinfo))
            {
                AllLoadedPictures.Add(newinfo);
            }
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
                AllLoadedPictures.Add(MakePictureInfoFromPath(cur));
            }
        }

        private PictureFileInfo MakePictureInfoFromPath(string cur)
        {
            string currentDir = Directory.GetCurrentDirectory() + "\\";
            string correctedPath = cur.Replace(currentDir, "");
            return new PictureFileInfo(correctedPath, Path.GetFileNameWithoutExtension(cur));
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
        public override bool Equals(object obj)
        {
            var pfi = obj as PictureFileInfo;
            if (pfi == null)
                return false;
            return pfi.PictureFileAbsolutePath.Equals(PictureFileAbsolutePath);
        }
    }
}