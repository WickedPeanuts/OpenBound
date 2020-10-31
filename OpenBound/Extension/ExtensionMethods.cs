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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace OpenBound.Extension
{
    public static class ExtensionMethods
    {
        public static T[][] ExtractMatrixSlice<T>(this T[][] Source, int StartingX, int EndingX, int StartingY, int EndingY)
        {
            T[][] newMatrix = new T[EndingY - StartingY][];
            for (int h = StartingY, hi = 0; h < EndingY; h++, hi++)
            {
                newMatrix[hi] = new T[EndingX - StartingX];
                for (int w = StartingX, wi = 0; w < EndingX; w++, wi++)
                {
                    newMatrix[hi][wi] = Source[h][w];
                }
            }

            return newMatrix;
        }

        public static void ApplyMatrixSlice<T>(this T[][] Source, T[][] Matrix, int StartingX, int StartingY)
        {
            for (int h = StartingY, hi = 0; hi < Matrix.Length; h++, hi++)
            {
                for (int w = StartingX, wi = 0; wi < Matrix[0].Length; w++, wi++)
                {
                    Source[h][w] = Matrix[hi][wi];
                }
            }
        }

        public static T[] ConvertTo1D<T>(this T[][] Source)
        {
            T[] newArr = new T[Source.Length * Source[0].Length];

            for (int h = 0, index = 0; h < Source.Length; h++)
            {
                for (int w = 0; w < Source[0].Length; w++)
                {
                    newArr[index] = Source[h][w];
                    index++;
                }
            }

            return newArr;
        }

        public static T[][] ConvertTo2D<T>(this T[] Source, int Width, int Height)
        {
            T[][] newArr = new T[Height][];

            for (int h = 0, index1D = 0; h < Height; h++)
            {
                newArr[h] = new T[Width];
                for (int w = 0; w < Width; w++)
                {
                    newArr[h][w] = Source[index1D++];
                }
            }

            return newArr;
        }

        /// <summary>
        /// Maps a function over the matrix if the calculated radial distance is lesser or equal than DistanceThreshold.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="DistanceThreshold"></param>
        /// <param name="Function"></param>
        public static void RadiusBasedMap<T>(this T[][] Source, int X, int Y, int DistanceThreshold, Func<T, bool> Condition, Func<T, T> Function)
        {
            for (int h = 0; h < Source.Length; h++)
            {
                for (int w = 0; w < Source[0].Length; w++)
                {
                    int calculatedDistance = (int)Math.Sqrt(Math.Pow((w - X), 2) + Math.Pow((h - Y), 2));
                    if (calculatedDistance <= DistanceThreshold)
                        if (Condition(Source[h][w]))
                            Source[h][w] = Function(Source[h][w]);
                }
            }
        }

        public static void Map<T>(this T[][] source, Point position, Point finalPosition, Func<T, T> function)
        {
            for (int h = position.Y; h < finalPosition.Y; h++)
            {
                for (int w = position.X; w < finalPosition.X; w++)
                {
                    source[h][w] = function(source[h][w]);
                }
            }
        }

        public static void ReverseForEach<T>(this IList<T> Source, Action<T> Action)
        {
            for (int i = Source.Count - 1; i >= 0; i--)
            {
                Action(Source[i]);
            }
        }

        public static T[] ToArray<T>(this Vector2 source)
        {
            return new T[] { (T)Convert.ChangeType(source.X, typeof(T)), (T)Convert.ChangeType(source.Y, typeof(T)) };
        }

        //Vector operations
        public static Vector2 ToVector2(this float[] source)
        {
            return new Vector2(source[0], source[1]);
        }

        public static Vector2 ToVector2(this int[] source)
        {
            return new Vector2(source[0], source[1]);
        }

        //Texture operations
        public static Color[][] GetData(this Texture2D source)
        {
            Color[] color1D = new Color[source.Width * source.Height];
            source.GetData(color1D);
            return color1D.ConvertTo2D(source.Width, source.Height);
        }

        public static void ShiftLeft(this Texture2D source, int offset, Color standardColor)
        {
            Color[][] mat = source.GetData();

            for (int h = 0; h < mat.Length; h++)
            {
                for (int w = 0, nw = offset; w < mat[0].Length; w++, nw++)
                {
                    if (nw < mat[0].Length)
                        mat[h][w] = mat[h][nw];
                    else
                        mat[h][w] = standardColor;
                }
            }

            source.SetData(mat.ConvertTo1D());
        }

        public static void AppendTexture(this Texture2D origin, Point originInitialPosition, Point originFinalPosition, Texture2D destiny, Point offset)
        {
            Color[][] destinyData = destiny.GetData().ExtractMatrixSlice(originInitialPosition.X, originFinalPosition.X, originInitialPosition.Y, originFinalPosition.Y);
            Color[][] originData = origin.GetData();

            originData.ApplyMatrixSlice(destinyData, offset.X, offset.Y);

            origin.SetData(originData.ConvertTo1D());
        }

        public static Color MultiplyByFactor(this Color source, float[] factor)
        {
            float[] cA = new float[] { source.R / 255f, source.G / 255f, source.B / 255f, source.A / 255f };

            return new Color(
                MathHelper.Clamp(cA[0] * factor[0], 0, 255),
                MathHelper.Clamp(cA[1] * factor[1], 0, 255),
                MathHelper.Clamp(cA[2] * factor[2], 0, 255),
                MathHelper.Clamp(cA[3] * factor[3], 0, 255));
        }

        public static Color TintPixel(this Color source, Color target)
        {
            return new Color((int)(source.R + (1 - source.R / 255.0) * target.R),
                (int)(source.G + (1 - source.G / 255.0) * target.G),
                (int)(source.B + (1 - source.B / 255.0) * target.B));
        }

        public static void ChangeColor(this Texture2D source, Point position, Point finalPosition, Color color)
        {
            Color[][] mat = source.GetData();

            mat.Map(position, finalPosition, (c) => { return color; });

            source.SetData(mat.ConvertTo1D());
        }

        public static bool Intersects(this Rectangle source, Vector2 position)
        {
            return source.Intersects(new Rectangle((int)position.X, (int)position.Y, 1, 1));
        }

        public static void AddRange<T>(this HashSet<T> source, List<T> list)
        {
            list.ForEach((x) => source.Add(x));
        }

        public static void AddRange<T>(this HashSet<T> source, HashSet<T> set)
        {
            foreach(T v in set)
                source.Add(v);
        }

        public static void ForEach<T>(this HashSet<T> source, Action<T> action)
        {
            foreach (T v in source)
                action(v);
        }
        
        //Queue
        public static void ForEach<T>(this Queue<T> source, Action<T> action)
        {
            foreach (T t in source)
                action(t);
        }

        public static Vector2 ToIntegerDomain(this Vector2 source)
        {
            return new Vector2((int)source.X, (int)source.Y);
        }
    }
}
