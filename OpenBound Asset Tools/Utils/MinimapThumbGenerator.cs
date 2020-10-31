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

namespace Openbound_Asset_Tools.Utils
{
    class MinimapThumbGenerator
    {
        const string Path = @"C:\Users\Carlos\source\repos\OpenBound\OpenBound\Content\Graphics\Maps";

        public static void GenerateButtonThumbnails()
        {
            string[] folder = Directory.GetDirectories(Path);

            foreach (string dir in folder)
            {
                GenerateThumbForMap(dir);
            }
        }

        public static void GenerateThumbForMap(string dir)
        {
            try
            {
                Bitmap bg = (Bitmap)Image.FromFile(dir + "/Background.png");
                List<Bitmap> bmpList = new List<Bitmap>();

                char startingChar = 'A';

                int i = 0;
                while (File.Exists(dir + $"/Foreground{(char)(startingChar + i)}.png"))
                {
                    bmpList.Add((Bitmap)Image.FromFile(dir + $"/Foreground{(char)(startingChar + i)}.png"));
                    i++;
                }

                for (i = 0; i < bmpList.Count; i++)
                {
                    GenerateGameButtonThumb(bg, bmpList[i], dir, (char)(startingChar + i));
                    GenerateGameMapThumb(bg, bmpList[i], dir, (char)(startingChar + i));
                }
            }
            catch { }
        }

        public static void GenerateGameButtonThumb(Bitmap bg, Bitmap fg, string path, char setting)
        {
            Bitmap frame = (Bitmap)Image.FromFile(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Files\MinimapFrame.png");
            int x = 92, y = 27;
            //int xx = 271, yy = 61;

            //float bgNewScale = Math.Max(x / (float)bg.Width, y / (float)bg.Height);
            //float fgNewScale = Math.Max(x / (float)fg.Width, y / (float)fg.Height);

            ThumbResize(x, y, frame, bg, fg, "GameListThumb", setting);
        }


        public static void GenerateGameMapThumb(Bitmap bg, Bitmap fg, string path, char setting)
        {
            Bitmap frame = (Bitmap)Image.FromFile(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Files\GameRoomFrame.png");
            int x = 209, y = 46;
            //int xx = 271, yy = 61;

            //float bgNewScale = Math.Max(x / (float)bg.Width, y / (float)bg.Height);
            //float fgNewScale = Math.Max(x / (float)fg.Width, y / (float)fg.Height);

            ThumbResize(x, y, frame, bg, fg, "GameRoomThumb", setting);
        }

        public static void ThumbResize(int x, int y, Bitmap frame, Bitmap bg, Bitmap fg, string output, char setting)
        {
            Color[][] bgMat = ImageProcessing.CreateMatrix(ImageProcessing.ResizeImage(x, x, bg));
            Color[][] fgMat = ImageProcessing.CreateMatrix(ImageProcessing.ResizeImage(x, x, fg));

            //dest, source, offset
            Bitmap fgImage = ImageProcessing.CreateImage(fgMat);
            ImageProcessing.BlendImageIntoMatrix(bgMat, fgImage, 0, 0, (yy, xx) =>
            {
                return Color.FromArgb(Math.Max(xx.A, yy.A),
                    (byte)CalculatePixel(xx.R, xx.A, yy.R, yy.A),
                    (byte)CalculatePixel(xx.G, xx.A, yy.G, yy.A),
                    (byte)CalculatePixel(xx.B, xx.A, yy.B, yy.A));
            });

            Bitmap bgImage = ImageProcessing.CreateImage(bgMat);

            Color[][] newMat = ImageProcessing.CreateMatrix(bgImage);

            int offsetY;
            int bestY = 0, bestCount = 0;
            for (offsetY = 0; offsetY < fgMat.Length; offsetY++)
            {
                int midCount = 0;
                for (int w = 0; w < fgMat[0].Length; w++)
                {
                    if (fgMat[offsetY][w].A != 0)
                        midCount++;
                }

                if (midCount > bestCount)
                {
                    bestY = offsetY;
                    bestCount = midCount;
                }
            }

            //Final Image
            Bitmap fImage = ImageProcessing.CropImage(newMat, x, y, Math.Min(newMat.Length - y, bestY - y / 3));
            Color[][] frameMat = ImageProcessing.CreateMatrix(frame);

            ImageProcessing.AddImageIntoMatrix2(frameMat, fImage, 1, 1);

            ImageProcessing.CreateImage(frameMat).Save($@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Thumb\{output}{setting}.png");
        }

        public static float CalculatePixel(float ca, float aa, float cb, float ab)
        {
            aa /= 255;
            ab /= 255;
            return (ca * aa + cb * ab * (1 - aa)) / (aa + ab * (1 - aa));
        }
    }
}
