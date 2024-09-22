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
using OpenBound.GameComponents.Debug;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Text
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class CompositeSpriteText
    {
        private List<List<SpriteText>> spriteTextMatrix;
        private Vector2 elementOffset;
        private Vector2 position;
        private Vector2 positionOffset;
        private Alignment alignment;

        #region Properties
        public List<List<SpriteText>> SpriteTextMatrix
        {
            get => spriteTextMatrix;
            set
            {
                spriteTextMatrix = value;
                RecalculatePosition();
            }
        }

        public Vector2 ElementOffset
        {
            get => elementOffset;
            set
            {
                elementOffset = value.ToIntegerDomain();
                RecalculatePosition();
            }
        }

        public Vector2 Position
        {
            get => position;
            set
            {
                position = value.ToIntegerDomain();
                RecalculatePosition();
            }
        }

        public Vector2 PositionOffset
        {
            get => positionOffset;
            set
            {
                positionOffset = value.ToIntegerDomain();
                UpdateAttatchedPosition();
                RecalculatePosition();
            }
        }

        public Vector2 ElementDimensions
        {
            get
            {
                Vector2 sum = Vector2.Zero;

                int sumX = 0, sumY = 0;
                int tmpSumX = 0, tmpSumY = 0;

                foreach (List<SpriteText> spriteTextList in spriteTextMatrix)
                {
                    foreach (SpriteText spriteText in spriteTextList)
                    {
                        Vector2 sentenceSize = spriteText.MeasureSize;

                        tmpSumX += (int)sentenceSize.X;
                        tmpSumY = (int)Math.Max(tmpSumY, sentenceSize.Y);
                    }

                    sumX += Math.Max(sumX, tmpSumX);
                    sumY += tmpSumY;
                }

                sumX += (int)(spriteTextMatrix.Max(x => x.Count) * elementOffset.X);
                sumY += (int)(spriteTextMatrix.Count * elementOffset.Y);

                return new Vector2(sumX, sumY);
            }
        }

        public float Transparency { set => SpriteTextMatrix.ForEach((x) => x.ForEach((y) => y.SetTransparency(value))); }
        #endregion

        public static List<CompositeSpriteText> CreateCustomMessage(CustomMessage customMessage, int maxWidth, float layerDepth)
        {
            List<CompositeSpriteText> cstList = new List<CompositeSpriteText>();

            int i = 0;
            while(i < customMessage.Text.Length)
            {
                List<SpriteText> line = new List<SpriteText>();

                SpriteText st = new SpriteText(customMessage.FontTextType, "", new Color(customMessage.TextColor), Alignment.Left, layerDepth, outlineColor: new Color(customMessage.TextBorderColor));

                line.Add(st);

                for (; i < customMessage.Text.Length && st.MeasureSubstring(st.GenerateTextWithSupportedCharacters(st.Text + customMessage.Text[i])).X < maxWidth; i++)
                {
                    st.Text += customMessage.Text[i];
                }

                if (st.Text.Length == 0) continue;

                cstList.Add(CreateCompositeSpriteText(line, Orientation.Horizontal, Alignment.Left, default));
            }

            return cstList;
        }

        /// <summary>
        /// This method does not support automatic line breaks
        /// </summary>
        /// <param name="customMessageList"></param>
        /// <param name="layerDepth"></param>
        /// <returns></returns>
        public static List<CompositeSpriteText> CreateCustomMessage(List<CustomMessage> customMessageList, float layerDepth)
        {
            List<CompositeSpriteText> cstList = new List<CompositeSpriteText>();

            List<SpriteText> line = new List<SpriteText>();
            
            foreach(CustomMessage cm in customMessageList)
            {
                cm.AppendTokenToText();
                line.Add(new SpriteText(cm.FontTextType, cm.Text, new Color(cm.TextColor), Alignment.Left, layerDepth, outlineColor: new Color(cm.TextBorderColor)));
            }

            cstList.Add(CreateCompositeSpriteText(line, Orientation.Horizontal, Alignment.Left, default));

            return cstList;
        }

        public static List<CompositeSpriteText> CreateChatMessage(PlayerMessage playerMessage, int maxWidth, float layerDepth)
        {
            List<CompositeSpriteText> cstList = new List<CompositeSpriteText>();

            Color color, outlineColor = Color.Black;

            if (playerMessage.PlayerTeam == null)
                color = Color.White;
            else if (playerMessage.PlayerTeam == PlayerTeam.Red)
                color = Parameter.TextColorTeamRed;
            else
                color = Parameter.TextColorTeamBlue;

            //Load the first line with the player nickname
            List<SpriteText> line = new List<SpriteText>() {
                new SpriteText(FontTextType.NotoSans10Family, "[", color, Alignment.Left, layerDepth, outlineColor: outlineColor),
                new SpriteText(FontTextType.NotoSans10Family, playerMessage.Player.Nickname, Message.TextToColor(playerMessage.Player.Nickname), Alignment.Left, layerDepth, outlineColor: outlineColor),
                new SpriteText(FontTextType.NotoSans10Family, "]: ", color, Alignment.Left, layerDepth, outlineColor: outlineColor),
            };

            SpriteText st = line.Last();
            int i = 0;
            while (i < playerMessage.Text.Length)
            {
                List<SpriteText> lineProjection = line.Except(new List<SpriteText>() { st }).ToList();

                for (; i < playerMessage.Text.Length && lineProjection.Sum((x) => x.MeasureSize.X) + st.MeasureSubstring(st.GenerateTextWithSupportedCharacters(st.Text + playerMessage.Text[i])).X < maxWidth; i++)
                {
                    st.Text += playerMessage.Text[i];
                }

                cstList.Add(CreateCompositeSpriteText(line, Orientation.Horizontal, Alignment.Left, default));

                line = new List<SpriteText>();
                st = new SpriteText(FontTextType.NotoSans10Family, "", color, Alignment.Left, layerDepth, outlineColor: outlineColor);
                line.Add(st);
            }

            return cstList;
        }

        public static CompositeSpriteText CreateCompositeSpriteText(List<List<SpriteText>> spriteTextMatrix, Alignment alignment, Vector2 position, Vector2 elementOffset)
        {
            return new CompositeSpriteText(spriteTextMatrix, alignment, position, elementOffset);
        }

        public static CompositeSpriteText CreateCompositeSpriteText(List<SpriteText> spriteTextList, Orientation orientation, Alignment alignment, Vector2 position, float elementOffset = 0)
        {
            if (orientation == Orientation.Horizontal)
            {
                return new CompositeSpriteText(new List<List<SpriteText>>() { spriteTextList }, alignment, position, new Vector2(elementOffset, 0));
            }
            else
            {
                List<List<SpriteText>> stList = new List<List<SpriteText>>();
                spriteTextList.ForEach((x) => stList.Add(new List<SpriteText>() { x }));
                return new CompositeSpriteText(stList, alignment, position, new Vector2(0, elementOffset));
            }
        }

        private CompositeSpriteText(List<List<SpriteText>> spriteTextMatrix, Alignment alignment, Vector2 position, Vector2 elementOffset)
        {
            this.elementOffset = elementOffset;
            this.position = position;
            this.spriteTextMatrix = spriteTextMatrix;
            this.alignment = alignment;

            spriteTextMatrix.ForEach((x) => x.ForEach((y) => y.Alignment = alignment == Alignment.Center ? Alignment.Left : alignment));

            RecalculatePosition();

#if DEBUG
            DebugCrosshair dc = new DebugCrosshair(Color.Blue);
            dc.Update(this.position);
            DebugHandler.Instance.Add(dc);
#endif
        }

        public void UpdateAttatchedPosition()
        {
            position = positionOffset;
            spriteTextMatrix.ForEach((x) => x.ForEach((y) => y.UpdateAttatchedPosition()));
        }

        public void ReplaceTextColor(Color from, Color to)
        {
            spriteTextMatrix.ForEach((x) => x.ForEach((y) =>
            {
                if (from == y.Color) y.Color = to;
            }));
        }

        public void ResetTextColor()
        {
            spriteTextMatrix.ForEach((x) => x.ForEach((y) => y.Color = y.BaseColor));
        }

        private void RecalculatePosition()
        {
            Vector2 newPosition = Position;

            if (alignment == Alignment.Center) newPosition -= new Vector2(ElementDimensions.X, 0) / 2;

            float offsetY = 0;

            foreach (List<SpriteText> spriteList in spriteTextMatrix)
            {
                float offsetX = 0;
                float maxY = 0;

                foreach (SpriteText spriteText in spriteList)
                {
                    Vector2 elemSize = spriteText.MeasureSize;

                    spriteText.Position = spriteText.PositionOffset = newPosition + new Vector2(offsetX, offsetY)
                        + spriteText.FontFamilyMetadata.ElementHeightOffset;
                    offsetX += elemSize.X + elementOffset.X;
                    maxY = Math.Max(maxY, elemSize.Y);
                }

                offsetY += maxY + elementOffset.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteTextMatrix.ForEach((x) => x.ForEach((y) => y.Draw(spriteBatch)));
        }
    }
}
