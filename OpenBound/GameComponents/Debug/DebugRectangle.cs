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
using System.Collections.Generic;

namespace OpenBound.GameComponents.Debug
{
    public class DebugRectangle : DebugElement
    {
        List<DebugLine> dLineList = new List<DebugLine>();

        public DebugRectangle(Color color)
        {
            dLineList = new List<DebugLine>()
            {
                new DebugLine(color),
                new DebugLine(color),
                new DebugLine(color),
                new DebugLine(color),
            };
        }

        public void SetColor(Color color)
        {
            dLineList.ForEach((x) => x.Sprite.Color = color);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            dLineList.ForEach((x) => x.Draw(spriteBatch));
        }

        public void Update(Vector2[] corner)
        {
            dLineList[0].Update(corner[0], corner[1]);
            dLineList[1].Update(corner[1], corner[2]);
            dLineList[2].Update(corner[2], corner[3]);
            dLineList[3].Update(corner[3], corner[0]);
        }

        public void Update(Rectangle rectangle)
        {
            dLineList[0].Update(new Vector2(rectangle.X, rectangle.Y),                                      new Vector2(rectangle.X,                   rectangle.Y + rectangle.Height));
            dLineList[1].Update(new Vector2(rectangle.X,                   rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height));
            dLineList[2].Update(new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height), new Vector2(rectangle.X + rectangle.Width, rectangle.Y));
            dLineList[3].Update(new Vector2(rectangle.X + rectangle.Width, rectangle.Y),                    new Vector2(rectangle.X,                   rectangle.Y));
        }
    }
}
#endif