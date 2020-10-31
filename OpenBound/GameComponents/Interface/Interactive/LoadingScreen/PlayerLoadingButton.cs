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
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;

namespace OpenBound.GameComponents.Interface.Interactive.LoadingScreen
{
    public class PlayerLoadingButton : Button
    {
        public Sprite PlayerStatus;

        public Mobile Mobile;
        public Nameplate Nameplate;

        private SpriteText loadingPercentage;
        private float percentageDone;

        public List<Flipbook> flipbookList;
        public List<Sprite> spriteList;

        public Player Player { get; private set; }

        public bool IsLoadingComplete { get => percentageDone >= 100; }

        protected static Dictionary<PlayerTeam, Rectangle> PlayerStatusPreset =
            new Dictionary<PlayerTeam, Rectangle>
            {
                //Team Red
                {
                    PlayerTeam.Red,
                    new Rectangle(0, 0, 70, 72)
                },
                //Team Blue
                {
                    PlayerTeam.Blue,
                    new Rectangle(70 * 4, 0, 70, 72)
                }
            };

        public PlayerLoadingButton(Player Player, Action<object> OnClick, Vector2 ButtonOffset)
            : base(ButtonType.PlayerButton, DepthParameter.InterfaceButton, OnClick, ButtonOffset)
        {
            //Hide button placeholder
            spriteList = new List<Sprite>();
            flipbookList = new List<Flipbook>();

            ButtonSprite.Color = Color.Transparent;
            this.Player = Player;

            //Since the button elements dont update, the screencenter
            //must be added in order to create the right position
            //on the elements
            ButtonOffset += Parameter.ScreenCenter;

            int sidePositionFactor = (Player.PlayerTeam == PlayerTeam.Red) ? 1 : -1;

            PlayerStatus = new Sprite("Interface/StaticButtons/LoadingScreen/LoadingProgressStatusMarker",
                ButtonOffset + new Vector2(82 * sidePositionFactor, 0),
                DepthParameter.InterfaceButtonIcon, PlayerStatusPreset[Player.PlayerTeam])
            {
                Pivot = new Vector2(70, 72) / 2,
            };

            spriteList.Add(PlayerStatus);

            loadingPercentage = new SpriteText(FontTextType.Consolas10, "0%",
                Color.White, Alignment.Center, DepthParameter.InterfaceButtonText,
                ButtonOffset + new Vector2(78 * sidePositionFactor, 24), Color.Black);

            spriteList.Add(new Sprite("Interface/StaticButtons/GameRoom/Player/Shadow",
                ButtonOffset + new Vector2(-20 * sidePositionFactor, 33),
                DepthParameter.Mobile - 0.01f));

            Mobile = ActorBuilder.BuildMobile(Player.PrimaryMobile, Player, ButtonOffset + new Vector2(-20 * sidePositionFactor, 7), false);
            Mobile.Rider.Update();

            if (Player.PlayerTeam == PlayerTeam.Red)
            {
                Mobile.Flip();
            }

            Nameplate = new Nameplate(Player, Alignment.Left, ButtonOffset - new Vector2(100, 47));

            Mobile.MobileFlipbook.JumpToRandomAnimationFrame();
        }

        public override void Update()
        {
            base.Update();
            Mobile.Rider.Update();
        }

        public void ChangePosition(Vector2 newPostion)
        {
            if (newPostion == ButtonOffset) return;

            Vector2 diff = newPostion - ButtonOffset;

            flipbookList.ForEach((x) => x.Position += diff);
            spriteList.ForEach((x) => x.Position += diff);
            Mobile.MobileFlipbook.Position += diff;

            Nameplate.Update(Nameplate.Position + diff);

            loadingPercentage.Position += diff;

            ButtonOffset = newPostion;
        }

        public void UpdatePercentage(int percentage)
        {
            percentageDone = percentage;
            PlayerStatus.SourceRectangle.X =
                PlayerStatusPreset[Player.PlayerTeam].X +
                (int)(percentageDone / 33) * 70;

            loadingPercentage.Text = $"{(int)percentageDone}%";
            percentageDone = Math.Min(100, percentageDone + (float)(0.8f * Parameter.Random.NextDouble()));
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            base.Draw(GameTime, SpriteBatch);

            flipbookList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
            spriteList.ForEach((x) => x.Draw(GameTime, SpriteBatch));

            Mobile.Draw(GameTime, SpriteBatch);
            Nameplate.Draw(SpriteBatch);
            loadingPercentage.Draw(SpriteBatch);
        }
    }
}
