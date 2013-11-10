using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
        
        public static ImageBrush MakeImageMaterial(String imageLocation)
        {
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource =
                new BitmapImage(
                    new Uri(imageLocation, UriKind.Relative)

                )
                {
                    
                };
                        
            return imageBrush;
        }

        public static Material PaperbackMaterial()
        {
            var expected = GameboardConstants.MapPictureDirectory + "\\" + GameboardConstants.BackgroundName;
            if (File.Exists(expected))
            {
                return new DiffuseMaterial(MakeImageMaterial(expected));
            }
            return DefaultBackMaterial;
        }
        public static Material DefaultFrontMaterial
        {
            get
            {
                return Materials.Green;
            }
        }
        private static Material DefaultBackMaterial
        {
            get
            {
                return Materials.LightGray;
            }
        }
    }
}
