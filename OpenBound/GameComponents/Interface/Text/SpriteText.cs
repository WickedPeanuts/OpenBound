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
using OpenBound.Extension;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Asset;
using OpenBound_Network_Object_Library.Entity.Text;
using System.Text;
using OpenBound_Network_Object_Library.Models;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Text
{
    public class SpriteText
    {
        public SpriteFont SpriteFont { get; private set; }
        public FontTextType FontTextType { get; private set; }
        public FontFamilyMetadata FontFamilyMetadata { get; private set; }

        //public int FontSize { get; set; }
        private Vector2 position;
        public Vector2 Position
        {
            get => position;
            //Prevent font of blurrying when it is in the middle of a coordinate
            set => position = value.ToIntegerDomain();
        }


        private Vector2 positionOffset;
        public Vector2 PositionOffset {
            get => positionOffset;
            set => positionOffset = value.ToIntegerDomain();
        }

        private Vector2 origin;
        public Vector2 Origin
        {
            get => origin;
            //Prevent font of blurrying when it is in the middle of a coordinate
            set => origin = new Vector2((int)value.X, (int)value.Y);
        }
        public Vector2 Scale { get; set; }

        public Color BaseColor { get; set; }
        public Color Color { get; set; }

        public Color OutlineBaseColor { get; set; }
        public Color OutlineColor { get; set; }

        private string text;
        public string Text
        {
            get { return text; }
            set { UpdateText(value); }
        }
        public float Rotation { get; set; }
        public float LayerDepth { get; set; }

        private Alignment alignment;
        public Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;
                UpdateText(Text);
            }
        }

        public SpriteEffects SpriteEffects { get; set; }

        public Vector2 MeasureSize { get => SpriteFont.MeasureString(Text); }

        private bool hasOutline;


        public SpriteText(FontTextType fontTextType, string text, Color color, Alignment alignment, float layerDepth, Vector2 position = default, Color outlineColor = default)
        {
            (SpriteFont, FontTextType) = AssetHandler.Instance.RequestFont(fontTextType, text);
            Position = position;

            //this.FontSize = FontSize;
            BaseColor = Color = color;
            this.alignment = alignment;
            LayerDepth = layerDepth;

            hasOutline = false;

            FontFamilyMetadata = FontFamilyMetadata.GetMetadata(FontTextType);

            if (outlineColor == default)
                outlineColor = Color.Black;

            Scale = new Vector2(1, 1);
            Rotation = 0f;
            SpriteEffects = SpriteEffects.None;

            if (outlineColor != default)
            {
                hasOutline = true;
                OutlineBaseColor = OutlineColor = outlineColor;
            }

            this.text = "";

            UpdateText(text);
        }

        public void SetTransparency(float transparency)
        {
            Color = BaseColor * transparency;
            OutlineColor = OutlineBaseColor * transparency;
        }

        public string GenerateTextWithSupportedCharacters(string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char c in text)
            {
                if (c == '\n' || SpriteFont.Characters.Contains(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public void UpdateAttatchedPosition()
        {
            Position = PositionOffset - GameScene.Camera.CameraOffset;
        }

        public Vector2 MeasureSubstring(string Text)
        {
            return SpriteFont.MeasureString(Text);
        }

        private void UpdateText(string Text)
        {
            text = GenerateTextWithSupportedCharacters(Text);

            //Fixing the origin
            Vector2 size = SpriteFont.MeasureString(text);

            if (Alignment == Alignment.Left)
                Origin = Vector2.Zero;
            else if (Alignment == Alignment.Center)
                Origin = new Vector2(size.X * 0.5f, 0);
            else
                Origin = new Vector2(size.X, 0);
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            SpriteBatch.DrawString(SpriteFont, Text, Position, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth);

            if (hasOutline)
            {
                //(-1,  1) ( 0,  1) ( 1,  1)
                //(-1,  0) ( 0,  0) ( 1,  0)
                //(-1, -1) ( 0, -1) ( 1, -1)
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(-1, 1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(0, 1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(1, 1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);

                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(-1, 0), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(1, 0), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);

                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(-1, -1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(0, -1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
                SpriteBatch.DrawString(SpriteFont, Text, Position + new Vector2(1, -1), OutlineColor, Rotation, Origin, Scale, SpriteEffects, LayerDepth - 0.001f);
            }
        }

        public static List<SpriteText> CreatePlayerTextBalloonMessage(Player player, PlayerMessage playerMessage, int maxWidth, float layerDepth)
        {
            string text = playerMessage.Text;

            //Append a space on every word bigger than maxWidth
            for (int i = 0, j = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    j = 0;
                }
                else
                {
                    j++;
                    if (j > maxWidth)
                    {
                        j = 0;
                        text = text.Insert(i, " ");
                    }
                }
            }

            List<string> strList = new List<string>();
            int fIndex = 0;

            //Whenever a space is found, save the smallest possible sentence on the list
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ' || text.Length <= maxWidth || i + 1 == text.Length)
                {
                    if (i >= maxWidth)
                    {
                        int cutIndex = fIndex == 0 ? i : fIndex;
                        string trimText = text.Substring(0, cutIndex).Trim();

                        if (trimText.Length > 0)
                            strList.Add(trimText);

                        text = text.Substring(cutIndex).Trim();
                        fIndex = 0;
                        i = 0;
                    }
                    else
                    {
                        fIndex = i;
                    }
                }
            }

            //Save the remaning text into the list
            if (text.Length > 0)
                strList.Add(text);

            //Select the color
            Color color = playerMessage.PlayerTeam switch
            {
                PlayerTeam.Red => Common.Parameter.TextColorTeamRed,
                PlayerTeam.Blue => Common.Parameter.TextColorTeamBlue,
                _ => Message.TextToColor(player.Nickname),
            };

            List<SpriteText> sTextList = new List<SpriteText>();
            Color outlineColor = Color.Black;

            foreach (string s in strList)
                sTextList.Add(new SpriteText(FontTextType.Consolas10Family, s, color, Alignment.Center, layerDepth, outlineColor: outlineColor));

            return sTextList;
        }
    }
}
