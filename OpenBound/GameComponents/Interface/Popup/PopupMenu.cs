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
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.Misc;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Popup
{
    public abstract class PopupMenu
    {
        protected Vector2 positionOffset;
        protected Sprite background;
        protected List<CompositeSpriteText> compositeSpriteTextList;
        protected List<SpriteText> spriteTextList;
        protected List<Button> buttonList;
        protected List<Sprite> spriteList;
        protected bool isDraggable;
        protected Vector2 onBeingDraggedStartingPos;
        protected bool shouldRender;

        public Sprite Background
        {
            get => background;
            set
            {
                background = value;

                if (isDraggable)
                {
                    TransparentButton tb = new TransparentButton(PositionOffset - new Vector2(0, background.SpriteHeight - 30) / 2, new Rectangle(0, 0, background.SpriteWidth, 30));
                    tb.OnBeingPressed = OnBeingPressed;
                    tb.OnBeingDragged = OnBeingDragged;
                    buttonList.Add(tb);
                }
            }
        }

        public Action<object> OnConfirm { get; set; }
        public Action<object> OnClose { get; set; }

        public bool ShouldRender
        {
            get => shouldRender; set
            {
                shouldRender = value;
                RealocateElements(default);
            }
        }

        protected Vector2 PositionOffset
        {
            get => positionOffset;
            set
            {
                positionOffset = value;
                UpdateAttatchmentPosition();
            }
        }

        public PopupMenu(bool isDraggable)
        {
            buttonList = new List<Button>();
            spriteTextList = new List<SpriteText>();
            compositeSpriteTextList = new List<CompositeSpriteText>();
            spriteList = new List<Sprite>();
            shouldRender = false;
            this.isDraggable = isDraggable;
        }

        public virtual void RealocateElements(Vector2 delta)
        {
            delta = delta.ToIntegerDomain();

            Background.PositionOffset += delta;
            PositionOffset += delta;
            buttonList.ForEach((x) => x.ButtonOffset += delta);
            spriteTextList.ForEach((x) => x.PositionOffset += delta);
            compositeSpriteTextList.ForEach((x) => x.PositionOffset += delta);

            UpdateAttatchmentPosition();
        }

        protected virtual void UpdateAttatchmentPosition()
        {
            Background.UpdateAttatchmentPosition();
            compositeSpriteTextList.ForEach((x) => x.UpdateAttatchedPosition());
            spriteTextList.ForEach((x) => x.UpdateAttatchedPosition());
            buttonList.ForEach((x) => x.UpdateAttatchedPosition());
            spriteList.ForEach((x) => x.UpdateAttatchmentPosition());
        }

        protected virtual void CloseAction(object sender)
        {
            OnClose?.Invoke(sender);
            ShouldRender = false;
        }

        protected virtual void Destroy()
        {
            PopupHandler.Remove(this);
        }

        public void OnBeingPressed(object sender)
        {
            onBeingDraggedStartingPos = Cursor.Instance.CurrentFlipbook.Position + GameScene.Camera.CameraOffset - Background.PositionOffset;
        }

        public void OnBeingDragged(object sender)
        {
            //Calculating the window position
            Vector2 newWinPos = Cursor.Instance.CurrentFlipbook.Position + GameScene.Camera.CameraOffset;

            //Calculating the movement variation when window is being dragged
            Vector2 delta = newWinPos - onBeingDraggedStartingPos - Background.PositionOffset;

            //Calculating the next window position in order to extract its boundaries
            Vector2 newPos = Background.PositionOffset + Background.Pivot + delta + Parameter.ScreenCenter;

            //Calculating screen offset in case of camera is zoomed (res changed)
            Vector2 cameraZoomOffset = (Parameter.ScreenResolution - Parameter.ScreenResolution / GameScene.Camera.Zoom) / 2;

            //If the window surpasses the screen size on the right side
            if (newPos.X > Parameter.ScreenResolution.X - cameraZoomOffset.X)
                delta += new Vector2(Parameter.ScreenResolution.X - newPos.X - cameraZoomOffset.X, 0);
            //And in the left side
            else if (newPos.X < Background.SpriteWidth + cameraZoomOffset.X)
                //-1 is used to fix the left bug
                delta += new Vector2(background.SpriteWidth - newPos.X - 1 + cameraZoomOffset.X, 0);

            //Bottom
            if (newPos.Y > Parameter.ScreenResolution.Y - cameraZoomOffset.Y)
                delta += new Vector2(0, Parameter.ScreenResolution.Y - newPos.Y - cameraZoomOffset.Y);
            //Top
            else if (newPos.Y < Background.SpriteHeight + cameraZoomOffset.Y)
                delta += new Vector2(0, background.SpriteHeight - newPos.Y + cameraZoomOffset.Y);

            RealocateElements(delta);
            UpdateAttatchmentPosition();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!ShouldRender) return;

            buttonList.ForEach((x) => x.Update());
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!ShouldRender) return;

            compositeSpriteTextList.ForEach((x) => x.Draw(spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
            buttonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            Background.Draw(gameTime, spriteBatch);
            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }
    }
}
