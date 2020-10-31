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
using OpenBound.Extension;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Asset;

namespace OpenBound.GameComponents.Animation
{
    public class Sprite : Renderable
    {
        public Rectangle CollisionBox => new Rectangle((int)(Position.X - Pivot.X), (int)(Position.Y - Pivot.Y), SourceRectangle.Width, SourceRectangle.Height);

        private Sprite() { }

        public Sprite(int textureWidth, int textureHeight, Vector2 position = default, float layerDepth = 0, Rectangle sourceRectangle = default)
        {
            LayerDepth = layerDepth;
            Position = PositionOffset = position;
            SourceRectangle = sourceRectangle;

            Texture2D = AssetHandler.Instance.CreateAsset(textureWidth, textureHeight);

            SpriteWidth = Texture2D.Width;
            SpriteHeight = Texture2D.Height;
        }

        public Sprite(string texture2DPath, Color color, Vector2 position = default, float layerDepth = 0, Rectangle sourceRectangle = default)
        {
            LayerDepth = layerDepth;
            Position = PositionOffset = position;
            SourceRectangle = sourceRectangle;

            Texture2D = AssetHandler.Instance.RequestTextureCopy(texture2DPath, color);

            SpriteWidth = Texture2D.Width;
            SpriteHeight = Texture2D.Height;
        }

        public Sprite(string texture2DPath, Vector2 position = default, float layerDepth = 0, Rectangle sourceRectangle = default, bool shouldCopyAsset = false)
        {
            LayerDepth = layerDepth;
            //this mod can break stuff
            Position = PositionOffset = position;
            Texture2DPath = texture2DPath;
            SourceRectangle = sourceRectangle;

            if (shouldCopyAsset)
                Texture2D = AssetHandler.Instance.RequestTextureCopy(texture2DPath);
            else
                Texture2D = AssetHandler.Instance.RequestTexture(texture2DPath);

            SpriteWidth = Texture2D.Width;
            SpriteHeight = Texture2D.Height;
            Pivot = new Vector2(SpriteWidth / 2, SpriteHeight / 2);

            //this mod can also break stuff
            if (sourceRectangle == default)
                SourceRectangle = new Rectangle(0, 0, SpriteWidth, SpriteHeight);
            //else
            //Pivot = new Vector2(SourceRectangle.Width / 2, SourceRectangle.Height / 2);
        }

        public void UpdateAttatchmentPosition()
        {
            Position = PositionOffset.ToIntegerDomain() - GameScene.Camera.CameraOffset;
        }
    }
}
