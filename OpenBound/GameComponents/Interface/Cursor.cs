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
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Level;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface
{
    public enum MouseCursorState
    {
        Idle,
        Click,
        DragCamera,
        DragShot,

        ScrollN,
        ScrollNE,
        ScrollNW,

        ScrollS,
        ScrollSE,
        ScrollSW,

        ScrollE,
        ScrollW,
    }

    public class Cursor
    {
        private static Dictionary<MouseCursorState, Flipbook> FlipbookDictionary
            = new Dictionary<MouseCursorState, Flipbook>()
            {
                { MouseCursorState.Idle,       new Flipbook(default, default,                22, 22, "Interface/Cursor/Cursor",      new AnimationInstance() { StartingFrame = 0, EndingFrame = 16, TimePerFrame = 0.1f },                                          DepthParameter.Cursor) },
                { MouseCursorState.Click,      new Flipbook(default, default,                22, 22, "Interface/Cursor/CursorClick", new AnimationInstance() { StartingFrame = 0, EndingFrame = 09, TimePerFrame = 0.2f },                                          DepthParameter.Cursor) },

                { MouseCursorState.DragCamera, new Flipbook(default, new Vector2(11, 11),    22, 22, "Interface/Cursor/DragCamera",  new AnimationInstance() { StartingFrame = 0, EndingFrame = 4, TimePerFrame = 0.1f, AnimationType = AnimationType.FowardStop }, DepthParameter.Cursor) },
                { MouseCursorState.DragShot,   new Flipbook(default, new Vector2(12, 11.5f), 24, 23, "Interface/Cursor/DragShot",    new AnimationInstance() { StartingFrame = 0, EndingFrame = 5, TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },

                { MouseCursorState.ScrollN,    new Flipbook(default, new Vector2(12, 0),      24, 23, "Interface/Cursor/ScrollN",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollS,    new Flipbook(default, new Vector2(12, 23),     24, 23, "Interface/Cursor/ScrollS",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollE,    new Flipbook(default, new Vector2(23, 12),     24, 23, "Interface/Cursor/ScrollE",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollW,    new Flipbook(default, new Vector2(0, 11.5f),   24, 23, "Interface/Cursor/ScrollW",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },

                { MouseCursorState.ScrollNE,   new Flipbook(default, new Vector2(24, 0),      24, 23, "Interface/Cursor/ScrollNE",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollNW,   new Flipbook(default, default,                 24, 23, "Interface/Cursor/ScrollNW",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollSE,   new Flipbook(default, new Vector2(24, 23),     24, 23, "Interface/Cursor/ScrollSE",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
                { MouseCursorState.ScrollSW,   new Flipbook(default, new Vector2(0, 23),      24, 23, "Interface/Cursor/ScrollSW",    new AnimationInstance(){ StartingFrame = 0, EndingFrame = 7,  TimePerFrame = 0.1f },                                           DepthParameter.Cursor) },
            };

        private static Cursor instance;
        public static Cursor Instance
        {
            get { return instance ?? (instance = new Cursor()); }
            private set { instance = value; }
        }

        public MouseCursorState MouseCursorState { get; private set; }
        public Flipbook CurrentFlipbook { get; private set; }
        private Vector2 cameraOffset;

#if DEBUG
        DebugCrosshair debug;
        DebugCrosshair debug2;
#endif

        private Cursor()
        {
            Instance = this;
            MouseCursorState = MouseCursorState.Idle;

            CurrentFlipbook = FlipbookDictionary[MouseCursorState];

#if DEBUG
            debug = new DebugCrosshair(Color.Red);
            debug2 = new DebugCrosshair(Color.Green);

            DebugHandler.Instance.Add(debug);
            DebugHandler.Instance.Add(debug2);
#endif
        }

        /// <summary>
        /// Toggles between the Menu cursor and the InGame cursor, setting the current mouse state to idle and updating its position
        /// </summary>
        /// <param name="NewState"></param>
        public void SetMouseCursorSceneState()
        {
            FlipbookDictionary[MouseCursorState.Idle].Position = CurrentFlipbook.Position;
            ChangeState(MouseCursorState.Idle);
        }

        /// <summary>
        /// Checks if the requested state change is valid based on the cursor rules for each menu
        /// </summary>
        /// <param name="NewState"></param>
        /// <returns></returns>
        private bool IsChangeStateValid(MouseCursorState NewState)
        {
            //Check if the state change is pointless
            if (NewState == MouseCursorState) return false;

            //Checks if mouse is in "menu mode" wich can only assume limited states (Idle/Click)
            if (GameInformation.Instance.GameState == GameState.Menu &&
                !(NewState == MouseCursorState.Click || NewState == MouseCursorState.Idle))
                return false;

            return true;
        }

        private void ChangeState(MouseCursorState NewState)
        {
            if (!IsChangeStateValid(NewState)) return;

            MouseCursorState = NewState;
            CurrentFlipbook = FlipbookDictionary[MouseCursorState];
            CurrentFlipbook.ResetCurrentAnimation();
        }

        /// <summary>
        /// Updates the mouse position based on its location or input. Actually this function does not support drag yet, nor conditions for menus.
        /// </summary>
        /// <param name="MouseState"></param>
        /// <param name="PreviousMouseState"></param>
        /// <param name="SlidingMask"></param>
        public void UpdateFlipbookState(Camera camera)
        {
            //Priority list for mouse inputs
            //1. Drag Camera
            //2. Scroll
            //3. Left Click
            //4. Idle

            cameraOffset = default;
            MouseState cMState = InputHandler.CurrentMState;
            MouseState pMState = InputHandler.PreviousMState;

            //Drag Camera Condition
            #region DRAG
            if (GameInformation.Instance.GameState == GameState.InGame)
            {
                if (InputHandler.IsBeingHeldDown(MKeys.Right))
                {
                    ChangeState(MouseCursorState.DragCamera);
                    cameraOffset += new Vector2(cMState.Position.X - pMState.Position.X, cMState.Position.Y - pMState.Position.Y);
                    return;
                }
                else if (InputHandler.IsBeingReleased(MKeys.Right))
                {
                    Mouse.SetPosition((int)Parameter.ScreenCenter.X, (int)Parameter.ScreenCenter.Y);
                }
            }
            #endregion DRAG

            //Calculating Scroll (if possible)
            #region SCROLL
            byte slidingMask = 0x0;
            float x = 0, y = 0;

            if (cMState.Position.X < Parameter.CameraSlidingStartThreshold)
            {
                x += Parameter.CameraSlidingSensibility;
                slidingMask |= Parameter.LeftMask;
            }
            else if (cMState.Position.X + Parameter.CameraSlidingStartThreshold > Parameter.ScreenResolution.X)
            {
                x -= Parameter.CameraSlidingSensibility;
                slidingMask |= Parameter.RightMask;
            }

            if (cMState.Position.Y < Parameter.CameraSlidingStartThreshold)
            {
                y += Parameter.CameraSlidingSensibility;
                slidingMask |= Parameter.TopMask;
            }
            else if (cMState.Position.Y + Parameter.CameraSlidingStartThreshold > Parameter.ScreenResolution.Y)
            {
                y -= Parameter.CameraSlidingSensibility;
                slidingMask |= Parameter.BottomMask;
            }

            if (slidingMask > 0)
            {
                //Drag Camera NE
                if ((slidingMask & Parameter.TopMask) > 0 && (slidingMask & Parameter.RightMask) > 0) ChangeState(MouseCursorState.ScrollNE);
                //Drag Camera NW
                else if ((slidingMask & Parameter.TopMask) > 0 && (slidingMask & Parameter.LeftMask) > 0) ChangeState(MouseCursorState.ScrollNW);
                //Drag Camera SE
                else if ((slidingMask & Parameter.BottomMask) > 0 && (slidingMask & Parameter.RightMask) > 0) ChangeState(MouseCursorState.ScrollSE);
                //Drag Camera SW
                else if ((slidingMask & Parameter.BottomMask) > 0 && (slidingMask & Parameter.LeftMask) > 0) ChangeState(MouseCursorState.ScrollSW);
                //Drag Camera N
                else if ((slidingMask & Parameter.TopMask) > 0) ChangeState(MouseCursorState.ScrollN);
                //Drag Camera S
                else if ((slidingMask & Parameter.BottomMask) > 0) ChangeState(MouseCursorState.ScrollS);
                //Drag Camera E
                else if ((slidingMask & Parameter.RightMask) > 0) ChangeState(MouseCursorState.ScrollE);
                //Drag Camera W
                else if ((slidingMask & Parameter.LeftMask) > 0) ChangeState(MouseCursorState.ScrollW);

                cameraOffset += new Vector2(x, y);

                camera.CancelTracking();

                return;
            }
            #endregion SCROLL

            //Left Click
            #region LEFT CLICK
            if (InputHandler.IsBeingHeldDown(MKeys.Left))
            {
                ChangeState(MouseCursorState.Click); //ChangeState(MouseCursorState.DragShot);
                return;
            }
            #endregion LEFT CLICK

            //IDLE
            #region IDLE
            //if (MouseState.RightButton == ButtonState.Released && PreviousMouseState.RightButton == ButtonState.Released &&
            //MouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Released)
            //{
            ChangeState(MouseCursorState.Idle);
            //}
            #endregion IDLE
        }

        public void UpdateCursorPosition(Camera camera)
        {
            Matrix invMat = Matrix.Invert(camera.Transform);
            Vector2 mouseRealPos = InputHandler.CurrentMState.Position.ToVector2();
            Vector2 newPosition = Vector2.Transform(mouseRealPos, invMat);

            float centerX = Parameter.ScreenCenter.X / camera.Zoom.X;
            float centerY = Parameter.ScreenCenter.Y / camera.Zoom.Y;

            float newPosX = MathHelper.Clamp(newPosition.X, -centerX - camera.CameraOffset.X, centerX - camera.CameraOffset.X);
            float newPosY = MathHelper.Clamp(newPosition.Y, -centerY - camera.CameraOffset.Y, centerY - camera.CameraOffset.Y);

            CurrentFlipbook.Position = new Vector2(newPosX, newPosY);

#if DEBUG
            debug.Update(CurrentFlipbook.Position - CurrentFlipbook.Pivot);
            debug2.Update(CurrentFlipbook.Position);
#endif
        }

        public void UpdateCursorScale(Camera camera)
        {
            CurrentFlipbook.Scale = Vector2.One / camera.Zoom;
        }

        public void Update(Camera camera)
        {
            camera.CameraOffset += cameraOffset;
            UpdateFlipbookState(camera);
            UpdateCursorPosition(camera);
            UpdateCursorScale(camera);
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            CurrentFlipbook.Draw(GameTime, SpriteBatch);
        }
    }
}
