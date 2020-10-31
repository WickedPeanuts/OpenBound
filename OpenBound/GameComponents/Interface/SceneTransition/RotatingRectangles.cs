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
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Level.Scene;
using System;

namespace OpenBound.GameComponents.Interface.SceneTransition
{
    class RotatingRectangles : MenuTransitionEffect
    {
        Sprite leftSquare, rightSquare;
        Vector2 LeftSquareOffset, RightSquareOffset;

        float maxAngle;
        float newRectangleRot;

        public RotatingRectangles(Action ClosedFunction)
            : base(1, 1, ClosedFunction)
        {
            transitioningAction = ClosedFunction;

            maxAngle = MathHelper.ToRadians(60);

            elapsedTime = 0f;
            totalElapsedTime = 0f;

            leftSquare = new Sprite(@"Interface/InGame/Scene/TransitionEffect/BlackSquare",
                layerDepth: 1);
            rightSquare = new Sprite(@"Interface/InGame/Scene/TransitionEffect/BlackSquare",
                layerDepth: 1);

            leftSquare.Pivot = new Vector2(0, 0);
            rightSquare.Pivot = new Vector2(1, 1);

            float scale = Math.Max(Parameter.ScreenResolution.X, Parameter.ScreenResolution.Y);
            leftSquare.Scale = rightSquare.Scale = new Vector2(scale * 2, scale * 2);

            spriteList.Add(leftSquare);
            spriteList.Add(rightSquare);
        }

        public override void Update(GameTime GameTime)
        {
            Vector2 screenSize = Parameter.ScreenResolution / GameScene.Camera.Zoom;
            LeftSquareOffset = new Vector2(-screenSize.X, screenSize.Y) / 2;
            RightSquareOffset = new Vector2(screenSize.X, -screenSize.Y) / 2;

            leftSquare.Position = LeftSquareOffset - GameScene.Camera.CameraOffset;
            rightSquare.Position = RightSquareOffset - GameScene.Camera.CameraOffset;

            elapsedTime = (float)GameTime.ElapsedGameTime.TotalSeconds;
            totalElapsedTime += elapsedTime;

            if (state == MenuAnimationState.Initialized)
            {
                if (totalElapsedTime > initialFreezetime)
                {
                    newRectangleRot = 0;
                    state = MenuAnimationState.Closing;
                    elapsedTime = 0;
                }
            }
            else if (state == MenuAnimationState.Closing)
            {
                float factor = (newRectangleRot * 100 / maxAngle) * 0.03f + 0.2f;
                if (factor == 0f) factor = 0.001f;

                newRectangleRot += maxAngle * elapsedTime / motionTime * factor;

                if (Math.Abs(newRectangleRot) > maxAngle)
                {
                    newRectangleRot = maxAngle;
                    state = MenuAnimationState.Closed;
                }

                leftSquare.Rotation = rightSquare.Rotation = -newRectangleRot;
            }
            else if (state == MenuAnimationState.Closed)
            {
                state = MenuAnimationState.ExecutingClosedOperation;

                // wait for the menu to be ready in order to keep executing the transitioning effects
                // otherwise the engine will skip all the possible frames during the initialization of the scene elements.
                // when recovering, all the frames skipped will be stuck on ExecutingClosedOperation, making no further actions/animations
                // until the timer reaches the instance elapsed time.
                transitioningAction?.Invoke();
            }
            else if (state == MenuAnimationState.ExecutingClosedOperation)
            {
                if (!SceneHandler.Instance.IsChangingScene)
                    state = MenuAnimationState.Opening;
            }
            else if (state == MenuAnimationState.Opening)
            {
                float factor = (newRectangleRot * 100 / maxAngle) * 0.03f + 0.2f;
                if (factor == 0f) factor = 0.001f;

                newRectangleRot -= maxAngle * elapsedTime / motionTime * factor;

                if (newRectangleRot < 0)
                {
                    newRectangleRot = 0;
                    state = MenuAnimationState.Finalized;
                }

                leftSquare.Rotation = rightSquare.Rotation = -newRectangleRot;
            }
            else if (state == MenuAnimationState.Finalized)
            {
                leftSquare.Scale = rightSquare.Scale = Vector2.Zero;
            }
        }
    }
}