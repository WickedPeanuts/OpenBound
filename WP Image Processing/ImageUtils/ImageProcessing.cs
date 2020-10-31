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

using WP_Image_Processing.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WP_Image_Processing.ImageUtils
{
    public class ImageProcessing
    {
        public static Color[][] CreateMatrix(Bitmap image)
        {
            Color[][] mat = CreateBlankColorMatrix(image.Width, image.Height);

            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    mat[h][w] = image.GetPixel(w, h);
                }
            }

            return mat;
        }

        public static T[][] DecreaseMatrixSize<T>(T[][] colorMatrix)
        {
            T[][] newColorMatrix = new T[colorMatrix.Length - 2][];
            for (int i = 0; i < newColorMatrix.Length; i++)
            {
                newColorMatrix[i] = new T[colorMatrix[0].Length - 2];
                for (int j = 0; j < newColorMatrix[0].Length; j++)
                {
                    newColorMatrix[i][j] = colorMatrix[i + 1][j + 1];
                }
            }

            return newColorMatrix;
        }

        public static T[][] IncreaseMatrixSize<T>(T[][] colorMatrix)
        {
            T[][] newColorMatrix = new T[colorMatrix.Length + 2][];
            for (int i = 0; i < newColorMatrix.Length; i++)
            {
                newColorMatrix[i] = new T[colorMatrix[0].Length + 2];
                for (int j = 0; j < newColorMatrix[0].Length; j++)
                {
                    if (i == 0 || i > colorMatrix.Length || j == 0 || j > colorMatrix[0].Length)
                    {
                        newColorMatrix[i][j] = default(T);
                    }
                    else
                    {
                        newColorMatrix[i][j] = colorMatrix[i - 1][j - 1];
                    }
                }
            }

            return newColorMatrix;
        }

        public static Bitmap CropImage(Color[][] bgMat, int x, int y, int offsetY)
        {
            Color[][] newMat = CreateBlankColorMatrix(x, y);

            for(int h = 0; h < y; h++)
            {
                for(int w = 0; w < x; w++)
                {
                    newMat[h][w] = bgMat[h + offsetY][w];
                }
            }

            return CreateImage(newMat);
        }

        public static Bitmap ResizeImage(int newWidth, int newHeight, Bitmap source)
        {
            var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            thumbnailGraph.DrawImage(source, imageRectangle);

            thumbnailGraph.Dispose();

            return thumbnailBitmap;
        }

        public static Color[][] KeepAspectRatioResize(Bitmap bmp, int newWidth, int newHeight)
        {
            float newScale = Math.Min((float)newWidth / bmp.Width, (float)newHeight / bmp.Height);

            Color[][] newImg = CreateBlankColorMatrix((int)Math.Ceiling(bmp.Width * newScale), (int)Math.Ceiling(bmp.Height * newScale));

            for(int h = 0; h < newImg.Length; h++)
            {
                for(int w = 0; w < newImg[0].Length; w++)
                {
                    newImg[h][w] = bmp.GetPixel((int)(w / newScale), (int)(h / newScale));
                }
            }

            return newImg;
        }

        public static bool IsEqual(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height) return false;

            bool isEqual = true;
            for (int w = 0; w < bmp1.Width; w++)
            {
                for (int h = 0; h < bmp1.Height; h++)
                {
                    Color c1 = bmp1.GetPixel(w, h);
                    Color c2 = bmp2.GetPixel(w, h);
                    isEqual = isEqual && c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
                }
            }
            return isEqual;
        }

        public static void CompareImages(Bitmap bitmap1, Bitmap bitmap2, string path)
        {
            int ocurrences = 0;
            for (int i = 0; i < bitmap1.Width; i++)
            {
                for (int j = 0; j < bitmap2.Height; j++)
                {
                    Color b1 = bitmap1.GetPixel(i, j);
                    Color b2 = bitmap2.GetPixel(i, j);

                    if (!(b1.R == b2.R && b1.B == b2.B &&
                          b1.G == b2.G && b1.A == b2.A))
                    {
                        ocurrences++;
                        bitmap2.SetPixel(i, j, Color.FromArgb(255, 0, 0));
                    }
                }
            }

            Console.WriteLine($"The comparison process has found {ocurrences} ocurrences.");

            if (ocurrences > 0)
            {
                Console.WriteLine($"File exported at: {path}");
                bitmap2.Save(path);
            }
        }

        public static void CompareImages(Bitmap bitmap1, Bitmap bitmap2, int imageIndex)
        {
            int ocurrences = 0;
            for(int i = 0; i < bitmap1.Width; i++)
            {
                for(int j = 0; j < bitmap2.Height; j++)
                {
                    Color b1 = bitmap1.GetPixel(i, j);
                    Color b2 = bitmap2.GetPixel(i, j);

                    if (!(b1.R == b2.R && b1.B == b2.B &&
                          b1.G == b2.G && b1.A == b2.A))
                    {
                        ocurrences++;
                        bitmap2.SetPixel(i, j, Color.FromArgb(255, 0, 0));
                    }
                }   
            }

            if (ocurrences > 0)
            {
                Console.WriteLine(ocurrences + " ocurrences");
                bitmap2.Save(@"C:\Users\Carlos\source\repos\OpenBound\OpenBound\bin\Windows\x86\Debug\SyncDebug\" + imageIndex + ".png");
            }
        }

        public static int[][] Erode(int[][] colorMatrix)
        {
            int[][] newColorMatrix = (int[][])colorMatrix.Clone();

            for (int h = 0; h < colorMatrix.Length; h++)
            {
                for (int w = 0; w < colorMatrix[0].Length; w++)
                {
                    newColorMatrix[h] = (int[])colorMatrix[h].Clone();
                }
            }

            for (int h = 1; h < colorMatrix.Length - 1; h++)
            {
                for (int w = 1; w < colorMatrix[0].Length - 1; w++)
                {
                    int sum = 0;

                    if (colorMatrix[h - 1][w - 1] > 0)
                        sum++;
                    if (colorMatrix[h - 1][w] > 0)
                        sum++;
                    if (colorMatrix[h - 1][w + 1] > 0)
                        sum++;

                    if (colorMatrix[h][w - 1] > 0)
                        sum++;
                    if (colorMatrix[h][w] > 0)
                        sum++;
                    if (colorMatrix[h][w + 1] > 0)
                        sum++;

                    if (colorMatrix[h + 1][w - 1] > 0)
                        sum++;
                    if (colorMatrix[h + 1][w] > 0)
                        sum++;
                    if (colorMatrix[h + 1][w + 1] > 0)
                        sum++;

                    if (sum >= 8)
                        newColorMatrix[h][w] = colorMatrix[h][w];
                    else
                        newColorMatrix[h][w] = 0;
                }
            }

            return newColorMatrix;
        }

        public static T[][] CopyMatrix<T>(T[][] input)
        {
            T[][] colorMatrix = CreateBlankMatrix<T>(input[0].Length, input.Length);

            for (int h = 0; h < input.Length; h++)
                for (int w = 0; w < input[0].Length; w++)
                    colorMatrix[h][w] = input[h][w];

            return colorMatrix;
        }

        public static Color[][] Blur(Color[][] color)
        {
            Color[][] colorMatrix = CopyMatrix(color);

            for (int h = 1; h < color.Length - 1; h++)
            {
                for (int w = 1; w < color[0].Length -1; w++)
                {
                    List<Color> lC = new List<Color>() {
                        color[h - 1][w - 1], color[h - 1][w + 0] , color[h - 1][w + 1],
                        color[h + 0][w - 1], color[h + 0][w + 0] , color[h + 0][w + 1],
                        color[h + 1][w - 1], color[h + 1][w + 0] , color[h + 1][w + 1],
                    };

                    colorMatrix[h][w] = Color.FromArgb(lC.Sum((x) => x.A) / 9, lC.Sum((x) => x.R) / 9, lC.Sum((x) => x.G) / 9, lC.Sum((x) => x.B) / 9);
                }
            }

            return colorMatrix;
        }

        public static Color[][] Dilatate(Color[][] colorMatrix, Color newColor)
        {

            Color[][] newMatrix = CreateBlankColorMatrix(colorMatrix[0].Length, colorMatrix.Length);

            for (int h = 1; h < newMatrix.Length - 1; h++)
            {
                for (int w = 1; w < newMatrix[0].Length - 1; w++)
                {
                    List<Color> tmpList = new List<Color>(){
                        /*colorMatrix[h - 1][w - 1],*/ colorMatrix[h - 1][w + 0], /*colorMatrix[h - 1][w + 1],*/
                          colorMatrix[h + 0][w - 1],   colorMatrix[h + 0][w + 0],   colorMatrix[h + 0][w + 1],
                        /*colorMatrix[h + 1][w - 1],*/ colorMatrix[h + 1][w + 0], /*colorMatrix[h + 1][w + 1],*/
                    };

                    int total = tmpList.Max((x) => x.R + x.G + x.B + x.A);

                    if (total > 0)
                        newMatrix[h][w] = newColor;
                }
            }

            //AddImageIntoMatrix(newMatrix, CreateImage(colorMatrix), 0, 0);

            return newMatrix;
        }

        public static Bitmap IncreaseImage(Bitmap image, float mFact, float sFact)
        {
            Color[][] a = CreateMatrix(image);

            for (int i = 0; i < a.Length; i++)
                for (int j = 0; j < a[0].Length; j++)
                    a[i][j] = Color.FromArgb((byte)(a[i][j].A), (int)Math.Min(255, (a[i][j].R * mFact + sFact)), (int)Math.Min(255, (a[i][j].G * mFact + sFact)), (int)Math.Min(255, (a[i][j].B * mFact + sFact)));

            return CreateImage(a);
        }

        public static int[][] Dilatate(int[][] colorMatrix)
        {
            int[][] newColorMatrix = new int[colorMatrix.Length][];

            for (int h = 0; h < colorMatrix.Length; h++)
            {
                newColorMatrix[h] = new int[colorMatrix[0].Length];
                for (int w = 0; w < colorMatrix[0].Length; w++)
                {
                    newColorMatrix[h][w] = colorMatrix[h][w];
                }
            }

            for (int h = 1; h < colorMatrix.Length - 1; h++)
            {
                newColorMatrix[h] = new int[colorMatrix[0].Length];
                for (int w = 1; w < colorMatrix[0].Length - 1; w++)
                {
                    double sum =
                          colorMatrix[h - 1][w - 1] + colorMatrix[h - 1][w] + colorMatrix[h - 1][w + 1]
                        + colorMatrix[h][w - 1] + colorMatrix[h][w] + colorMatrix[h][w + 1]
                        + colorMatrix[h + 1][w - 1] + colorMatrix[h + 1][w] + colorMatrix[h + 1][w + 1];

                    newColorMatrix[h][w] = (sum > 200) ? 255 : 0;
                }
            }

            return newColorMatrix;
        }

        public static void AddImageIntoMatrix(Color[][] newImageMatrix, Bitmap image, int x, int y)
        {
            for (int hImage = 0, h = y; hImage < image.Height; h++, hImage++)
            {
                for (int wImage = 0, w = x; wImage < image.Width; w++, wImage++)
                {
                    if (w >= 0 && h >= 0 && w < newImageMatrix[0].Length && h < newImageMatrix.Length)
                        newImageMatrix[h][w] = image.GetPixel(wImage, hImage);
                }
            }
        }

        public static void AddImageIntoMatrix2(Color[][] destiny, Bitmap source, int x, int y)
        {
            for (int hImage = 0, h = y; hImage < source.Height; h++, hImage++)
            {
                for (int wImage = 0, w = x; wImage < source.Width; w++, wImage++)
                {
                    if (w >= 0 && h >= 0 && w < destiny[0].Length && h < destiny.Length && source.GetPixel(wImage, hImage).A != 0)
                        destiny[h][w] = source.GetPixel(wImage, hImage);
                }
            }
        }

        public static void BlendImageIntoMatrix(Color[][] newImageMatrix, Bitmap image, int x, int y, Func<Color, Color, Color> apply)
        {
            for (int hImage = 0, h = y; hImage < image.Height; h++, hImage++)
            {
                for (int wImage = 0, w = x; wImage < image.Width; w++, wImage++)
                {
                    if (w >= 0 && h >= 0 && w < newImageMatrix[0].Length && h < newImageMatrix.Length)
                        newImageMatrix[h][w] = apply(newImageMatrix[h][w], image.GetPixel(wImage, hImage));
                }
            }
        }

        public static void AddImageIntoMatrix(Color[][] newImageMatrix, Bitmap image, int x, int y, int pivotX, int pivotY)
        {
            for (int hImage = pivotY, h = y; hImage < image.Height; h++, hImage++)
            {
                for (int wImage = pivotX, w = x; wImage < image.Width; w++, wImage++)
                {
                    if (w >= 0 && h >= 0 && w < newImageMatrix[0].Length && h < newImageMatrix.Length &&
                        hImage >= 0 && wImage >= 0)
                        newImageMatrix[h][w] = image.GetPixel(wImage, hImage);
                }
            }
        }
        /*
        public static void AddImageIntoMatrix(Color[][] newImageMatrix, Bitmap image, int initialShiftX, int initialShiftY, int endingX, int endingY)
        {
            for (int hImage = 0, h = initialShiftY; hImage < endingY; h++, hImage++)
            {
                for (int wImage = 0, w = initialShiftX; wImage < endingX; w++, wImage++)
                {
                    try
                    {
                        newImageMatrix[h][w] = image.GetPixel(wImage, hImage);
                    } catch
                    { }
                }
            }
        }*/

        public static void SaveImage(int[][] colorMatrixR, int[][] colorMatrixG, int[][] colorMatrixB, int[][] colorMatrixA, string filePath)
        {
            Bitmap newImage = new Bitmap(colorMatrixA[0].Length, colorMatrixA.Length);

            for (int h = 0; h < newImage.Height; h++)
            {
                for (int w = 0; w < newImage.Width; w++)
                {
                    Color c = Color.FromArgb((int)colorMatrixA[h][w], (int)colorMatrixR[h][w], (int)colorMatrixG[h][w], (int)colorMatrixB[h][w]);
                    //Color c = Color.FromArgb((int)colorMatrixA[h][w] / 2, (int)colorMatrixA[h][w] / 2, (int)colorMatrixA[h][w] / 2, (int)colorMatrixA[h][w] / 2);
                    newImage.SetPixel(w, h, c);
                }
            }

            newImage.Save(filePath);
        }

        public static Bitmap FixAlphaChannel(Bitmap image)
        {
            int[][] colorMatrixR = new int[image.Height][];
            int[][] colorMatrixG = new int[image.Height][];
            int[][] colorMatrixB = new int[image.Height][];
            int[][] colorMatrixA = new int[image.Height][];

            for (int h = 0; h < image.Height; h++)
            {
                colorMatrixR[h] = new int[image.Width];
                colorMatrixG[h] = new int[image.Width];
                colorMatrixB[h] = new int[image.Width];
                colorMatrixA[h] = new int[image.Width];

                for (int w = 0; w < image.Width; w++)
                {
                    Color c = image.GetPixel(w, h);
                    colorMatrixR[h][w] = c.R;
                    colorMatrixG[h][w] = c.G;
                    colorMatrixB[h][w] = c.B;
                    colorMatrixA[h][w] = (c.R > 0 || c.G > 0 || c.B > 0 || c.A > 0) ? c.A : 0;
                }
            }

            colorMatrixA = IncreaseMatrixSize(colorMatrixA);
            for (int i = 0; i < 1; i++)
                colorMatrixA = Dilatate(colorMatrixA);

            for (int i = 0; i < 1; i++)
                colorMatrixA = Erode(colorMatrixA);

            //Reaply Alpha Matrix
            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    Color c = image.GetPixel(w, h);
                    if (c.A > 0)
                        colorMatrixA[h][w] = c.A;
                }
            }

            colorMatrixA = DecreaseMatrixSize(colorMatrixA);

            return CreateImage(colorMatrixA, colorMatrixR, colorMatrixG, colorMatrixB);
        }

        public static Bitmap CreateImage(int[][] colorMatrixA, int[][] colorMatrixR, int[][] colorMatrixG, int[][] colorMatrixB)
        {
            Bitmap newBmp = new Bitmap(colorMatrixA[0].Length, colorMatrixA.Length);

            for(int h = 0; h < colorMatrixR.Length; h++)
            {
                for(int w = 0; w < colorMatrixR[0].Length; w++)
                {
                    newBmp.SetPixel(w, h, Color.FromArgb(colorMatrixA[h][w], colorMatrixR[h][w], colorMatrixG[h][w], colorMatrixB[h][w]));
                }
            }

            return newBmp;
        }

        public static Bitmap CreateImage(Color[][] colorMatrix)
        {
            Bitmap newBmp = new Bitmap(colorMatrix[0].Length, colorMatrix.Length);

            for (int h = 0; h < colorMatrix.Length; h++)
            {
                for (int w = 0; w < colorMatrix[0].Length; w++)
                {
                    newBmp.SetPixel(w, h, colorMatrix[h][w]);
                }
            }

            return newBmp;
        }

        public static Bitmap ShiftPixels(Bitmap image, int xShift, int yShift)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    try
                    {
                        bmp.SetPixel(w + xShift, h + yShift, image.GetPixel(w, h));
                    }
                    catch
                    {
                        bmp.SetPixel(w, h, Color.FromArgb(0, 0, 0, 0));
                    }
                }
            }
            return bmp;
        }

        public static (int, int, int, int) GetImageStartingCoordinates(Bitmap Image)
        {
            int minX = Image.Height;
            int minY = Image.Width;
            int maxX = 0;
            int maxY = 0;

            for (int h = 0; h < Image.Height; h++)
            {
                for (int w = 0; w < Image.Width; w++)
                {
                    Color c = Image.GetPixel(w, h);

                    if (c.R > 0 || c.G > 0 || c.B > 0)
                    {
                        minX = Math.Min(minX, w);
                        minY = Math.Min(minY, h);
                        maxX = Math.Max(maxX, w);
                        maxY = Math.Max(maxY, h);
                    }
                }
            }

            return (minX, minY, maxX, maxY);
        }

        public static T[][] CreateBlankMatrix<T>(int Width, int Height)
        {
            T[][] c = new T[Height][];
            for (int i = 0; i < Height; i++) c[i] = new T[Width];
            return c;
        }

        public static Color[][] CreateBlankColorMatrix(int Width, int Height)
        {
            Color[][] c = new Color[Height][];
            for (int i = 0; i < Height; i++) c[i] = new Color[Width];
            return c;
        }

        public static Color[][] Flip(Color[][] matrix, bool horizontal, bool vertical)
        {
            Color[][] newMatrix = CreateBlankColorMatrix(matrix[0].Length, matrix.Length);
            
            {
                for (int height = 0; height < matrix.Length; height++)
                {
                    for (int width = 0; width < matrix[0].Length; width++)
                    {
                        newMatrix[vertical ? matrix.Length - 1 - height : height][horizontal ? matrix[0].Length - 1 - width : width] = matrix[height][width];
                    }
                }
            }

            return newMatrix;
        }

        public static Color[][] JoinImageChannels(int[][][] image)
        {
            Color[][] color = CreateBlankColorMatrix(image[0][0].Length, image[0].Length);

            for(int h = 0; h < image[0].Length; h++)
            {
                for(int w = 0; w < image[0][0].Length; w++)
                {
                    color[h][w] = Color.FromArgb(image[0][h][w], image[1][h][w], image[2][h][w], image[3][h][w]);
                }
            }

            return color;
        }

        public static int[][][] SplitImageChannels(Bitmap Image)
        {
            int[][][] channels = new int[4][][];//[0 = A][1 = R][2 = G][3 = B]

            int w;

            //Channel Loop
            for(int c = 0; c < channels.Length; c++)
            {
                channels[c] = new int[Image.Width][];
                //Width Loop
                for (w = 0; w < channels[0].Length; w++) channels[c][w] = new int[Image.Height];
            }

            w = 0;
            int h = 0;

            Image.ForEach((Color c) =>
            {
                channels[0][h][w] = c.A;
                channels[1][h][w] = c.R;
                channels[2][h][w] = c.G;
                channels[3][h][w] = c.B;

                w++;
                if (w == Image.Width) { h++; w = 0; }
            });

            return channels;
        }
    }
}
