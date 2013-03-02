using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLCharSheets
{
    public class PictureSelectionViewModel
    {
        private ObservableCollection<PictureFileInfo> _loadedPictures = new ObservableCollection<PictureFileInfo>();
        public ObservableCollection<PictureFileInfo> AllLoadedPictures
        {
            get { return _loadedPictures; }
            set { _loadedPictures = value; }
        }

        private ObservableCollection<PictureFileInfo> _activeLoadedPictures = new ObservableCollection<PictureFileInfo>();
        public ObservableCollection<PictureFileInfo> ActiveLoadedPictures
        {
            get { return _activeLoadedPictures; }
            set { _activeLoadedPictures = value; }
        }

        string picTarget = Directory.GetCurrentDirectory() + @"\PiecePictures";
        public PictureSelectionViewModel()
        {
            LoadPictures();
            ResetActiveList();
        }

        private void LoadPictures()
        {
            string[] pictureFiles = Directory.GetFiles(picTarget, "*.*", SearchOption.AllDirectories);
            foreach(var cur in pictureFiles)
            {
                AllLoadedPictures.Add(new PictureFileInfo(cur, Path.GetFileNameWithoutExtension(cur)));
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
            var matches = AllLoadedPictures.Where(x => x.PictureName.ToLower().Contains(searchTerm));
            foreach (var curMatch in matches)
                ActiveLoadedPictures.Add(curMatch);
        }

        public void ResetActiveList()
        {
            if (AllLoadedPictures.Count() != ActiveLoadedPictures.Count())
            {
                ActiveLoadedPictures.Clear();
                foreach (var cur in AllLoadedPictures)
                {
                    ActiveLoadedPictures.Add(cur);
                }
            }
        }

    }
    public class PictureFileInfo
    {
        private String _pictureName = "";

        public String PictureName
        {
            get { return _pictureName; }
            set
            {
                _pictureName = value;
            }
        }

        private String _pictureFile = "";
        public PictureFileInfo(string filePath, string fileName)
        {
            PictureName = fileName;
            PictureFile = filePath;
        }
        public String PictureFile
        {
            get { return _pictureFile; }
            set
            {
                _pictureFile = value;
            }
        }
	
    }
}
