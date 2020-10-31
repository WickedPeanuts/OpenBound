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

using System;
using System.Linq;

namespace OpenBound_Network_Object_Library.Extension
{
    public static class EArray
    {
        public static void SafeResize<T>(this T[] ArrayObject, int NewSize)
        {
            if (NewSize > ArrayObject.Count())
                throw new Exception();
            Array.Resize(ref ArrayObject, NewSize);
        }

        public static T[][] ToMatrix<T>(this T[,] source)
        {
            int height = source.GetLength(0);
            int width = source.GetLength(1);

            T[][] output = new T[height][];

            for (int i = 0; i < height; i++)
            {
                output[i] = new T[width];
                for (int j = 0; j < width; j++)
                {
                    output[i][j] = source[i, j];
                }
            }

            return output;
        }
    }
}
