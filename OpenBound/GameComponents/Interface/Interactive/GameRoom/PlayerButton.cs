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

namespace OpenBound.GameComponents.Interface.Interactive.GameRoom
{
    public class PlayerButton : Button
    {
        public Flipbook PlayerStatus;
        public Flipbook PlayerStatusText;

        //
        public Mobile Mobile;
        public Nameplate Nameplate;

        public List<Flipbook> flipbookList;
        public List<Sprite> spriteList;

        public Player Player { get; private set; }

        protected static Dictionary<PlayerTeam, Dictionary<PlayerRoomStatus, AnimationInstance>> PlayerStatusPreset =
            new Dictionary<PlayerTeam, Dictionary<PlayerRoomStatus, AnimationInstance>>
            {
                //Team Red
                {
                    PlayerTeam.Red,
                    new Dictionary<PlayerRoomStatus, AnimationInstance>()
                    {
                        {
                            PlayerRoomStatus.NotReady,
                            new AnimationInstance() { StartingFrame = 0, EndingFrame = 0, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Ready,
                            new AnimationInstance() { StartingFrame = 1, EndingFrame = 1, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Master,
                            new AnimationInstance() { StartingFrame = 1, EndingFrame = 1, TimePerFrame = 1f }
                        },
                    }
                },
                //Team Blue
                {
                    PlayerTeam.Blue,
                    new Dictionary<PlayerRoomStatus, AnimationInstance>()
                    {
                        {
                            PlayerRoomStatus.NotReady,
                            new AnimationInstance() { StartingFrame = 2, EndingFrame = 2, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Ready,
                            new AnimationInstance() { StartingFrame = 3, EndingFrame = 3, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Master,
                            new AnimationInstance() { StartingFrame = 3, EndingFrame = 3, TimePerFrame = 1f }
                        }
                    }
                }
            };

        protected static Dictionary<PlayerTeam, Dictionary<PlayerRoomStatus, AnimationInstance>> PlayerStatusTextPreset =
            new Dictionary<PlayerTeam, Dictionary<PlayerRoomStatus, AnimationInstance>>()
            {
                //Team Red
                {
                    PlayerTeam.Red,
                    new Dictionary<PlayerRoomStatus, AnimationInstance>()
                    {
                        {
                            PlayerRoomStatus.NotReady,
                            new AnimationInstance() { StartingFrame = 1, EndingFrame = 1, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Ready,
                            new AnimationInstance() { StartingFrame = 1, EndingFrame = 1, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Master,
                            new AnimationInstance() { StartingFrame = 0, EndingFrame = 0, TimePerFrame = 1f }
                        },
                    }
                },
                //Team Blue
                {
                    PlayerTeam.Blue,
                    new Dictionary<PlayerRoomStatus, AnimationInstance>()
                    {
                        {
                            PlayerRoomStatus.NotReady,
                            new AnimationInstance() { StartingFrame = 2, EndingFrame = 2, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Ready,
                            new AnimationInstance() { StartingFrame = 2, EndingFrame = 2, TimePerFrame = 1f }
                        },
                        {
                            PlayerRoomStatus.Master,
                            new AnimationInstance() { StartingFrame = 0, EndingFrame = 0, TimePerFrame = 1f }
                        }
                    }
                }
            };

        public PlayerButton(Player Player, Action<object> OnClick, Vector2 ButtonOffset)
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

            PlayerStatus = new Flipbook(
                ButtonOffset + new Vector2(82 * sidePositionFactor, 0),
                new Vector2((296 / 4) / 2, 76 / 2),
                74, 76, "Interface/StaticButtons/GameRoom/Player/StatusMarker",
                PlayerStatusPreset[Player.PlayerTeam][Player.PlayerRoomStatus],
                DepthParameter.InterfaceButtonIcon);
            flipbookList.Add(PlayerStatus);

            PlayerStatusText = new Flipbook(
                ButtonOffset + new Vector2(78 * sidePositionFactor, 28),
                new Vector2((150 / 3) / 2, 16 / 2),
                50, 16, "Interface/StaticButtons/GameRoom/Player/StatusText",
                PlayerStatusTextPreset[Player.PlayerTeam][Player.PlayerRoomStatus],
                DepthParameter.InterfaceButtonText);
            flipbookList.Add(PlayerStatusText);

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

        public void ChangePosition(Vector2 newPostion)
        {
            if (newPostion == ButtonOffset) return;

            Vector2 diff = newPostion - ButtonOffset;

            flipbookList.ForEach((x) => x.Position += diff);
            spriteList.ForEach((x) => x.Position += diff);
            Mobile.MobileFlipbook.Position += diff;

            Nameplate.Update(Nameplate.Position + diff);

            ButtonOffset = newPostion;
        }

        public override void Update()
        {
            base.Update();
            Mobile.Rider.Update();
            UpdatePlayerStatus();
        }

        public void UpdatePlayerStatus()
        {
            if (Player.PlayerRoomStatus == PlayerRoomStatus.NotReady)
                PlayerStatusText.Color = Color.Transparent;
            else
                PlayerStatusText.Color = Color.White;
        }

        public override void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            base.Draw(GameTime, SpriteBatch);

            flipbookList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
            spriteList.ForEach((x) => x.Draw(GameTime, SpriteBatch));

            Mobile.Draw(GameTime, SpriteBatch);
            Nameplate.Draw(SpriteBatch);
        }
    }
}
