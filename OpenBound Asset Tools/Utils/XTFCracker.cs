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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Openbound_Asset_Tools.Utils
{
    class XTFCracker
    {
        public static void Crack()
        {
            Console.WriteLine("Importing xtf Files");

            foreach (string file in FileImportManager.ReadMultipleXTFFiles())
            {
                Console.WriteLine(file);

                byte[] lines = File.ReadAllBytes(file);
                byte[] header = lines.ToList().GetRange(0, 9).ToArray();
                lines = lines.ToList().GetRange(9, lines.Length - 9).ToArray();
                int[] lineInt = new int[lines.Length / 2];
                int[] headerInt = new int[header.Length / 2];

                for (int i = 0; i < header.Length - 1; i += 2)
                {
                    headerInt[i / 2] += header[i] + (header[i + 1] << 8);
                }

                for (int i = 0; i < lines.Length; i += 2)
                {
                    lineInt[i / 2] += lines[i] + (lines[i + 1] << 8);
                }

                int imageWidth = 256;

                Bitmap image = new Bitmap(imageWidth, lineInt.Length / imageWidth);
                
                for (int width = 0; width < lineInt.Length; width++)
                {
                    int value = lineInt[width];

                    int a = ((value >> 12) & 0xFF) << 4;
                    int r = ((value >> 08) & 0xFF) << 4;
                    int g = ((value >> 04) & 0xFF) << 4;
                    int b = ((value >> 00) & 0xFF) << 4;

                    a = (int)Math.Ceiling(a * 255f / 240f);
                    r = (int)Math.Ceiling(r * 255f / 240f);
                    g = (int)Math.Ceiling(g * 255f / 240f);
                    b = (int)Math.Ceiling(b * 255f / 240f);

                    image.SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
                }

                image.Save($@"{Parameters.TextureOutputDirectory}{file.Split('\\').Last().Split('.')[0]}.png");
            }
        }

        /*
        public static void Crack2()
        {
            Console.WriteLine("Importing IMG Files");
            Thread t = new Thread(() => ImportIMGThread());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (t.IsAlive) Thread.Sleep(100);

            int index = 0;
            foreach (string file in imageFilePaths)
            {
                Console.WriteLine(file);

                byte[] lines = File.ReadAllBytes(file);
                byte[] header = lines.ToList().GetRange(0, 9).ToArray();
                lines = lines.ToList().GetRange(9, lines.Length - 9).ToArray();
                int[] lineInt = new int[lines.Length / 2];
                int[] headerInt = new int[header.Length / 2];

                for (int i = 0; i < header.Length - 1; i += 2)
                {
                        headerInt[i / 2] += header[i] + header[i + 1];
                }
                
                for(int i = 0; i < lines.Length; i+=2)
                {
                    lineInt[i / 2] += lines[i] + lines[i + 1];
                }
                
                int imageWidth = 256;
                List<Bitmap> imgList1 = new List<Bitmap>();
                List<Bitmap> imgList2 = new List<Bitmap>();
                List<Bitmap> imgList3 = new List<Bitmap>();
                List<Bitmap> imgList4 = new List<Bitmap>();

                List<Bitmap> imgList5 = new List<Bitmap>();
                List<Bitmap> imgList6 = new List<Bitmap>();
                List<Bitmap> imgList7 = new List<Bitmap>();
                List<Bitmap> imgList8 = new List<Bitmap>();

                List<Bitmap> imgList9 = new List<Bitmap>();
                List<Bitmap> imgList10 = new List<Bitmap>();
                List<Bitmap> imgList11 = new List<Bitmap>();
                List<Bitmap> imgList12 = new List<Bitmap>();

                List<Bitmap> imgList13 = new List<Bitmap>();
                List<Bitmap> imgList14 = new List<Bitmap>();
                List<Bitmap> imgList15 = new List<Bitmap>();
                List<Bitmap> imgList16 = new List<Bitmap>();

                List<Bitmap> imgList17 = new List<Bitmap>();
                List<Bitmap> imgList18 = new List<Bitmap>();
                List<Bitmap> imgList19 = new List<Bitmap>();
                List<Bitmap> imgList20 = new List<Bitmap>();

                List<Bitmap> imgList21 = new List<Bitmap>();
                List<Bitmap> imgList22 = new List<Bitmap>();
                List<Bitmap> imgList23 = new List<Bitmap>();
                List<Bitmap> imgList24 = new List<Bitmap>();
                //Bitmap image = new Bitmap(imageWidth, lineInt.Length / imageWidth);

                for (int i = 0; i < 32; i++)
                {
                    imgList1.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList2.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList3.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList4.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList5.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList6.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList7.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList8.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList9.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList10.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList11.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList12.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList13.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList14.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList15.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList16.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList17.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList18.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList19.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList20.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList21.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList22.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList23.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                    imgList24.Add(new Bitmap(imageWidth, lineInt.Length / imageWidth));
                }

                for (int width = 0; width < lineInt.Length; width++)
                {
                    int a = lineInt[width];

                    //image.SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(a));
                    List<uint> uIntList = RotateLeft((uint)a);

                    for (int i = 0; i < uIntList.Count; i++)
                    {
                        a = (int)uIntList[i];

                        int alpha = (a >> 24) & 0xFF;
                        int red = (a >> 16) & 0xFF;
                        int green = (a >> 8) & 0xFF;
                        int blue = (a >> 0) & 0xFF;

                        //imgList1[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(a));
                        imgList1[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, red, green, blue));
                        imgList2[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, red, blue, green));

                        imgList3[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, green, red, blue));
                        imgList4[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, green, blue, red));

                        imgList5[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, blue, red, green));
                        imgList6[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(alpha, blue, green, red));

                        imgList7[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, alpha, green, blue));
                        imgList8[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, alpha, blue, green));

                        imgList9[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, green, alpha, blue));
                        imgList10[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, green, green, alpha));

                        imgList11[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, blue, alpha, green));
                        imgList12[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(red, blue, green, alpha));

                        imgList15[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, alpha, red, blue));
                        imgList16[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, alpha, blue, red));

                        imgList13[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, red, alpha, blue));
                        imgList14[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, red, blue, alpha));

                        imgList17[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, blue, alpha, red));
                        imgList18[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(green, blue, red, alpha));

                        imgList19[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, alpha, red, green));
                        imgList20[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, alpha, green, red));

                        imgList21[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, red, green, alpha));
                        imgList22[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, red, alpha, blue));

                        imgList23[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, green, alpha, red));
                        imgList24[i].SetPixel(width % imageWidth, width / imageWidth, Color.FromArgb(blue, green, red, alpha));
                    }
                }

                int kk = 0;
                //image.Save($@"C:\Users\Carlos\Desktop\Sample\Imported\{index}-img.png");
                
                imgList1.AddRange(imgList2);
                imgList1.AddRange(imgList3);
                imgList1.AddRange(imgList4);
                imgList1.AddRange(imgList5);
                imgList1.AddRange(imgList6);
                imgList1.AddRange(imgList7);
                imgList1.AddRange(imgList8);
                imgList1.AddRange(imgList9);
                imgList1.AddRange(imgList10);
                imgList1.AddRange(imgList11);
                imgList1.AddRange(imgList12);
                imgList1.AddRange(imgList13);
                imgList1.AddRange(imgList14);
                imgList1.AddRange(imgList15);
                imgList1.AddRange(imgList16);
                imgList1.AddRange(imgList17);
                imgList1.AddRange(imgList18);
                imgList1.AddRange(imgList19);
                imgList1.AddRange(imgList20);
                imgList1.AddRange(imgList21);
                imgList1.AddRange(imgList22);
                imgList1.AddRange(imgList23);
                imgList1.AddRange(imgList24);
                imgList1.ForEach((x) => x.Save($@"C:\Users\Carlos\Desktop\Sample\Imported\{index}-{kk++}-img.png"));
            }
        }

        public static List<uint> RotateLeft(uint value)
        {
            List<uint> uIntList = new List<uint>();

            for(int i = 0; i < 32; i++)
            {
                uint val = (value << i) | (value >> (32 - i));
                uIntList.Add(val);
            }

            return uIntList;
        }

        public static List<byte> getAllPossibilities(int param)
        {
            List<byte> lByteList = new List<byte>();

            byte b1 = ((byte)((param >> 00) & 0b_0000_1111));
            byte b2 = ((byte)((param >> 04) & 0b_0000_1111));
            byte b3 = ((byte)((param >> 08) & 0b_0000_1111));
            byte b4 = ((byte)((param >> 12) & 0b_0000_1111));

            byte b5 = ((byte)((param >> 00) & 0b_1111_0000));
            byte b6 = ((byte)((param >> 04) & 0b_1111_0000));
            byte b7 = ((byte)((param >> 08) & 0b_1111_0000));
            byte b8 = ((byte)((param >> 12) & 0b_1111_0000));

            lByteList.Add(b1);
            lByteList.Add(b2);
            lByteList.Add(b3);
            lByteList.Add(b4);
            lByteList.Add(b5);
            lByteList.Add(b6);
            lByteList.Add(b7);
            lByteList.Add(b8);

            return lByteList;
        }

        public static List<int> getAllFourtuples(List<byte> p1, List<byte> p2)
        {
            List<int> lInt = new List<int>();

            foreach (byte b1 in p1)
            {
                foreach(byte b2 in p2)
                {
                    lInt.Add((b1 << 8) + b2);
                    lInt.Add(b1 + (b2 << 8));
                }
            }

            return lInt;
        }
        */
    }
}

