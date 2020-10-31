using Openbound_Asset_Tools.Entity;
using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Openbound_Asset_Tools.Common
{
    public class FileImportManager
    {

        /// <summary>
        /// Read a single image file
        /// </summary>
        /// <returns></returns>
        public static ImportedImage ReadSingleImage()
        {
            ImportedImage image = null;
            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "Image Files (png)|*.png;";
                dialog.ShowDialog();

                foreach (string str in dialog.FileNames)
                {
                    image = new ImportedImage(str);
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return image;
        }

        /// <summary>
        /// Returns the path of one .IMG file.
        /// </summary>
        /// <returns></returns>
        public static string ReadSingleIMGFile()
        {
            string importedImage = null;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "IMG Files (img)|*.img;";
                dialog.ShowDialog();

                importedImage = dialog.FileNames[0];
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImage;
        }

        /// <summary>
        /// Returns the path of all selected .IMG files.
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadMultipleIMGFiles()
        {
            List<string> importedImageList = null;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "IMG Files (img)|*.img;";
                dialog.ShowDialog();

                importedImageList = dialog.FileNames.ToList();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImageList;
        }

        /// <summary>
        /// Returns the path of all selected files.
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadMultipleFilePaths()
        {
            List<string> importedImageList = null;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Finding Files...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.ShowDialog();

                importedImageList = dialog.FileNames.ToList();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImageList;
        }

        /// <summary>
        /// Return the path of all selected .xtf files
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadMultipleXTFFiles()
        {
            List<string> importedImageList = null;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Finding Files...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "XTF Files (xtf)|*.xtf;";
                dialog.ShowDialog();

                importedImageList = dialog.FileNames.ToList();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImageList;
        }

        /// <summary>
        /// Reads multiple image files (.png). It does not contains its pivot information.
        /// </summary>
        /// <returns></returns>
        public static List<ImportedImage> ReadMultipleImages()
        {
            List<ImportedImage> importedImageList = new List<ImportedImage>();

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "Image Files (png)|*.png;";
                dialog.ShowDialog();

                foreach (string str in dialog.FileNames)
                {
                    importedImageList.Add(new ImportedImage(str));
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImageList;
        }

        /// <summary>
        /// Reads a pivot file and return its lines as a List of integer arrays
        /// </summary>
        /// <returns></returns>
        public static List<int[]> ReadPivotFile()
        {
            string fileName = "";
            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "Text Files (txt)|*.txt;";
                dialog.ShowDialog();
                fileName = dialog.FileName;
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            List<int[]> fileContent = new List<int[]>();

            foreach(string x in File.ReadAllLines(fileName))
            {
                if (x.Count() == 0 || x[0] == '#') continue;

                string[] line = x.Split(',');

                if (line.Count() >= 3)
                    fileContent.Add(new int[] { int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]) });
            }

            return fileContent;
        }

        /// <summary>
        /// Reads multiple image files (.png) and its pivot file (.txt). All files must be selected at once.
        /// </summary>
        /// <returns></returns>
        public static List<ImportedImage> ReadMultipleImagesWithPivot()
        {
            List<ImportedImage> importedImageList = new List<ImportedImage>();

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Importing Images...");
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.InitialDirectory = Parameters.OutputDirectory;
                dialog.Filter = "Image Files with pivot text file|*.png;*.txt";
                dialog.ShowDialog();

                string[] pivotFileContent = null;

                foreach (string str in dialog.FileNames)
                {
                    if (str.ToLower().Contains(".txt"))
                        pivotFileContent = File.ReadAllLines(str);
                    else
                        importedImageList.Add(new ImportedImage(str));

                    Console.WriteLine("Loading File: " + str);
                }

                if (pivotFileContent == null)
                    throw new Exception("There was no .txt file on the imported files");

                Console.WriteLine("Loading Pivot File...");

                foreach (string str in pivotFileContent)
                {
                    if (str.Count() == 0 || str[0] == '#') continue;

                    string[] line = str.Split(',');

                    if (line.Length >= 3)
                        importedImageList[int.Parse(line[0])].Pivot = (int.Parse(line[1]), int.Parse(line[2]));
                }

                Console.WriteLine("\nWiping unecessary assets...\n");
                WipeDummyImages(importedImageList);
                Console.WriteLine("Done!");
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(50);
            }

            return importedImageList;
        }

        /// <summary>
        /// Replaces the dummy image (black square) with a transparent dummy (transparent square) with the same width/height.
        /// </summary>
        /// <param name="imgList"></param>
        private static void WipeDummyImages(List<ImportedImage> imgList)
        {
            Bitmap dummy = new Bitmap(Parameters.ContentDirectory + @"IgnoredDummy.png");

            foreach (ImportedImage img in imgList)
            {
                if (!ImageProcessing.IsEqual(dummy, img.BitmapImage)) continue;

                img.BitmapImage = ImageProcessing.CreateImage(ImageProcessing.CreateBlankColorMatrix(img.BitmapImage.Width, img.BitmapImage.Height));
                Console.WriteLine($"Wiped image: {img.FilePath}");
            }
        }
    }
}
