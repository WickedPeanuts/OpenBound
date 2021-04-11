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
using ImgTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Openbound_Asset_Tools.Utils
{
    class SpriteImportManager
    {
        /// <summary>
        /// Import multiple .IMG files at once. It may fail due to errors containing in the external library parser.
        /// This method is used to extract the .txt pivot files from .IMG its associated .png images.
        /// </summary>
        public void ImportSprites()
        {
            List<string> failureList = new List<string>();

            foreach (string imgFilePath in FileImportManager.ReadMultipleIMGFiles())
            {
                string fileName = "";

                try
                {
                    foreach (ArchivedFile aFile in Archive.LoadIMG(imgFilePath).Files)
                    {
                        string[] tmp = imgFilePath.Split('\\');
                        tmp = tmp.Last().Split('.');

                        fileName = tmp[0];

                        Frame[] frameArr = ImageDecoder.LoadFrames(aFile);

                        string pivotStrList = "";

                        for (int i2 = 0; i2 < frameArr.Length; i2++)
                        {
                            pivotStrList += $"{string.Format("{0:000}", i2)},{frameArr[i2].m_CenterX},{frameArr[i2].m_CenterY},{frameArr[i2].pR1},{frameArr[i2].pR2},{frameArr[i2].m_CenterX + frameArr[i2].pR1},{frameArr[i2].m_CenterY + frameArr[i2].pR2}\n";

                            string s = $@"{Parameters.RawOutputDirectory}\{fileName}-{string.Format("{0:000}", i2)}.png";
                            frameArr[i2].m_Image.Save(s);
                        }

                        File.WriteAllText($@"{Parameters.RawOutputDirectory}\{fileName}.txt", pivotStrList);

                        Console.WriteLine($"Imported: {fileName}");
                    }
                }
                catch
                {
                    failureList.Add(fileName);
                }
            }

            Console.Write("\n\nProcess terminated.");

            if (failureList.Count > 0)
            {
                Console.WriteLine(" A few errors were encountered: ");
                failureList.ForEach((x) => Console.WriteLine("File:" + x));
            }
        }
    }
}
