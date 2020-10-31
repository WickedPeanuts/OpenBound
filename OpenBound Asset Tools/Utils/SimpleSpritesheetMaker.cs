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
using System.Windows.Forms;
using System.Threading;
using Openbound_Asset_Tools.Entity;
using Openbound_Asset_Tools.Common;
using System.Linq;
using Openbound_Asset_Tools.Helper;

namespace Openbound_Asset_Tools.Utils
{
    class SimpleSpritesheetMaker
    {
        private List<ImportedImage> _imgList;
        private RestartHelper _restartHelper = new RestartHelper();
        public void CreateSpritesheet()
        {
            _imgList = FileImportManager.ReadMultipleImages();

            Start();
            _restartHelper.RestartFunction(Start);
        }

        private void Start()
        {

            int width = _imgList[0].BitmapImage.Width;
            int height = _imgList[0].BitmapImage.Height;

            Console.WriteLine("--Sprites per Line: ");

            int spritesPerLine = int.Parse(Console.ReadLine());

            Color[][] newImageMatrix = ImageProcessing.CreateBlankColorMatrix((width + 2) * spritesPerLine, (height + 2) * (int)Math.Ceiling(_imgList.Count / (double)spritesPerLine));

            int accX = 1;
            int accY = 1;
            int index = 0;
            foreach (ImportedImage img in _imgList)
            {
                ImageProcessing.AddImageIntoMatrix(newImageMatrix, img.BitmapImage, accX, accY);

                accX += width + 2;

                if (++index % spritesPerLine == 0)
                {
                    accY += height + 2;
                    accX = 1;
                }
            }

            ImageProcessing.CreateImage(newImageMatrix).Save(Parameters.SpritesheetOutputDirectory + @"output.png");
            ExplorerHelper.OpenDirectory(Parameters.SpritesheetOutputDirectory);
            Thread.Sleep(1500);
            PaintHelper.OpenPictureFromOutputFolder(Parameters.SpritesheetOutputDirectory + @"output.png");
        }
    }
}
