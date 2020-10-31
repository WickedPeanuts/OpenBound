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
using Openbound_Asset_Tools.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Openbound_Asset_Tools.Utils
{
    public class IMGCracker
    {
        /// <summary>
        /// Exports the IMG data saving it on RawOutputDirectory folder.
        /// </summary>
        public static void ExportIMGData()
        {
            List<string> paths = FileImportManager.ReadMultipleIMGFiles();

            for (int i = 0; i < paths.Count; i++)
            {
                List<ImportedImage> importedImageList = ExtractIMGData(paths[i]);
                string filePath = paths[i];
                string fileName = filePath.Split('\\').Last().Split('.').First();

                string pivotList = "[\n";
                string pivotFileStr = $"{Parameters.PivotFileHeader}\n";
                Directory.CreateDirectory($@"{Parameters.RawOutputDirectory}\{fileName}");
                for (int j = 0; j < importedImageList.Count; j++)
                {
                    ImportedImage ii = importedImageList[j];

                    pivotFileStr += $"{j},{ii.Pivot.Item1},{ii.Pivot.Item2},{ii.RiderOffset.Item1},{ii.RiderOffset.Item2},{ii.RealRiderOffset.Item1},{ii.RealRiderOffset.Item2}\n";
                    pivotList += $"[ {ii.RealRiderOffset.Item1}, {ii.RealRiderOffset.Item2} ],\n";
                    ii.BitmapImage.Save($@"{Parameters.RawOutputDirectory}\{fileName}\{fileName}_{j}.png");
                }

                pivotList += "]";

                Console.WriteLine($"Imported: {paths[i]}");
                File.WriteAllText($@"{Parameters.RawOutputDirectory}\{fileName}\{fileName}.txt", pivotFileStr);
                File.WriteAllText($@"{Parameters.RawOutputDirectory}\{fileName}\RiderPivot.txt", pivotList);
            }
        }

        /// <summary>
        /// Extracts the IMG file data and returns a list of ImportedImage.
        /// </summary>
        public static List<ImportedImage> ExtractIMGData(string fileName)
        {
            byte[] imgByteArray = File.ReadAllBytes(fileName);

            // I have decided to parse the whole byte array into queue to make things easier to understand
            Queue<byte> valueQueue = new Queue<byte>();

            for (int i = 0; i < imgByteArray.Length; i++)
                valueQueue.Enqueue(imgByteArray[i]);

            // File data:
            //
            //  bits  |   i      | value
            //   2x16 |   0      | zero - ? 
            //   2x16 |   1      | number of images
            //   2x16 |   2  (0) | color mode, 0 = no alpha (565 rgb), 1 = simple transparency, 2 = transparency (4444 rgba)
            //  ------+----------+ Loop starts here
            //     32 |   3  (1) | width
            //     32 |   4  (2) | height
            //     32 |   5  (3) | pivot X
            //     32 |   6  (4) | pivot Y
            //    4x8 |   7  (5) | ? - v5
            //    4x8 |   8  (6) | ? - v6
            //    4x8 |  11  (9) | pilot pivot X
            //    4x8 |  14 (10) | pilot pivot Y
            //     32 |  15 (11) | Data stream length
            //(11)x16 |  16 (12) | Image Begining...
            //     16 |  17+(11) | Image Ending
            // ------+----------+ Loop ends here
            //
            // After iterating dataStreamLength times, it will start another header, begining from the loop indicator.
            // until the eof is reached
            //
            // Each pixel of the image is built like this:
            //
            // byte1 = 0x 0011 0100; // (queue.Dequeue();)
            // byte2 = 0x 0110 1100; // (queue.Dequeue();)
            //
            // Where:
            //
            // alpha = 0011 XXXX (1st 4 bits of byte1 + 4 don't care); 
            // red   = 0100 XXXX (2nd 4 bits of byte1 + 4 don't care);
            // green = 0110 XXXX (1st 4 bits of byte2 + 4 don't care);
            // blue  = 1100 XXXX (2st 4 bits of byte2 + 4 don't care);
            //
            // I have decided to replace the 4 don't care for proportional values, where
            // 240 (0xF0 - 0b1111000) is 255 (0xFF - 0b11111111). Using the following formula:
            //
            // trueColor = 255 * current / 240;
            //
            // Where v0 is the first information on the header, numberOfImages is v1, and v2 is the third
            // Every v# variable from now on are unknown values.

            int v0 = valueQueue.DequeueInt32();
            int numberOfImages = valueQueue.DequeueInt32();

            List<ImportedImage> importedImage = new List<ImportedImage>();

            for (int i = 0; i < numberOfImages; i++)
            {
                byte[] colorMode = new byte[] { (byte)valueQueue.DequeueInt16(), (byte)valueQueue.DequeueInt16() };
                int width = valueQueue.DequeueInt32();
                int height = valueQueue.DequeueInt32();
                int pivotX = valueQueue.DequeueInt32();
                int pivotY = valueQueue.DequeueInt32();
                int[] v5 = new int[] { valueQueue.DequeueInt16(), valueQueue.DequeueInt16() };
                int[] v6 = new int[] { valueQueue.DequeueInt16(), valueQueue.DequeueInt16() };
                int pilotPivotX = (sbyte)valueQueue.DequeueInt32();
                int pilotPivotY = (sbyte)valueQueue.DequeueInt32();
                int dataStreamLength = valueQueue.DequeueInt32();

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);

                for (int bitCount = 0, pixelCount = 0; bitCount < dataStreamLength; bitCount += 2, pixelCount++)
                {
                    bitmap.SetPixel(
                        pixelCount % width,
                        pixelCount / width,
                        GetPixelColor(
                            colorMode[0], (short)valueQueue.DequeueInt16()));
                }

                importedImage.Add(new ImportedImage(bitmap, (pivotX, pivotY), (pilotPivotX, pilotPivotY)));
            }

            return importedImage;
        }

        private static Color GetPixelColor(byte colorType, short encodedValue)
        {
            if (encodedValue > 1)
            {
                int kkk = 10;
                kkk++;
            }

            int a = 0, r = 0, g = 0, b = 0;
            switch (colorType)
            {
                case 0:
                    //In case no transparency is being used, 5-6-5 rgb pixel format it is
                    a = 255;
                    r = (encodedValue & 0b1111_1000_0000_0000) >> 8;
                    g = (encodedValue & 0b0000_0111_1110_0000) >> 3;
                    b = (encodedValue & 0b0000_0000_0001_1111) << 3;
                    break;

                case 1:
                    r = (encodedValue & 0b1111_1000_0000_0000) >> 8;
                    g = (encodedValue & 0b0000_0111_1110_0000) >> 3;
                    b = (encodedValue & 0b0000_0000_0001_1111) << 3;
                    if (r == 0 && g == 0 && b == 0)
                        a = 0;
                    else
                        a = 255;
                    break;

                case 2:
                    //In case there is transparency, each color is made of 4 bits
                    a = (encodedValue & 0xF000) >> 8;
                    r = (encodedValue & 0x0F00) >> 4;
                    g = (encodedValue & 0x00F0) >> 0;
                    b = (encodedValue & 0x000F) << 4;

                    a = (int)Math.Ceiling(255f / 240f * a);
                    r = (int)Math.Ceiling(255f / 240f * r);
                    g = (int)Math.Ceiling(255f / 240f * g);
                    b = (int)Math.Ceiling(255f / 240f * b);
                    break;
            }

            return Color.FromArgb(a, r, g, b);
        }
    }
}
