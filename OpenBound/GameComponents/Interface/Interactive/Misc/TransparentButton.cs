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
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Interactive.Misc
{
    public class TransparentButton : Button
    {
        public static ButtonPreset buildDummyButtonPreset(Rectangle interactionArea)
        {
            return new ButtonPreset()
            {
                SpritePath = "Misc/Dummy",
                StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                { { ButtonAnimationState.Normal, interactionArea }, }
            };
        }

        public TransparentButton(Vector2 buttonOffset, Rectangle interactionArea)
            : base(ButtonType.Dummy, 0, default, buttonOffset, buildDummyButtonPreset(interactionArea))
        {

        }

        public void SetInteractionArea(Rectangle rectangle)
        {
            ButtonSprite.SourceRectangle = rectangle;
        }
    }
}
