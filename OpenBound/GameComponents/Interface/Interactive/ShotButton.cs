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
using Microsoft.Xna.Framework.Input;
using OpenBound.Extension;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public class ShotButton : Button
    {
        public List<ShotButton> OtherButtons { get; set; }
        private ShotType shotType;

        private readonly Mobile mobile;

        public ShotButton(ButtonType ButtonType, Mobile Mobile, float LayerDepth, ShotType ShotType, bool IsActivated = false, Vector2 ButtonOffset = default(Vector2))
            : base(ButtonType, LayerDepth, null, buttonOffset: ButtonOffset)
        {
            base.IsActivated = IsActivated;

            if (base.IsActivated)
                ChangeButtonState(ButtonAnimationState.Activated, true);

            shotType = ShotType;
            mobile = Mobile;
        }

        public void UpdateCurrentShot()
        {
            if (OtherButtons[0].IsActivated)
                shotType = ShotType.S1;
            else if (OtherButtons[1].IsActivated)
                shotType = ShotType.S2;
            else if (OtherButtons[2].IsActivated)
                shotType = ShotType.SS;
        }

        public void UpdateKeyboard()
        {
            if (InputHandler.IsBeingHeldDown(Keys.Tab))
            {
                OtherButtons.ForEach((x) =>
                {
                    if (x.IsActivated)
                    {
                        x.ChangeButtonState(ButtonAnimationState.Clicked, true);
                    }
                });
            }
            else if (InputHandler.IsBeingReleased(Keys.Tab))
            {
                int prevIndex = 0;
                int nextIndex = 0;

                for (int i = 0; i < OtherButtons.Count; i++)
                {
                    if (OtherButtons[i].IsActivated)
                    {
                        prevIndex = i;
                        break;
                    }
                }

                nextIndex = (prevIndex + 1) % 3;

                if (OtherButtons[nextIndex].IsEnabled == false)
                    nextIndex = (nextIndex + 1) % 3;

                OtherButtons[prevIndex].IsActivated = false;
                OtherButtons[prevIndex].ChangeButtonState(ButtonAnimationState.Normal, true);

                OtherButtons[nextIndex].IsActivated = true;
                OtherButtons[nextIndex].ChangeButtonState(ButtonAnimationState.Activated, true);

                UpdateCurrentShot();

                mobile.ChangeShot(shotType);
            }
        }

        public void UpdateMouse()
        {
            Rectangle collisionRectangle = CalculateCollisionRectangle();

            if (collisionRectangle.Intersects(cursor.CurrentFlipbook.Position))
            {
                if (InputHandler.IsBeingPressed(MKeys.Left))
                {
                    isBeingPressed = true;
                }
                else if (InputHandler.IsBeingHeldDown(MKeys.Left))
                {
                    if (isBeingPressed && buttonAnimationState != ButtonAnimationState.Disabled)
                        ChangeButtonState(ButtonAnimationState.Clicked);
                }
                else if (InputHandler.IsBeingReleased(MKeys.Left))
                {
                    isBeingPressed = false;
                    IsActivated = true;

                    ChangeButtonState(ButtonAnimationState.Activated);

                    OtherButtons.ForEach((x) =>
                    {
                        if (x != this && x.IsEnabled)
                        {
                            x.IsActivated = false;
                            x.ChangeButtonState(ButtonAnimationState.Normal, true);
                        }
                    });

                    UpdateCurrentShot();

                    mobile.ChangeShot(shotType);
                }
                else
                {
                    ChangeButtonState(ButtonAnimationState.Hoover);
                }
            }
            else
            {
                if (InputHandler.IsBeingReleased(MKeys.Right))
                {
                    isBeingPressed = false;
                }

                if (IsActivated)
                    ChangeButtonState(ButtonAnimationState.Activated);
                else
                    ChangeButtonState(ButtonAnimationState.Normal);
            }

#if DEBUG
            DebugCrosshairList[0].Update(new Vector2(collisionRectangle.X, collisionRectangle.Y));
            DebugCrosshairList[1].Update(new Vector2(collisionRectangle.X, collisionRectangle.Y + collisionRectangle.Height));
            DebugCrosshairList[2].Update(new Vector2(collisionRectangle.X + collisionRectangle.Width, collisionRectangle.Y + collisionRectangle.Height));
            DebugCrosshairList[3].Update(new Vector2(collisionRectangle.X + collisionRectangle.Width, collisionRectangle.Y));
#endif
        }

        public override void Update()
        {
            UpdateAttatchedPosition();

            if (!IsEnabled) return;

            UpdateMouse();
            if (OtherButtons[0] == this) UpdateKeyboard();
        }
    }
}
