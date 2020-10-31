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

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Input
{
    public enum MKeys
    {
        Left, Middle, Right
    }

    public class InputHandler
    {
        public static KeyboardState CurrentKBState { get; private set; }
        public static KeyboardState PreviousKBState { get; private set; }
        public static MouseState CurrentMState { get; private set; }
        public static MouseState PreviousMState { get; private set; }

        private static Dictionary<Keys, Func<bool>> KBCIsPressed = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBCIsReleased = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBPIsPressed = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBPIsReleased = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBIsBeingPressed = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBIsBeingReleased = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBIsBeingHeldDown = new Dictionary<Keys, Func<bool>>();
        private static Dictionary<Keys, Func<bool>> KBIsBeingHeldUp = new Dictionary<Keys, Func<bool>>();

        private static Dictionary<MKeys, Func<bool>> MCIsPressed = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MCIsReleased = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MPIsPressed = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MPIsReleased = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MIsBeingPressed = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MIsBeingReleased = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MIsBeingHeldDown = new Dictionary<MKeys, Func<bool>>();
        private static Dictionary<MKeys, Func<bool>> MIsBeingHeldUp = new Dictionary<MKeys, Func<bool>>();

        private static List<Keys> inputKBKeyList = new List<Keys>()
        {
            //Movement
            Keys.Left, Keys.Right, Keys.Up, Keys.Down,
            //Change projectile
            Keys.Tab,
            //Shoot
            Keys.Space,
            //Close Game
            Keys.LeftAlt, Keys.F4,
            //Open/Send Message
            Keys.Enter,

            //In-Game Status Bar Hotkey
            Keys.Z,

            //Debug functions
            Keys.F1, Keys.F2,  Keys.F3,  Keys.F4,
            Keys.F5, Keys.F6,  Keys.F7,  Keys.F8,
            Keys.F9, Keys.F10, Keys.F11, Keys.F12,

            Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
            Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
        };

        private static List<MKeys> inputMKeyList = new List<MKeys>()
        {
            MKeys.Left, MKeys.Middle, MKeys.Right
        };

        public static void Initialize()
        {
            foreach (Keys k in inputKBKeyList)
            {
                KBCIsPressed[k] = new Func<bool>(() => CurrentKBState.IsKeyDown(k));
                KBCIsReleased[k] = new Func<bool>(() => CurrentKBState.IsKeyUp(k));
                KBPIsPressed[k] = new Func<bool>(() => PreviousKBState.IsKeyDown(k));
                KBPIsReleased[k] = new Func<bool>(() => PreviousKBState.IsKeyUp(k));
                KBIsBeingPressed[k] = new Func<bool>(() => KBCIsPressed[k]() && KBPIsReleased[k]());
                KBIsBeingReleased[k] = new Func<bool>(() => KBCIsReleased[k]() && KBPIsPressed[k]());
                KBIsBeingHeldDown[k] = new Func<bool>(() => KBCIsPressed[k]() && KBPIsPressed[k]());
                KBIsBeingHeldUp[k] = new Func<bool>(() => KBCIsReleased[k]() && KBPIsReleased[k]());
            }

            foreach (MKeys k in inputMKeyList)
            {
                MCIsPressed[k] = new Func<bool>(() => SelectState(CurrentMState, k) == ButtonState.Pressed);
                MCIsReleased[k] = new Func<bool>(() => SelectState(CurrentMState, k) == ButtonState.Released);
                MPIsPressed[k] = new Func<bool>(() => SelectState(PreviousMState, k) == ButtonState.Pressed);
                MPIsReleased[k] = new Func<bool>(() => SelectState(PreviousMState, k) == ButtonState.Released);
                MIsBeingPressed[k] = new Func<bool>(() => MCIsPressed[k]() && MPIsReleased[k]());
                MIsBeingReleased[k] = new Func<bool>(() => MCIsReleased[k]() && MPIsPressed[k]());
                MIsBeingHeldDown[k] = new Func<bool>(() => MCIsPressed[k]() && MPIsPressed[k]());
                MIsBeingHeldUp[k] = new Func<bool>(() => MCIsReleased[k]() && MPIsReleased[k]());
            }

            CurrentKBState = PreviousKBState = Keyboard.GetState();
            CurrentMState = PreviousMState = Mouse.GetState();
        }

        private static ButtonState SelectState(MouseState ms, MKeys k)
        {
            switch (k)
            {
                case MKeys.Left:
                    return ms.LeftButton;
                case MKeys.Right:
                    return ms.RightButton;
                default:
                    return ms.MiddleButton;
            }
        }

        public static bool IsCKDown(Keys k) => KBCIsPressed[k]();
        public static bool IsCKUp(Keys k) => KBCIsReleased[k]();
        public static bool IsPKDown(Keys k) => KBCIsPressed[k]();
        public static bool IsPKUp(Keys k) => KBCIsReleased[k]();
        public static bool IsBeingPressed(Keys k) => KBIsBeingPressed[k]();
        public static bool IsBeingReleased(Keys k) => KBIsBeingReleased[k]();
        public static bool IsBeingHeldDown(Keys k) => KBIsBeingHeldDown[k]();
        public static bool IsBeingHeldUp(Keys k) => KBIsBeingHeldUp[k]();

        public static bool IsCKDown(MKeys k) => MCIsPressed[k]();
        public static bool IsCKUp(MKeys k) => MCIsPressed[k]();
        public static bool IsPKDown(MKeys k) => MPIsPressed[k]();
        public static bool IsPKUp(MKeys k) => MPIsReleased[k]();
        public static bool IsBeingPressed(MKeys k) => MIsBeingPressed[k]();
        public static bool IsBeingReleased(MKeys k) => MIsBeingReleased[k]();
        public static bool IsBeingHeldDown(MKeys k) => MIsBeingHeldDown[k]();
        public static bool IsBeingHeldUp(MKeys k) => MIsBeingHeldUp[k]();



        public static void Update()
        {
            PreviousKBState = CurrentKBState;
            CurrentKBState = Keyboard.GetState();

            PreviousMState = CurrentMState;
            CurrentMState = Mouse.GetState();
        }
    }
}
