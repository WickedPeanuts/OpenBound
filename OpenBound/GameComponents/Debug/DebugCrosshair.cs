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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Asset;
using OpenBound.GameComponents.Level.Scene;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Debug
{
    public class DebugCrosshair : DebugElement
    {
        public DebugCrosshair(Color Color)
        {
            this.Color = Color;
            Sprite = new Sprite(@"Debug/DebugCrosshair",
                layerDepth: 1f,
                shouldCopyAsset: true);

            Color[] color = new Color[Sprite.Texture2D.Width * Sprite.Texture2D.Height];
            Sprite.Texture2D.GetData(color);

            for (int id = 0; id < color.Length; id++)
            {
                color[id] = Color;
            }

            foreach (int i in
                new List<int>() {
                8, 9, 11, 12, 15, 16, 18, 19, 29, 30, 32, 33, 36, 37, 39, 40
                })
            {
                color[i] = Color.Transparent;
            }

            Texture2D newTex = AssetHandler.Instance.CreateAsset(7, 7);
            newTex.SetData(color);
            Sprite.Texture2D = newTex;
        }

        public void Update(Vector2 Position)
        {
            Sprite.Position = Position;
        }
    }
}
#endif