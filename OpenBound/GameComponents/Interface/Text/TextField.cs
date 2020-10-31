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
using OpenBound.Extension;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Text
{
    public class TextField : IDisposable
    {
        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            set {
                position = value.ToIntegerDomain();
                UpdatePostion();
            }
        }

        public SpriteText Text { get; private set; }

        private SpriteText textPointer;

        //Control
        public bool IsActive { get; set; }
        bool isEnabled;

        //Collision Box preset
        private int boxWidth, boxHeight;
        private Rectangle collisionRectangle;

        //Maximum character
        private readonly int maximumTextLength;

        //Flow Control
        private float textPointerBlinkingTime;
        private float keyInputTimer;
        private int textPointerLocation;

        //Tabing Index
        public List<TextField> TabIndex;

        //Positioning
        private Vector2 alignmentOffset;

        //Event
        public Action<object> OnActive { get; set; }
        public Action<string> OnTextChange { get; set; }
        public Dictionary<Keys, Action<object>> OnPressKey { get; set; }

#if DEBUG
        DebugRectangle debugRectangle;
#endif

        public TextField(Vector2 positionOffset, int boxWidth, int boxHeight, int maximumTextLength, FontTextType fontTextType, Color color, float layerDepth, Color outlineColor = default)
        {
            this.maximumTextLength = maximumTextLength;
            this.boxHeight = boxHeight;
            this.boxWidth = boxWidth;

            textPointerBlinkingTime = 0;
            keyInputTimer = 0;
            textPointerLocation = 0;

            //Text
            Text = new SpriteText(fontTextType, "", color, Alignment.Left, layerDepth, default, outlineColor);
            textPointer = new SpriteText(fontTextType, "|", Parameter.NameplateGuildColor, Alignment.Left, layerDepth + 0.01f, default, Parameter.NameplateGuildOutlineColor);
            alignmentOffset = Text.MeasureSubstring(" ").ToIntegerDomain();

            DeactivateElement();

            isEnabled = true;

            OnPressKey = new Dictionary<Keys, Action<object>>();

#if DEBUG
            debugRectangle = new DebugRectangle(Color.CornflowerBlue);
            DebugHandler.Instance.Add(debugRectangle);
#endif

            Position = positionOffset;
        }

        private void UpdatePostion()
        {
            collisionRectangle = new Rectangle((int)position.X, (int)position.Y, boxWidth, boxHeight);
            Text.Position = position + new Vector2(0, (int)(-alignmentOffset.Y / 2 + boxHeight / 2));

            Vector2 blinkingMeasured = textPointer.SpriteFont.MeasureString(textPointer.Text).ToIntegerDomain();
            Vector2 textPointerMeasured = Text.SpriteFont.MeasureString(Text.Text.Substring(0, textPointerLocation)).ToIntegerDomain();

            textPointer.Position = position
                + new Vector2(textPointerMeasured.X - blinkingMeasured.X / 2, 0).ToIntegerDomain()
                + new Vector2(0, -alignmentOffset.Y / 2 + boxHeight / 2).ToIntegerDomain();

#if DEBUG
            debugRectangle.Update(
                new Vector2[]
                {
                    new Vector2(collisionRectangle.Left, collisionRectangle.Top),
                    new Vector2(collisionRectangle.Left, collisionRectangle.Bottom),
                    new Vector2(collisionRectangle.Right, collisionRectangle.Bottom),
                    new Vector2(collisionRectangle.Right, collisionRectangle.Top),
                });
#endif
        }

        public void Update(GameTime gameTime)
        {
            if (!isEnabled) return;

            CheckMouseIntersection();
            UpdateBlinkingBar(gameTime);
            UpdateActivatedTextInputs(gameTime);
            UpdatePostion();
        }

        private void CheckMouseIntersection()
        {
            Rectangle mouseRect = new Rectangle(
             (int)Cursor.Instance.CurrentFlipbook.Position.X,
             (int)Cursor.Instance.CurrentFlipbook.Position.Y, 1, 1);

            if (mouseRect.Intersects(collisionRectangle) && InputHandler.IsBeingReleased(MKeys.Left))
            {
                ActivateElement();
            }
        }

        public void Enable()
        {
            isEnabled = true;
        }

        public void Disable()
        {
            isEnabled = false;
            DeactivateElement();
        }

        public void DeactivateElement()
        {
            Text.Color = Parameter.TextColorTextBoxSelectedMessage;
            IsActive = false;
            Uninstall();
        }

        public void ActivateElement()
        {
            if (IsActive) return;

            if (TabIndex != null)
            {
                foreach (TextField tf in TabIndex)
                {
                    tf.DeactivateElement();
                }
            }

            Text.Color = Text.BaseColor;
            Text.OutlineColor = Text.OutlineBaseColor;
            OnActive?.Invoke(this);
            IsActive = true;
            Install();
        }

        public void Uninstall()
        {
            Game1.Instance.Window.TextInput -= GetKeyboardInput;
        }

        public void Install()
        {
            Game1.Instance.Window.TextInput += GetKeyboardInput;
        }

        private void UpdateActivatedTextInputs(GameTime gameTime)
        {
            if (!IsActive) return;

            List<Keys> pressedKeys = InputHandler.PreviousKBState.GetPressedKeys()
                .Intersect(InputHandler.CurrentKBState.GetPressedKeys())
                .ToList();

            if (pressedKeys.Count == 0)
            {
                keyInputTimer = 0;
                return;
            }

            keyInputTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyInputTimer > 0) return;

            pressedKeys.ForEach((key) =>
            {
                if (key == Keys.Left)
                {
                    textPointerLocation--;
                    if (textPointerLocation < 0) textPointerLocation = 0;
                }
                else if (key == Keys.Right)
                {
                    textPointerLocation++;
                    if (textPointerLocation > Text.Text.Length) textPointerLocation = Text.Text.Length;
                }
                else if (key == Keys.Delete)
                {
                    if (textPointerLocation + 1 > Text.Text.Length) return;

                    string before = Text.Text.Substring(0, textPointerLocation);
                    string after = Text.Text.Substring(textPointerLocation + 1, Text.Text.Length - textPointerLocation - 1);

                    UpdateText(before + after);
                }
                else if (key == Keys.Home)
                {
                    textPointerLocation = 0;
                }
                else if (key == Keys.End)
                {
                    textPointerLocation = Text.Text.Length;
                }
            });

            textPointerBlinkingTime = 0;
            keyInputTimer = 0.10f;
        }

        private void UpdateBlinkingBar(GameTime gameTime)
        {
            textPointerBlinkingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            textPointerBlinkingTime = textPointerBlinkingTime % 1;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Text.Draw(spriteBatch);
            if (IsActive && textPointerBlinkingTime < 0.5)
                textPointer.Draw(spriteBatch);
        }

        public void ClearText()
        {
            UpdateText("");
            textPointerLocation = 0;
        }

        private void GetKeyboardInput(object gameWindow, TextInputEventArgs pressedKey)
        {
            if (!IsActive) return;

            if (OnPressKey.ContainsKey(pressedKey.Key))
                OnPressKey[pressedKey.Key].Invoke(this);

            //Backspace
            if (pressedKey.Key == Keys.Back)
            {
                if (textPointerLocation == 0) return;

                string before = Text.Text.Substring(0, textPointerLocation - 1);
                string after = Text.Text.Substring(textPointerLocation, Text.Text.Length - textPointerLocation);

                UpdateText(before + after);
                textPointerLocation--;
            }
            else if (pressedKey.Key == Keys.Tab)
            {
                if (TabIndex == null) return;

                int currentIndex = TabIndex.IndexOf(this);
                currentIndex++;

                if (currentIndex > TabIndex.Count - 1)
                    currentIndex = 0;

                DeactivateElement();
                TabIndex[currentIndex].ActivateElement();
            }
            else if (pressedKey.Character < 32 || pressedKey.Character > 254)
            {
                return;
            }
            else
            {
                if (Text.Text.Length > maximumTextLength) return;

                string before = Text.Text.Substring(0, textPointerLocation);
                string after = Text.Text.Substring(textPointerLocation, Text.Text.Length - textPointerLocation);
                UpdateText(before + pressedKey.Character + after);
                textPointerLocation++;
            }
        }

        public void UpdateText(string newText)
        {
            Text.Text = newText;
            OnTextChange?.Invoke(newText);
        }

        public void Dispose()
        {
            Uninstall();
        }
    }
}
