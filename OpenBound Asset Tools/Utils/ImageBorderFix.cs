using Openbound_Asset_Tools.Common;
using Openbound_Asset_Tools.Entity;
using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Utils
{
    public class ImageBorderFix
    {
        /// <summary>
        /// Applies an erosion on each border of the image, excluding possible erros in it
        /// </summary>
        public static void FixBorder()
        {
            Bitmap bmp = FileImportManager.ReadSingleImage().BitmapImage;

            int[][][] imageChannels = ImageProcessing.SplitImageChannels(bmp);

            imageChannels[0] = ImageProcessing.Erode(imageChannels[0]);
            imageChannels[1] = ImageProcessing.Erode(imageChannels[1]);
            imageChannels[2] = ImageProcessing.Erode(imageChannels[2]);
            imageChannels[3] = ImageProcessing.Erode(imageChannels[3]);

            ImageProcessing.CreateImage(ImageProcessing.JoinImageChannels(imageChannels)).Save(@$"{Parameters.FixedImageOutputDirectory}output.png");
        }
    }
}
