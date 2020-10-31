/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using Openbound_Asset_Tools.Common;
using Openbound_Asset_Tools.Entity;
using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Openbound_Asset_Tools.Utils
{
    class ImageComparer
    {
        /// <summary>
        /// Compare and check if two images are identical
        /// </summary>
        public static void CompareImages()
        {
            List<ImportedImage> list1, list2;
         
            Console.WriteLine("Importing first set of image files");
            list1 = FileImportManager.ReadMultipleImages();

            Console.WriteLine("Importing first set of image files");
            list2 = FileImportManager.ReadMultipleImages();

            for (int i = 0; i < list1.Count; i++)
            {
                ImageProcessing.CompareImages(list1[i].BitmapImage, list2[i].BitmapImage, Parameters.ComparisonOutputDirectory + $@"{i}.png");
            }
        }
    }
}
