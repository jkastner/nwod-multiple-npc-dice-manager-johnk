using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace GameBoard
{
    public class MaterialMaker
    {

        static String boardBackground = @"MapPictures\SquarePaper.jpg";
        public static Material MakeImageMaterial(String imageLocation)
        {
            ImageBrush imageBrush = new ImageBrush();


            imageBrush.ImageSource =
                new BitmapImage(
                    new Uri(imageLocation, UriKind.Relative)
                );
            Material image = new DiffuseMaterial(imageBrush);
            return image;
        }

        public static Material PaperbackMaterial()
        {
            return MakeImageMaterial(boardBackground);
        }
    }
}
