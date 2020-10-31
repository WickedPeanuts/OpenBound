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
using System.Drawing;

namespace WP_Image_Processing.Extension
{
    public static class ExtensionMethods
    {
        public static void ForEach(this Bitmap Source, Action<Color> Action)
        {
            for (int h = 0; h < Source.Height; h++)
            {
                for (int w = 0; w < Source.Width; w++)
                {
                    Action(Source.GetPixel(w, h));
                }
            }
        }

        public static void ForEach(this Bitmap[] Source, Action<Bitmap> Action)
        {
            for (int i = 0; i < Source.Length; i++)
            {
                Action(Source[i]);
            }
        }
    }
}
