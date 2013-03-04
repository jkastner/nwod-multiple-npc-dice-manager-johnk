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
        public static ImageBrush MakeImageMaterial(String imageLocation)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource =
                new BitmapImage(
                    new Uri(imageLocation, UriKind.Relative)
                );
            return imageBrush;
        }

        public static Material PaperbackMaterial()
        {
            return new DiffuseMaterial(MakeImageMaterial(boardBackground));
        }
    }
}
