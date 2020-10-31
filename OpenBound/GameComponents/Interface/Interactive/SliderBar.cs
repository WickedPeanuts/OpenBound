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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive.Misc;
using OpenBound.GameComponents.Level.Scene;
using System;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public class SliderBar
    {
        TransparentButton interactionRangeButton;
        ProgressBar slidingBar;

        Sprite slidingBarFrame;
        Sprite slidingGraduationMark;


        public Action<object> OnBeingDragged;
        Action<object> OnBeingReleased { get => interactionRangeButton.OnBeingReleased; set => interactionRangeButton.OnBeingReleased = value; }
        Action<object> OnBeingPressed { get => interactionRangeButton.OnBeingPressed; set => interactionRangeButton.OnBeingPressed = value; }

        public SliderBar(Vector2 position, int defaultValue)
        {
            slidingBar = new ProgressBar("Interface/Popup/Blue/Options/Bar", LayerDepth: DepthParameter.InterfacePopupText, BarOffset: position);
            slidingBar.Intensity = defaultValue;

            slidingGraduationMark = new Sprite("Interface/Popup/Blue/Options/BarIntensityPointer", layerDepth: DepthParameter.InterfacePopupButtons);
            slidingGraduationMark.PositionOffset = position + new Vector2(slidingBar.MeasureCurrentSize.X - slidingBar.BarSprite.SpriteWidth / 2, -5);

            slidingBarFrame = new Sprite("Interface/Popup/Blue/Options/BarFrame", layerDepth: DepthParameter.InterfacePopupButtons);
            slidingBarFrame.PositionOffset = position;

            interactionRangeButton = new TransparentButton(position, new Rectangle(0, 0, 145, 20));
            interactionRangeButton.OnBeingDragged = (button) =>
            {
                float startPositionX = slidingBar.PositionOffset.X - slidingBar.BarSprite.SpriteWidth / 2;
                float endPositionX = slidingBar.PositionOffset.X + slidingBar.BarSprite.SpriteWidth / 2;
                float currentX = MathHelper.Clamp(
                    Cursor.Instance.CurrentFlipbook.Position.X + GameScene.Camera.CameraOffset.X,
                    startPositionX, endPositionX);

                slidingGraduationMark.PositionOffset = new Vector2(currentX,
                    slidingGraduationMark.PositionOffset.Y);

                slidingBar.Intensity = (currentX - startPositionX) * 100 / (endPositionX - startPositionX);

                OnBeingDragged?.Invoke(slidingBar);
            };
        }

        public void RealocateElements(Vector2 delta)
        {
            interactionRangeButton.ButtonOffset += delta;
            slidingBar.PositionOffset += delta;
            slidingBarFrame.PositionOffset += delta;
            slidingGraduationMark.PositionOffset += delta;

            UpdateAttatchmentPosition();
        }

        public void UpdateAttatchmentPosition()
        {
            slidingGraduationMark.UpdateAttatchmentPosition();
            slidingBarFrame.UpdateAttatchmentPosition();
            slidingBar.UpdateAttatchmentPosition();
            slidingBar.UpdateBar();
        }

        public void Update()
        {
            interactionRangeButton.Update();
            UpdateAttatchmentPosition();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            slidingBar.Draw(gameTime, spriteBatch);
            slidingBarFrame.Draw(gameTime, spriteBatch);
            slidingGraduationMark.Draw(gameTime, spriteBatch);
        }
    }
}
