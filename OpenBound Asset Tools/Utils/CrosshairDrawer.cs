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
using WP_Image_Processing.ImageUtils;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Openbound_Asset_Tools.Utils
{
    public class CrosshairDrawer
    {
        public static void DrawCrosshairs()
        {
            Bitmap image = new Bitmap($@"{Parameters.BaseDirectory}\Files\crosshair.png");
            Color[][] cMatrix = ImageProcessing.CreateBlankColorMatrix(83, 83);
            ImageProcessing.AddImageIntoMatrix(cMatrix, image, 0, 0);
            cMatrix = ImageProcessing.Flip(cMatrix, true, false);
            ImageProcessing.CreateImage(cMatrix).Save($@"{Parameters.BaseDirectory}\\Output\\Crosshair\\CrosshairFrame.png");

            foreach (KeyValuePair<MobileType, Dictionary<ShotType, AimPreset>> kvp in MobileMetadata.AimPresets)
            {
                foreach (KeyValuePair<ShotType, AimPreset> kvp2 in kvp.Value)
                {
                    cMatrix = DrawCrosshair(
                        kvp2.Value.AimTrueRotationMin,
                        kvp2.Value.AimTrueRotationMax,
                        kvp2.Value.AimFalseRotationMin,
                        kvp2.Value.AimFalseRotationMax,
                        true);
                    ImageProcessing.CreateImage(cMatrix).Save($@"{Parameters.BaseDirectory}\\Output\\Crosshair\\{kvp.Key}{kvp2.Key}Ally.png");

                    cMatrix = DrawCrosshair(
                        kvp2.Value.AimTrueRotationMin, kvp2.Value.AimTrueRotationMax,
                        kvp2.Value.AimFalseRotationMin, kvp2.Value.AimFalseRotationMax,
                        false);

                    ImageProcessing.CreateImage(cMatrix).Save($@"{Parameters.BaseDirectory}\\Output\\Crosshair\\{kvp.Key}{kvp2.Key}Enemy.png");
                }
            }

            Process.Start($@"{Parameters.BaseDirectory}\\Output\\Crosshair");
        }

        private static Color[][] DrawCrosshair(int startingAngle, int endingAngle, int startingFadedAngle, int endingFadedAngle, bool IsAlly = true)
        {
            //
            Color[][] cMatrix = ImageProcessing.CreateBlankColorMatrix(83, 83);
            //ImageProcessing.AddImageIntoMatrix(cMatrix, image, 0, 0);

            //cMatrix = ImageProcessing.Flip(cMatrix, true, false);

            //Draw the Green/RED area

            int dY = cMatrix.Length / 2;
            int dX = cMatrix[0].Length / 2;

            int red, green, blue, alpha, subAlpha;
            int subRed, subGreen, subBlue;

            if (IsAlly)
            {
                red = 0; subRed = 50;
                green = 255; subGreen = 255;
                blue = 0; subBlue = 50;
                alpha = 100; subAlpha = 50;
            }
            else
            {
                red = 255; subRed = 255;
                green = 0; subGreen = 00;
                blue = 0; subBlue = 0;
                alpha = 100; subAlpha = 50;
            }

            float DistThreshold = 40.5f;
            int DistInnerCircleThreshold = 18;

            for (int h = 0; h < cMatrix.Length; h++)
            {
                for (int w = 0; w < cMatrix[0].Length; w++)
                {
                    double dist = Math.Sqrt(Math.Pow(dY - h, 2) + Math.Pow(dX - w, 2));

                    double angleDegree = (Microsoft.Xna.Framework.MathHelper.ToDegrees((float)Math.Atan2(dY - h, dX - w)) + 360) % 360;

                    if (dist < DistThreshold && dist > DistInnerCircleThreshold &&
                        angleDegree < endingFadedAngle && angleDegree > startingFadedAngle)
                        cMatrix[h][w] = Color.FromArgb(
                            (byte)Math.Max(subAlpha, 0),
                            subRed, subGreen, subBlue);

                    //Draw the green angle (stronger)
                    if (dist < DistThreshold && dist > DistInnerCircleThreshold &&
                        angleDegree < endingAngle && angleDegree > startingAngle)
                        cMatrix[h][w] = Color.FromArgb(
                            (byte)(alpha),
                            red, green, blue);
                }
            }

            return cMatrix;
        }
    }
}
