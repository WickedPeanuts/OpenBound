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
using Openbound_Asset_Tools.Helper;
using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Openbound_Asset_Tools.Utils
{
    /// <summary>
    /// Creates a spritesheet given a two-layer image sets.
    /// This method is suited for the following creations: Mobile, Multi-Layered Projectiles.
    /// </summary>
    public class MultiLayerSpritesheetMaker
    {
        private List<ImportedImage> _imgListSFX, _imgList;
        private RestartHelper _restartHelper = new RestartHelper();

        public void CreateSpritesheet()
        {
            Console.WriteLine("\n\nSelect the main layer image files including the TXT file: \n");
            _imgList = FileImportManager.ReadMultipleImagesWithPivot();

            Console.WriteLine("\n\nSelect the alternative layer image files including the TXT file: \n");
            _imgListSFX = FileImportManager.ReadMultipleImagesWithPivot();

            Start();
            _restartHelper.RestartFunction(Start);
        }

        public void Start()
        {
            List<ImportedImage> imgListSFX = _imgListSFX.ToList();
            List<ImportedImage> imgList = _imgList.ToList();

            int[] imagePerLayer = new int[] { imgList.Count, imgListSFX.Count };

            imgList.AddRange(imgListSFX);

            (int, int) maxImageSize = (imgList.Max((x) => x.BitmapImage.Width), imgList.Max((x) => x.BitmapImage.Height));
            (int, int) maxImagePivot = (imgList.Max((x) => x.Pivot.Item1), imgList.Max((x) => x.Pivot.Item2));
            (int, int) minImagePivot = (imgList.Min((x) => x.Pivot.Item1), imgList.Min((x) => x.Pivot.Item2));

            (int, int) newSize = (
                maxImageSize.Item1 + Math.Max(Math.Abs(minImagePivot.Item1), Math.Abs(minImagePivot.Item1)),
                maxImageSize.Item2 + Math.Max(Math.Abs(maxImagePivot.Item2), Math.Abs(minImagePivot.Item2)));

            Console.WriteLine("Sprites per line: ");
            int imgPerLine = int.Parse(Console.ReadLine());

            Console.WriteLine("Squish X factor:");
            int squishXFactor = int.Parse(Console.ReadLine());

            Console.WriteLine("Squish Y factor:");
            int squishYFactor = int.Parse(Console.ReadLine());

            Console.WriteLine("Initial X Shift factor:");
            int initialXFactor = int.Parse(Console.ReadLine());

            Console.WriteLine("Initial Y Shift factor:");
            int initialYFactor = int.Parse(Console.ReadLine());

            (int, int) newBigImageSize = ((newSize.Item1 - squishXFactor + initialXFactor) * imgPerLine, (newSize.Item2 - squishYFactor) * (int)Math.Ceiling((double)imagePerLayer[0] / imgPerLine));

            Color[][] nCM1 = ImageProcessing.CreateBlankColorMatrix(newBigImageSize.Item1, newBigImageSize.Item2);
            Color[][] nCM2 = ImageProcessing.CreateBlankColorMatrix(newBigImageSize.Item1, newBigImageSize.Item2);

            int index = 0;
            int w = 0;
            int h = 0;

            bool reaply = false;

            foreach (ImportedImage img in imgList)
            {
                w = initialXFactor + /* * (1 + (index % imgPerLine))*/ + (newSize.Item1 - squishXFactor) * (index % imgPerLine) + newSize.Item1 / 2 + img.Pivot.Item1;
                h = initialYFactor + (newSize.Item2 - squishYFactor) * (index / imgPerLine) + newSize.Item2 / 2 + img.Pivot.Item2;

                if (!reaply)
                {
                    ImageProcessing.AddImageIntoMatrix(nCM1, img.BitmapImage, w, h);
                    ImageProcessing.AddImageIntoMatrix(nCM2, img.BitmapImage, w, h);
                }
                else
                {
                    ImageProcessing.BlendImageIntoMatrix(nCM1, img.BitmapImage, w, h, (y, x) =>
                    {
                        return ColorBlending.MultiChannelAlphaBlending(x, y);
                    });

                    ImageProcessing.BlendImageIntoMatrix(nCM2, img.BitmapImage, w, h, (x, y) =>
                    {
                        return ColorBlending.MultiChannelAlphaBlending(x, y);
                    });
                }

                if (++index == imagePerLayer[0] && !reaply)
                {
                    reaply = true;
                    index = 0;
                }
            }

            ImageProcessing.BlendImageIntoMatrix(nCM1, ImageProcessing.CreateImage(nCM2), 0, 0, (x, y) =>
            {
                return Color.FromArgb(Math.Max(x.A, y.A), Math.Max(x.R, y.R), Math.Max(x.G, y.G), Math.Max(x.B, y.B));
            });

            ImageProcessing.CreateImage(nCM1).Save(Parameters.SpritesheetOutputDirectory + @"output.png");

            ExplorerHelper.OpenDirectory(Parameters.SpritesheetOutputDirectory);
            Thread.Sleep(1500);
            PaintHelper.OpenPictureFromOutputFolder(Parameters.SpritesheetOutputDirectory + @"output.png");
        }
    }
}
