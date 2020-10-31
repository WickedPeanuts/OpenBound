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

namespace OpenBound.GameComponents.Animation
{
    public abstract class Renderable
    {
        public virtual Vector2 Position { get; set; }

        public float LayerDepth;
        public Vector2 PositionOffset;
        public Vector2 Pivot;
        public Texture2D Texture2D;
        public SpriteEffects Effect;
        public int SpriteWidth;
        public int SpriteHeight;
        public float Rotation;
        public Rectangle SourceRectangle;
        protected string Texture2DPath;
        public Vector2 Scale;
        public Color BaseColor;
        public Color Color;

        public Renderable()
        {
            Scale = Vector2.One;
            BaseColor = Color = Color.White;
        }

        public virtual void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            SpriteBatch.Draw(
               texture: Texture2D,
               position: Position,
               sourceRectangle: SourceRectangle,
               color: Color,
               rotation: Rotation,
               origin: Pivot,
               scale: Scale,
               effects: Effect,
               layerDepth: LayerDepth);
        }

        public void SetTransparency(float Alpha)
        {
            Color = new Color(new Vector4(Alpha, Alpha, Alpha, Alpha));
        }

        public void HideElement()
        {
            Color = Color.Transparent;
        }

        public void ShowElement()
        {
            Color = Color.White;
        }
    }
}
