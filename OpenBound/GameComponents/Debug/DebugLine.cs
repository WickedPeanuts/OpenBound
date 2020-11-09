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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Asset;

namespace OpenBound.GameComponents.Debug
{
    public class DebugLine : DebugElement
    {
        public DebugLine(Color Color)
        {
            Sprite = new Sprite("Debug/DebugDot",
                layerDepth: 1f,
                shouldCopyAsset: true);

            Color[] color = new Color[1];
            Sprite.Texture2D.GetData(color);

            color[0] = Color;

            Texture2D newTex = AssetHandler.Instance.RequestTextureCopy("Debug/DebugDot");
            newTex.SetData(color);
            Sprite.Texture2D = newTex;
        }


        public void Update(Vector2 Origin, Vector2 Destiny)
        {
            int dist = (int)Helper.EuclideanDistance(Origin, Destiny);
            Sprite.SourceRectangle = new Rectangle(0, 0, dist, 1);
            Sprite.Rotation = (float)Helper.AngleBetween(Origin, Destiny);
            Sprite.Position = (Destiny + Origin) / 2f;
            Sprite.Pivot = new Vector2(dist / 2, 0);
        }
    }
}
#endif