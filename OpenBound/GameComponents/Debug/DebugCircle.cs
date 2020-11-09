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

#if DEBUG
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Asset;
using System;

namespace OpenBound.GameComponents.Debug
{
    class DebugCircle : DebugElement
    {
        public int Radius;

        public DebugCircle(Color Color, int Radius)
        {
            this.Radius = Radius;
            this.Color = Color;
            Sprite = new Sprite(@"Debug/DebugCrosshair",
                layerDepth: 1f,
                shouldCopyAsset: true);

            Texture2D newTex = AssetHandler.Instance.CreateAsset(2 * Radius, 2 * Radius);
            Color[][] color2D = new Color[2 * Radius][];

            Vector2 center = new Vector2(Radius, Radius);

            for (int h = 0; h < color2D.Length; h++)
            {
                color2D[h] = new Color[2 * Radius];

                for (int w = 0; w < color2D[0].Length; w++)
                {
                    Vector2 currPos = new Vector2(h, w);

                    if (Math.Floor(Helper.EuclideanDistance(center, currPos)) == Radius - 1)
                    {
                        color2D[h][w] = Color.Pink;
                    }
                }
            }

            newTex.SetData(color2D.ConvertTo1D());

            Sprite.SourceRectangle = new Rectangle(0, 0, 2 * Radius, 2 * Radius);
            Sprite.SpriteWidth = 2 * Radius;
            Sprite.SpriteHeight = 2 * Radius;
            Sprite.Texture2D = newTex;
            Sprite.Pivot = center;
        }

        public void Update(Vector2 Position)
        {
            Sprite.Position = Position;
        }
    }
}
#endif