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

using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Openbound_Asset_Tools.Utils
{
    public class ImageSyncComparer
    {
        public static void ImageSyncCompare()
        {
            string path = @"C:\Users\Carlos\source\repos\OpenBound\OpenBound\bin\Windows\x86\Debug\SyncDebug";

            List<Bitmap> wickedList = new List<Bitmap>();
            List<Bitmap> wingedList = new List<Bitmap>();

            List<string> wickedStrList = new List<string>();
            List<string> wingedStrList = new List<string>();

            foreach (string s in Directory.GetFiles(path).ToList().OrderBy((x) => x))
            {
                Console.WriteLine(s);
                Bitmap b = new Bitmap(s);

                if (s.Contains("Wicked"))
                {
                    wickedList.Add(b);
                    wickedStrList.Add(s);
                }
                else
                {
                    wingedList.Add(b);
                    wingedStrList.Add(s);
                }
            }

            for (int i = 0; i < wingedList.Count; i++)
            {
                Console.WriteLine("\nComparing: " + wickedStrList[i].Split('\\').Last() + " with " + wingedStrList[i].Split('\\').Last());
                //ImageProcessing.CompareImages(wickedList[i], wingedList[i], int.Parse(wickedStrList[i].Split(' ').Last().Replace(".png", "")));
            }
        }
    }
}
