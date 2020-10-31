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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Openbound_Asset_Tools.Utils
{
    class FileNameFixer
    {
        public void ImportSprites()
        {
            List<string> filePaths = FileImportManager.ReadMultipleFilePaths();

            try
            {
                Console.WriteLine("Enter the starting number: ");
                int sNum = int.Parse(Console.ReadLine());

                string[] name = filePaths[0].Split('\\');

                string text = string.Join("/", name.ToList().GetRange(0, name.Length - 1));

                foreach (string imgFilePath in filePaths)
                {
                    try
                    {
                        File.Move(imgFilePath, text + "/" + sNum++ + ".png");
                        Console.WriteLine($"File: {imgFilePath} was moved");
                    }
                    catch
                    {
                        Console.WriteLine($"File: {imgFilePath} could not be moved");
                    }
                }

                Console.Write("\n\nProcess terminated.");
            }
            catch { }
        }
    }
}
