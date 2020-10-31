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

using Openbound_Asset_Tools.Entity;
using Openbound_Asset_Tools.Extension;
using Openbound_Asset_Tools.Helper;
using Openbound_Asset_Tools.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Openbound_Asset_Tools
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Preventing exceptions when outputing assets
            ExplorerHelper.CreateAllFolders();

            do
            {
                Console.WriteLine("Gunbound Raw Image Fix Tools");

                Console.WriteLine("SpriteSheet tools");
                Console.WriteLine("A1 - Create Spritesheet [SFX][Custom Image Size][1-Layer]");
                Console.WriteLine("A2 - Create Spritesheet [SFX][Custom Image Size][2-Layer - AlphaBlend]");
                Console.WriteLine("A3 - Create Spritesheet [Equal Images]");

                Console.WriteLine("\nImage Utils");
                Console.WriteLine("B1 - Image Border Fixer");

                Console.WriteLine("\nImage Processing");
                Console.WriteLine("C1 - Massive .IMG import.");
                Console.WriteLine("C2 - IMG Crack [Mobiles]");
                Console.WriteLine("C3 - Image Sync Comparer [DEPRECATED]");
                Console.WriteLine("C4 - Image Comparer");

                Console.WriteLine("\nFile manipulation");
                Console.WriteLine("D1 - Name Fixer.");
                Console.WriteLine("D2 - Pivot Offset Fixer");

                Console.WriteLine("\nDecrypt/Cypher");
                Console.WriteLine("E1 - XTF Crack");

                Console.WriteLine("\nCreate Assets");
                Console.WriteLine("F1 - Crosshair Drawer");
                Console.WriteLine("F2 - Mobile Buttons");
                Console.WriteLine("F3 - Create Minimap Tumbnails");
                Console.WriteLine("F4 - Spritefont Range Builder");
                Console.WriteLine("F5 - Item buttons thumbnails");

                try
                {
                    DateTime sDate = DateTime.Now;

                    switch (Console.ReadLine().ToUpper())
                    {
                        case "A1":
                            new SingleLayerSpritesheetMaker().CreateSpritesheet();
                            break;
                        case "A2":
                            new MultiLayerSpritesheetMaker().CreateSpritesheet();
                            break;
                        case "A3":
                            new SimpleSpritesheetMaker().CreateSpritesheet();
                            break;

                        case "B1":
                            ImageBorderFix.FixBorder();
                            break;

                        case "C1":
                            new SpriteImportManager().ImportSprites();
                            break;
                        case "C2":
                            IMGCracker.ExportIMGData();
                            break;
                        case "C3":
                            ImageSyncComparer.ImageSyncCompare();
                            break;
                        case "C4":
                            ImageComparer.CompareImages();
                            break;

                        case "D1":
                            new FileNameFixer().ImportSprites();
                            break;
                        case "D2":
                            PivotFileManager.FixPivotFile();
                            break;

                        case "E1":
                            XTFCracker.Crack();
                            break;

                        case "F1":
                            CrosshairDrawer.DrawCrosshairs();
                            break;
                        case "F2":
                            new AssetMaker().CreateButton();
                            break;
                        case "F3":
                            MinimapThumbGenerator.GenerateButtonThumbnails();
                            break;
                        case "F4":
                            SpritefontRangeBuilder.BuildSpritefontRange();
                            break;
                        case "F5":
                            new AssetMaker().CreateItemButton();
                            break;
                        default:
                            throw new Exception();
                    }

                    Console.WriteLine("Process complete. Running time: " + (DateTime.Now - sDate).TotalSeconds);
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadKey();
                    Console.Clear();
                }
            } while (true);
        }
    }
}
