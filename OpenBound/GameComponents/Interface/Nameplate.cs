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
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface
{
    public class Nameplate
    {
        Sprite rankIndicator;
        CompositeSpriteText compositeSpriteText;

        public Vector2 Position { get; private set; }
        public Vector2 PositionOffset;
        public Alignment Alignment { get; set; }

        //references
        public Mobile Mobile;

        private bool shouldRender;

        public Color PlayerColor { get; private set; }
        public Color GuildColor { get; private set; }

        //Reference
        public readonly Player Player;

        /// <summary>
        /// Must be used only on menu components. For in-game nameplate you must specify the
        /// attatched mobile on the other constructor
        /// </summary>
        /// <param name="player"></param>
        /// <param name="alignment"></param>
        /// <param name="position"></param>
        public Nameplate(Player player, Alignment alignment, Vector2 position, bool showGuild = true, float layerDepth = DepthParameter.Nameplate)
        {
            Player = player;

            Alignment = alignment;
            Position = PositionOffset = position;

            rankIndicator = IconBuilder.Instance.BuildPlayerRank(player.PlayerRank, layerDepth);

            List<SpriteText> spTL = new List<SpriteText>();

            if (showGuild && player.Guild != null)
            {
                spTL.Add(new SpriteText(FontTextType.NotoSans10Family, "[",
                    Color.White, alignment, layerDepth,
                    outlineColor: Color.Black));
                spTL.Add(new SpriteText(FontTextType.NotoSans10Family, player.Guild.Tag,
                    Parameter.NameplateGuildColor, alignment, layerDepth,
                    outlineColor: Parameter.NameplateGuildOutlineColor));
                spTL.Add(new SpriteText(FontTextType.NotoSans10Family, "] ",
                    Color.White, alignment, layerDepth,
                    outlineColor: Color.Black));
            }

            PlayerColor = Message.TextToColor(player.Nickname);

            spTL.Add(new SpriteText(FontTextType.NotoSans10Family, player.Nickname,
                PlayerColor, alignment, layerDepth,
                outlineColor: Color.Black));

            compositeSpriteText = CompositeSpriteText.CreateCompositeSpriteText(spTL, Orientation.Horizontal, Alignment.Left, position, 0);

            shouldRender = true;

            Update(position);
        }

        /// <summary>
        /// This constructor is made for nameplates used on the in-game scene
        /// </summary>
        /// <param name="mobile"></param>
        public Nameplate(Mobile mobile)
        {
            Player = mobile.Owner;

            Mobile = mobile;
            Alignment = Alignment.Center;

            Player player = mobile.Owner;

            rankIndicator = IconBuilder.Instance.BuildPlayerRank(player.PlayerRank, DepthParameter.Nameplate);

            List<SpriteText> spTL = new List<SpriteText>();

            Color textColor = (player.PlayerTeam == PlayerTeam.Red) ?
                Parameter.TextColorTeamRed :
                Parameter.TextColorTeamBlue;

            if (player.Guild != null)
            {
                spTL.Add(new SpriteText(FontTextType.NotoSans10Family, $"[{player.Guild.Tag}] ",
                    textColor, Alignment, DepthParameter.Nameplate,
                    outlineColor: Color.Black));
            }

            spTL.Add(new SpriteText(FontTextType.NotoSans10Family, player.Nickname,
                textColor, Alignment, DepthParameter.Nameplate,
                outlineColor: Color.Black));

            compositeSpriteText = CompositeSpriteText.CreateCompositeSpriteText(spTL, Orientation.Horizontal, Alignment.Left, default, 0);

            shouldRender = true;

            Update();
        }

        public void ResetTextColor()
        {
            compositeSpriteText.ResetTextColor();
        }

        public void ReplaceTextColor(Color from, Color to)
        {
            compositeSpriteText.ReplaceTextColor(from, to);
        }

        public Vector2 ElementDimensions()
        {
            Vector2 textSize = compositeSpriteText.ElementDimensions;
            return new Vector2(rankIndicator.SourceRectangle.Width + textSize.X, MathHelper.Max(rankIndicator.SourceRectangle.Height, textSize.Y));
        }

        public void UpdateAttatchmentPosition()
        {
            Update(PositionOffset - GameScene.Camera.CameraOffset);
        }

        public void Update()
        {
            Update(Mobile.MobileFlipbook.Position + new Vector2(0, 70));
        }

        public void Update(Vector2 NewPosition)
        {
            if (Alignment == Alignment.Left)
            {
                //Rank Indicator (Image)
                rankIndicator.Position = NewPosition + new Vector2(rankIndicator.Pivot.X, 0);
                compositeSpriteText.Position = rankIndicator.Position +
                    new Vector2(rankIndicator.Pivot.X + 2, -compositeSpriteText.ElementDimensions.Y / 2f);
            }
            else if (Alignment == Alignment.Center)
            {
                Vector2 completeSentence = (compositeSpriteText.ElementDimensions + rankIndicator.Pivot) / 2;
                rankIndicator.Position = NewPosition - completeSentence;
                compositeSpriteText.Position = rankIndicator.Position +
                    new Vector2(rankIndicator.Pivot.X + 2, -compositeSpriteText.ElementDimensions.Y / 2f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!shouldRender) return;

            rankIndicator.Draw(null, spriteBatch);
            compositeSpriteText.Draw(spriteBatch);
        }

        public void HideElement()
        {
            shouldRender = false;
        }

        public void ShowElement()
        {
            shouldRender = true;
        }

        public override int GetHashCode()
        {
            return Player.ID;
        }
    }
}
