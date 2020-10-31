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
using WP_Image_Processing.Extension;
using WP_Image_Processing.ImageUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Openbound_Asset_Tools.Utils
{
    public class AssetMaker
    {
        public class ImportedImage
        {
            public string filePath;
            public Bitmap image;
        }

        List<ImportedImage> importedImageList = new List<ImportedImage>();

        void ReadAllImages()
        {
            Console.WriteLine("Importing Sprites...");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.InitialDirectory = Parameters.OutputDirectory;
            dialog.ShowDialog();

            if (dialog.FileNames.Length == 0)
            {
                Console.WriteLine("Aborted!");
                return;
            }

            string[] filePath;
            Bitmap[] imageList;

            filePath = new string[dialog.FileNames.Length];
            imageList = new Bitmap[dialog.FileNames.Length];

            for (int i = 0; i < dialog.FileNames.Length; i++)
            {
                filePath[i] = dialog.FileNames[i];
                imageList[i] = new Bitmap(filePath[i]);
                Console.WriteLine("I: " + filePath[i]);

                importedImageList.Add(new ImportedImage()
                {
                    filePath = filePath[i],
                    image = imageList[i],
                });
            }
        }

        public void CreateSpritesheet()
        {
            Thread t = new Thread(() => ReadAllImages());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            while (t.IsAlive)
            {
                Thread.Sleep(1000);
            }
        }

        public void CreateButton()
        {
            CreateSpritesheet();
            Bitmap bmp = new Bitmap(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Files\MobileButtonBG.png");

            List<Color[][]> output = new List<Color[][]>();

            foreach (ImportedImage ii in importedImageList)
            {
                bool[][] mask = { new bool[] { true, true, true }, new bool[] { true, true, true }, new bool[] { true, true, true } };
                Color[][] c = ImageProcessing.CreateMatrix(ii.image);

                c = ImageProcessing.IncreaseMatrixSize(c);
                c = ImageProcessing.IncreaseMatrixSize(c);
                c = ImageProcessing.IncreaseMatrixSize(c);
                //c = ImageProcessing.IncreaseMatrixSize(c);
                //c = ImageProcessing.IncreaseMatrixSize(c);
                //c = ImageProcessing.IncreaseMatrixSize(c);

                output.Add(ImageProcessing.Dilatate(c, Color.FromArgb(255, 255, 174, 98)));
                output.Add(ImageProcessing.Dilatate(output[0], Color.FromArgb(255, 238, 145, 98)));
                output.Add(ImageProcessing.Dilatate(output[1], Color.FromArgb(255, 205, 121, 106)));
                //output[3] = ImageProcessing.Dilatate(output[2], Color.FromArgb(255, 172, 153, 172));
                //output[4] = ImageProcessing.Dilatate(output[3], Color.FromArgb(255, 172, 161, 180));
                //output[5] = ImageProcessing.Dilatate(output[4], Color.FromArgb(255, 164, 178, 205));

                for (int i = 1; i < output.Count(); i++)
                {
                    Bitmap bemp = ImageProcessing.CreateImage(output[i - 1]);

                    ImageProcessing.BlendImageIntoMatrix(output[i], bemp, 0, 0, (y, x) =>
                    {
                        return Color.FromArgb(Math.Max(x.A, y.A),
                            (byte)CalculatePixel(x.R, x.A, y.R, y.A),
                            (byte)CalculatePixel(x.G, x.A, y.G, y.A),
                            (byte)CalculatePixel(x.B, x.A, y.B, y.A));
                    });
                }

                //Apply Blur
                output[0] = ImageProcessing.Blur(output.Last());

                ImageProcessing.BlendImageIntoMatrix(output[0], ii.image, output.Count(), output.Count(), (y, x) =>
                {
                    return Color.FromArgb(Math.Max(x.A, y.A),
                        (byte)CalculatePixel(x.R, x.A, y.R, y.A),
                        (byte)CalculatePixel(x.G, x.A, y.G, y.A),
                        (byte)CalculatePixel(x.B, x.A, y.B, y.A));
                });

                Bitmap newImg = ImageProcessing.CreateImage(output[0]);
                newImg.Save(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Buttons\output4.png");

                //Base img
                Color[][] bi = ImageProcessing.CreateMatrix(bmp);

                ImageProcessing.BlendImageIntoMatrix(bi, ii.image, output.Count(), output.Count(), (y, x) =>
                {
                    return Color.FromArgb(Math.Max(x.A, y.A),
                        (byte)CalculatePixel(x.R, x.A, y.R, y.A),
                        (byte)CalculatePixel(x.G, x.A, y.G, y.A),
                        (byte)CalculatePixel(x.B, x.A, y.B, y.A));
                });

                newImg = ImageProcessing.CreateImage(bi);
                newImg.Save(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Buttons\output1.png");

                //Base img hoover
                Bitmap bmpppp = ImageProcessing.IncreaseImage(ii.image, 1.2f, 0f);

                bi = ImageProcessing.CreateMatrix(bmp);

                ImageProcessing.BlendImageIntoMatrix(bi, bmpppp, output.Count(), output.Count(), (y, x) =>
                {
                    return Color.FromArgb(Math.Max(x.A, y.A),
                        (byte)CalculatePixel(x.R, x.A, y.R, y.A),
                        (byte)CalculatePixel(x.G, x.A, y.G, y.A),
                        (byte)CalculatePixel(x.B, x.A, y.B, y.A));
                });

                newImg = ImageProcessing.CreateImage(bi);
                newImg.Save(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Buttons\output2.png");

                ImageProcessing.BlendImageIntoMatrix(bi, bmpppp, output.Count(), output.Count(), (y, x) =>
                {
                    return Color.FromArgb(Math.Max(x.A, y.A),
                        (byte)CalculatePixel(x.R * 0.8f, x.A, y.R, y.A),
                        (byte)CalculatePixel(((x.R * 3 + x.G) / 4) * 0.8f, x.A, y.G, y.A),
                        (byte)CalculatePixel(((x.R * 3 + x.B) / 4) * 0.8f, x.A, y.B, y.A));
                });

                newImg = ImageProcessing.CreateImage(bi);
                newImg.Save(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Buttons\output5.png");

                //Base img hoover
                bi = ImageProcessing.CreateMatrix(bmp);

                ImageProcessing.BlendImageIntoMatrix(bi, bmpppp, output.Count() + 1, output.Count() + 1, (y, x) =>
                {
                    return Color.FromArgb(Math.Max(x.A, y.A),
                        (byte)CalculatePixel(x.R, x.A, y.R, y.A),
                        (byte)CalculatePixel(x.G, x.A, y.G, y.A),
                        (byte)CalculatePixel(x.B, x.A, y.B, y.A));
                });


                newImg = ImageProcessing.CreateImage(bi);
                newImg.Save(@"C:\Users\Carlos\source\repos\OpenBound\GunboundImageFix\Output\Buttons\output3.png");
            }
        }

        public void CreateItemButton()
        {
            List<Entity.ImportedImage> _imgList = FileImportManager.ReadMultipleImages();

            Bitmap bmpHoover;
            Bitmap bmpDisabled;
            int i = 0;
            foreach (var img in _imgList)
            {
                bmpHoover = new Bitmap(img.BitmapImage.Width, img.BitmapImage.Height);
                bmpDisabled = new Bitmap(img.BitmapImage.Width, img.BitmapImage.Height);

                for (int h = 0; h < img.BitmapImage.Height; h++)
                {
                    for(int w = 0; w < img.BitmapImage.Width; w++)
                    {
                        Color c = img.BitmapImage.GetPixel(w, h);
                        bmpHoover.SetPixel(w, h, Color.FromArgb(255, Math.Min(c.R + 20, 255), Math.Min(c.G + 20, 255), Math.Min(c.B + 20, 255)));
                        bmpDisabled.SetPixel(w, h, Color.FromArgb(255, (c.R + c.G + c.B) / 3, (c.R + c.G + c.B) / 3, (c.R + c.G + c.B) / 3));
                    }
                }

                bmpHoover.Save($"{Parameters.ButtonOutputDirectory}/{i++} - Hoover.png");
                bmpDisabled.Save($"{Parameters.ButtonOutputDirectory}/{i} - Disabled.png");
            }
        }

        public float CalculatePixel(float ca, float aa, float cb, float ab)
        {
            aa /= 255;
            ab /= 255;
            return (ca * aa + cb * ab * (1 - aa)) / (aa + ab * (1 - aa));
        }
    }
}
