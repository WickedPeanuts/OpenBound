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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive.Misc;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public enum RadioButtonType
    {
        InterfaceRadioButton,
    }

    public class RadioButtonElement
    {
        public TransparentButton TransparentButton;
        public SpriteText SpriteText;
        public Sprite Sprite;

        public RadioButtonElement(TransparentButton transparentButton, SpriteText spriteText, Sprite sprite)
        {
            TransparentButton = transparentButton; SpriteText = spriteText; Sprite = sprite;
        }

        public void RealocateElements(Vector2 delta)
        {
            Sprite.PositionOffset += delta;
            SpriteText.PositionOffset += delta;
            TransparentButton.ButtonOffset += delta;

            UpdateAttatchmentPosition();
        }

        public void UpdateAttatchmentPosition()
        {
            Sprite.UpdateAttatchmentPosition();
            SpriteText.UpdateAttatchedPosition();
            TransparentButton.UpdateAttatchedPosition();
        }

        public void Update()
        {
            TransparentButton.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            TransparentButton.Draw(gameTime, spriteBatch);
            SpriteText.Draw(spriteBatch);
            Sprite.Draw(gameTime, spriteBatch);
        }
    }

    public class RadioButtonSet
    {
        public Dictionary<RadioButtonType, ButtonPreset> radioButtonStatePresets
            = new Dictionary<RadioButtonType, ButtonPreset>()
        {
            {
                RadioButtonType.InterfaceRadioButton,
                new ButtonPreset() {
                    SpritePath = "Interface/Popup/Blue/Options/RadioButton",
                    StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                    {
                        { ButtonAnimationState.Normal,    new Rectangle( 0, 0, 12, 12) },
                        { ButtonAnimationState.Activated, new Rectangle(12, 0, 12, 12) },
                    }
                }
            }
        };

        private readonly List<RadioButtonElement> radioButtonElementList;
        private readonly ButtonPreset selectedButtonPreset;
        private readonly Vector2 attatchedTextOffset;

        public Action<object> OnClick;

        public RadioButtonSet(RadioButtonType radioButtonType, Vector2 position, Vector2 elementOffset, List<SpriteText> attatchedTextList, Vector2 attatchedTextOffset, int defaultValue, float layerDepth)
        {
            radioButtonElementList = new List<RadioButtonElement>();

            selectedButtonPreset = radioButtonStatePresets[radioButtonType];
            this.attatchedTextOffset = attatchedTextOffset;

            for (int i = 0; i < attatchedTextList.Count; i++)
            {
                //Sprite
                Sprite sprite = new Sprite(selectedButtonPreset.SpritePath, layerDepth: layerDepth, sourceRectangle: selectedButtonPreset.StatePreset[ButtonAnimationState.Normal]);
                sprite.Pivot = new Vector2(6, 6);

                //SpriteText
                attatchedTextList[i].Alignment = Alignment.Left;

                //TransparentButton
                TransparentButton button = new TransparentButton(default, default)
                { OnBeingPressed = RadioButtonOnBeingPressedAction };

                radioButtonElementList.Add(new RadioButtonElement(button, attatchedTextList[i], sprite));

                //Repositioning
                UpdateButtonPosition(i, position + i * elementOffset);
            }

            //Inserting Default Value
            radioButtonElementList[defaultValue].Sprite.SourceRectangle = selectedButtonPreset.StatePreset[ButtonAnimationState.Activated];
        }

        public void RadioButtonOnBeingPressedAction(object sender)
        {
            RadioButtonElement rbE = radioButtonElementList.Find((x) => x.TransparentButton == sender);

            rbE.Sprite.SourceRectangle = selectedButtonPreset.StatePreset[ButtonAnimationState.Activated];

            radioButtonElementList.Where((x) => x != rbE).ToList().ForEach((x) =>
            {
                x.Sprite.SourceRectangle = selectedButtonPreset.StatePreset[ButtonAnimationState.Normal];
            });

            OnClick?.Invoke(radioButtonElementList.IndexOf(rbE));
        }

        public void UpdateButtonPosition(int index, Vector2 newPosition)
        {
            RadioButtonElement rbE = radioButtonElementList[index];

            //Sprite
            rbE.Sprite.PositionOffset = newPosition;

            //SpriteText
            Vector2 textSize = rbE.SpriteText.MeasureSize;
            rbE.SpriteText.PositionOffset = rbE.Sprite.PositionOffset + new Vector2(rbE.Sprite.Pivot.X, 0) + attatchedTextOffset - new Vector2(0, (int)(textSize.Y / 2));

            //TransparentButton - Calculations
            /*
             * spriteStartPosX
             * /             \
             *  (O) TextA     ( ) TextB
             *           \             /
             *             textEndPosX
             */
            float textEndPosX = rbE.SpriteText.PositionOffset.X + textSize.X;
            float spriteStartPosX = rbE.Sprite.PositionOffset.X - rbE.Sprite.Pivot.X;
            Vector2 boxSize = new Vector2(textEndPosX - spriteStartPosX, textSize.Y);

            rbE.TransparentButton.ButtonOffset = new Vector2((int)((spriteStartPosX + textEndPosX) / 2), (int)rbE.Sprite.PositionOffset.Y);
            rbE.TransparentButton.ButtonSprite.SourceRectangle = new Rectangle(0, 0, (int)boxSize.X, (int)boxSize.Y);
            rbE.TransparentButton.ButtonSprite.Pivot = new Vector2((int)boxSize.X, (int)boxSize.Y) / 2;
        }

        public void UpdateAttatchmentPosition()
        {
            radioButtonElementList.ForEach((x) => x.UpdateAttatchmentPosition());
        }

        public void Update()
        {
            radioButtonElementList.ForEach((x) =>
            {
                x.Update();
            });

            UpdateAttatchmentPosition();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            radioButtonElementList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }

        public void RealocateElements(Vector2 delta)
        {
            radioButtonElementList.ForEach((x) => x.RealocateElements(delta));
        }
    }
}