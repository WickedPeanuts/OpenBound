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
using OpenBound.Extension;
using System;

namespace OpenBound.Common
{
    public class Helper
    {
        public static T[] CreateVector<T>(int length, T defaultValue)
        {
            T[] newVector = new T[length];

            for (int i = 0; i < length; i++)
                newVector[i] = defaultValue;

            return newVector;
        }

        public static T[][] CreateMatrix<T>(int width, int height, T defaultValue)
        {
            T[][] newMatrix = new T[height][];

            for (int h = 0; h < newMatrix.Length; h++)
            {
                newMatrix[h] = new T[width];

                for (int w = 0; w < newMatrix[0].Length; w++)
                {
                    newMatrix[h][w] = defaultValue;
                }
            }

            return newMatrix;
        }

        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            return Math.Atan2(a.Y - b.Y, a.X - b.X);
        }

        public static double EuclideanDistance(Vector2 a, Vector2 b)
        {
            Vector2 c = a - b;
            return Math.Sqrt(c.X * c.X + c.Y * c.Y);
        }

        public static double SquaredEuclideanDistance(Vector2 a, Vector2 b)
        {
            Vector2 c = a - b;
            return c.X * c.X + c.Y * c.Y;
        }

        public static float EuclideanDistance(float xA, float yA, float xB, float yB)
        {
            float x = xA - xB;
            float y = yA - yB;
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static float SquaredEuclideanDistance(float xA, float yA, float xB, float yB)
        {
            float x = xA - xB;
            float y = yA - yB;
            return x * x + y * y;
        }
    }
}
