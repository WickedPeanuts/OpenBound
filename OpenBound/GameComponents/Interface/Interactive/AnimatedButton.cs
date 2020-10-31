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
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Interface.Text;
using System;
using System.Collections.Generic;
using OpenBound.Extension;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public enum AnimatedButtonType
    {
        //Menu Flow
        ExitDoor, AvatarShop,

        //Popups
        Buddy, CashCharge, MuteList,
        Event, MyInfo, MatchSettings, Options, ReportPlayer,

        //Navigation
        LeftArrow, RightArrow,

        //GameList/Room Filtering
        ViewAll, ViewFriend, ViewWaiting,

        //GameList/Room Options
        Create, GoTo, QuickJoin,

        //GameList/GameModes
        Jewel, Score, Solo, Tag,
        Ready,
        ChangeMobile,
        ChangeItem,
        ChangeTeam,

        //AvatarShop/Avatar
        Buy, Gift, Try,

        //AvatarShop/Avatar Filtering
        Hat, Body, Goggles, Flag, ExItem, Pet, Necklace, Ring,
    }

    public struct AnimatedButtonPreset
    {
        public string SpritePath;
        public Dictionary<ButtonAnimationState, AnimationInstance> StatePreset;
        public int SpriteWidth, SpriteHeight;
        public Vector2 Pivot;
        public Vector2 CollisionRectangleOffset;
    }

    public class AnimatedButton
    {
        protected static Dictionary<AnimatedButtonType, AnimatedButtonPreset> ButtonPresets = new Dictionary<AnimatedButtonType, AnimatedButtonPreset>()
        {
            #region MenuFlow
            {
                AnimatedButtonType.ExitDoor,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/Exit",
                    SpriteWidth = 1320/20, SpriteHeight = 132/2,
                    Pivot = new Vector2(27, 31), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 0,  EndingFrame = 0,  TimePerFrame = 0.1f } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 0,  EndingFrame = 22, TimePerFrame = 1/23f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 13, EndingFrame = 38, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 39, EndingFrame = 39, TimePerFrame = 0.1f } },
                    },
                }
            },
            {
                AnimatedButtonType.AvatarShop,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/Avatar",
                    SpriteWidth = 1560/20, SpriteHeight = 474/6,
                    Pivot = new Vector2((1560/20)/2, (474/6)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 080, EndingFrame = 099, TimePerFrame = 1/20f } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 031, TimePerFrame = 1/31f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 032, EndingFrame = 079, TimePerFrame = 1/48f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 101, EndingFrame = 101, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.CashCharge,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/CashCharge",
                    SpriteWidth = 1320/20, SpriteHeight = 132/2,
                    Pivot = new Vector2(26, (132/2)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 011, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 012, EndingFrame = 028, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 029, EndingFrame = 029, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region Popups
            {
                AnimatedButtonType.Buddy,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/Buddy",
                    SpriteWidth = 1320/20, SpriteHeight = 198/3,
                    Pivot = new Vector2((1320/20)/2, (198/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 019, EndingFrame = 019, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/20f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 018, EndingFrame = 056, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 057, EndingFrame = 057, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ReportPlayer,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/ReportPlayer",
                    SpriteWidth = 1320/20, SpriteHeight = 198/3,
                    Pivot = new Vector2((1320/20)/2, (198/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 014, TimePerFrame = 1/20f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 015, EndingFrame = 044, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 046, EndingFrame = 046, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.MatchSettings,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/Options",
                    SpriteWidth = 1480/20, SpriteHeight = 288/4,
                    Pivot = new Vector2((1480/20)/2, (288/4)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 030, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 031, EndingFrame = 060, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 061, EndingFrame = 061, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Options,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/Options",
                    SpriteWidth = 1480/20, SpriteHeight = 288/4,
                    Pivot = new Vector2((1480/20)/2, (288/4)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 030, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 031, EndingFrame = 060, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 061, EndingFrame = 061, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.MyInfo,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/MyInfo",
                    SpriteWidth = 1400/20, SpriteHeight = 210/3,
                    Pivot = new Vector2((1400/20)/2, 42), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 020, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 021, EndingFrame = 040, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 041, EndingFrame = 041, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.MuteList,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/MuteList",
                    SpriteWidth = 1320/20, SpriteHeight = 264/4,
                    Pivot = new Vector2((1320/20)/2, (264/4)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 060, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 061, EndingFrame = 061, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region RoomFiltersGamemode
            {
                AnimatedButtonType.Solo,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/GameModes/Solo",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 050, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 051, EndingFrame = 051, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Tag,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/GameModes/Tag",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 050, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 051, EndingFrame = 051, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Score,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/GameModes/Score",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 050, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 051, EndingFrame = 051, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Jewel,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/GameModes/Jewel",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 050, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 051, EndingFrame = 051, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region RoomFilterStatus
            {
                AnimatedButtonType.ViewAll,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/RoomFilter/All",
                    SpriteWidth = 920/20, SpriteHeight = 168/4,
                    Pivot = new Vector2((920/20)/2, (168/4)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 029, TimePerFrame = 1/18f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 030, EndingFrame = 060, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 061, EndingFrame = 061, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ViewWaiting,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/RoomFilter/Wait",
                    SpriteWidth = 920/20, SpriteHeight = 168/4,
                    Pivot = new Vector2((920/20)/2, (168/4)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 032, TimePerFrame = 1/18f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 031, EndingFrame = 072, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 073, EndingFrame = 073, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ViewFriend,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/RoomFilter/Friend",
                    SpriteWidth = 920/20, SpriteHeight = 168/4,
                    Pivot = new Vector2((920/20)/2, (168/4)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 012, TimePerFrame = 1/18f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 013, EndingFrame = 063, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 064, EndingFrame = 064, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region RoomNavitagion
            {
                AnimatedButtonType.LeftArrow,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/LeftArrow",
                    SpriteWidth = 920/20, SpriteHeight = 84/2,
                    Pivot = new Vector2((920/20)/2, (84/2)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 019, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 030, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 031, EndingFrame = 031, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.RightArrow,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/RightArrow",
                    SpriteWidth = 920/20, SpriteHeight = 84/2,
                    Pivot = new Vector2((920/20)/2, (84/2)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 019, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 030, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 031, EndingFrame = 031, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region RoomOptions
            {
                AnimatedButtonType.GoTo,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/Room/GoTo",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 037, EndingFrame = 054, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 000, EndingFrame = 035, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 036, EndingFrame = 036, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Create,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/Room/Create",
                    SpriteWidth = 1000/20, SpriteHeight = 150/3,
                    Pivot = new Vector2((1000/20)/2, (150/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 013, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 014, EndingFrame = 050, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 052, EndingFrame = 052, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.QuickJoin,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameList/Room/QuickJoin",
                    SpriteWidth = 1400/20, SpriteHeight = 192/3,
                    Pivot = new Vector2((1400/20)/2, (192/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 024, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 025, EndingFrame = 055, TimePerFrame = 1/25f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 056, EndingFrame = 056, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region Room
            {
                AnimatedButtonType.Ready,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameRoom/Ready",
                    SpriteWidth = 1320/20, SpriteHeight = 264/4,
                    Pivot = new Vector2((1320/20)/2, (264/4)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/28f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 042, TimePerFrame = 1/28f, AnimationType = AnimationType.Cycle } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 043, EndingFrame = 043, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ChangeMobile,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameRoom/ChangeMobile",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 052, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 053, EndingFrame = 053, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ChangeItem,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameRoom/ChangeItem",
                    SpriteWidth = 920/20, SpriteHeight = 126/3,
                    Pivot = new Vector2((920/20)/2, (126/3)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 001, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 057, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 058, EndingFrame = 058, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ChangeTeam,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/GameRoom/SwitchTeams",
                    SpriteWidth = 920/20, SpriteHeight = 168/4,
                    Pivot = new Vector2((920/20)/2, (168/4)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 020, EndingFrame = 065, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 066, EndingFrame = 066, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region AvatarShop/Avatar
            {
                AnimatedButtonType.Buy,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Buy",
                    SpriteWidth = 512/8, SpriteHeight = 438/6,
                    Pivot = new Vector2((512/8)/2, (438/6)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 023, TimePerFrame = 1/30f, AnimationType = AnimationType.Cycle } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 026, EndingFrame = 045, TimePerFrame = 1/30f, AnimationType = AnimationType.Cycle } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 046, EndingFrame = 046, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Gift,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Gift",
                    SpriteWidth = 768/12, SpriteHeight = 414/6,
                    Pivot = new Vector2((414/6)/2, (414/6)/2 + 5), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 014, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 031, EndingFrame = 062, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 071, EndingFrame = 071, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Try,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Try",
                    SpriteWidth = 768/12, SpriteHeight = 432/6,
                    Pivot = new Vector2((768/12)/2, (432/6)/2), CollisionRectangleOffset = new Vector2(18, 21),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,   new AnimationInstance() { StartingFrame = 013, EndingFrame = 013, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,   new AnimationInstance() { StartingFrame = 000, EndingFrame = 043, TimePerFrame = 1/30f, AnimationType = AnimationType.Cycle } },
                        { ButtonAnimationState.Clicked,  new AnimationInstance() { StartingFrame = 045, EndingFrame = 070, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled, new AnimationInstance() { StartingFrame = 071, EndingFrame = 071, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
            #region AvatarShop/Avatar Filtering
            {
                AnimatedButtonType.Hat,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Hat",
                    SpriteWidth = 245/5, SpriteHeight = 336/7,
                    Pivot = new Vector2((245/5)/2 - 2, (336/7)/2 - 3), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 021, EndingFrame = 032, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 033, EndingFrame = 033, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 034, EndingFrame = 034, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Body,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Body",
                    SpriteWidth = 539/11, SpriteHeight = 117/3,
                    Pivot = new Vector2((539/11)/2 - 2, (117/3)/2 - 3), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 020, EndingFrame = 030, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 031, EndingFrame = 031, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 032, EndingFrame = 032, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Goggles,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Goggles",
                    SpriteWidth = 931/19, SpriteHeight = 92/2,
                    Pivot = new Vector2((931/19)/2, (92/2)/2 - 5), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 020, EndingFrame = 035, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 036, EndingFrame = 036, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 037, EndingFrame = 037, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Flag,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Flag",
                    SpriteWidth = 672/12, SpriteHeight = 188/4,
                    Pivot = new Vector2((672/12)/2, (188/4)/2), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 014, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 015, EndingFrame = 044, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 045, EndingFrame = 045, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 046, EndingFrame = 046, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.ExItem,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/ExItem",
                    SpriteWidth = 540/10, SpriteHeight = 212/4,
                    Pivot = new Vector2((540/10)/2 + 2, (212/4)/2 - 8), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 022, EndingFrame = 037, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 038, EndingFrame = 038, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 039, EndingFrame = 039, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Pet,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Pet",
                    SpriteWidth = 795/15, SpriteHeight = 147/3,
                    Pivot = new Vector2((795/15)/2 - 4, (147/3)/2 - 4), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 001, EndingFrame = 021, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 022, EndingFrame = 042, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 043, EndingFrame = 043, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 044, EndingFrame = 044, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Necklace,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Necklace",
                    SpriteWidth = 450/9, SpriteHeight = 176/4,
                    Pivot = new Vector2((450/9)/2 - 4, (176/4)/2 - 6), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 021, EndingFrame = 033, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 034, EndingFrame = 034, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 035, EndingFrame = 035, TimePerFrame = 0.1f  } },
                    },
                }
            },
            {
                AnimatedButtonType.Ring,
                new AnimatedButtonPreset() {
                    SpritePath = "Interface/AnimatedButtons/AvatarShop/Ring",
                    SpriteWidth = 660/15, SpriteHeight = 138/3,
                    Pivot = new Vector2((660/15)/2 - 3, (138/3)/2 - 5), CollisionRectangleOffset = new Vector2(15, 15),
                    StatePreset = new Dictionary<ButtonAnimationState, AnimationInstance>()
                    {
                        { ButtonAnimationState.Normal,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 000, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Hoover,    new AnimationInstance() { StartingFrame = 000, EndingFrame = 019, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Clicked,   new AnimationInstance() { StartingFrame = 022, EndingFrame = 040, TimePerFrame = 1/30f } },
                        { ButtonAnimationState.Disabled,  new AnimationInstance() { StartingFrame = 041, EndingFrame = 041, TimePerFrame = 0.1f  } },
                        { ButtonAnimationState.Activated, new AnimationInstance() { StartingFrame = 042, EndingFrame = 042, TimePerFrame = 0.1f  } },
                    },
                }
            },
            #endregion
        };

        AnimatedButtonPreset selectedPreset;

        public Flipbook Flipbook { get; set; }

        public Action<object> OnClick;
        public Action<object> OnDisabled;

        ButtonAnimationState state;

        Rectangle collisionBox;

        bool isBeingPressed;

        SpriteText animatedButtonText;

        public bool IsDisabled { get => state == ButtonAnimationState.Disabled; }
        public bool ShouldUpdate;

#if DEBUG
        List<DebugCrosshair> debugCrosshairList;
#endif

        public AnimatedButton(AnimatedButtonType buttonType, Vector2 position, Action<object> onClick,
            SpriteText buttonText = null)
        {
            selectedPreset = ButtonPresets[buttonType];

            state = ButtonAnimationState.Normal;
            ShouldUpdate = true;

            Flipbook = new Flipbook(position,
                selectedPreset.Pivot,
                selectedPreset.SpriteWidth, selectedPreset.SpriteHeight, selectedPreset.SpritePath,
                selectedPreset.StatePreset[ButtonAnimationState.Normal], DepthParameter.InterfaceButton);

            OnClick = onClick;

            collisionBox = new Rectangle(
                (int)(position.X - selectedPreset.CollisionRectangleOffset.X),
                (int)(position.Y - selectedPreset.CollisionRectangleOffset.Y),
                (int)(selectedPreset.CollisionRectangleOffset.X * 2),
                (int)(selectedPreset.CollisionRectangleOffset.Y * 2));

            isBeingPressed = false;

            if (buttonText != null)
            {
                animatedButtonText = buttonText;
                animatedButtonText.Position = position + new Vector2(0, 15);
            }

#if DEBUG
            debugCrosshairList = new List<DebugCrosshair>()
            {
                new DebugCrosshair(Color.White),new DebugCrosshair(Color.White),
                new DebugCrosshair(Color.White),new DebugCrosshair(Color.White),
                new DebugCrosshair(Color.Red),
            };

            debugCrosshairList[0].Update(new Vector2(collisionBox.Left, collisionBox.Top));
            debugCrosshairList[1].Update(new Vector2(collisionBox.Right, collisionBox.Top));
            debugCrosshairList[2].Update(new Vector2(collisionBox.Left, collisionBox.Bottom));
            debugCrosshairList[3].Update(new Vector2(collisionBox.Right, collisionBox.Bottom));
            debugCrosshairList[4].Update(position);

            DebugHandler.Instance.AddRange(debugCrosshairList);
#endif
        }

        public void Disable(bool force = false)
        {
            ChangeButtonState(ButtonAnimationState.Disabled, force);
        }

        public void Enable()
        {
            state = ButtonAnimationState.Disabled;
            ChangeButtonState(ButtonAnimationState.Normal, true);
        }

        /*public void AppendAnimationIntoCycle(ButtonAnimationState newState, bool force = false)
        {
            Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[newState], force);
        }*/

        public void ChangeButtonState(ButtonAnimationState newState, bool force = false)
        {
            if (state == newState) return;

            if (!force)
            {
                if (newState == ButtonAnimationState.Disabled)
                    Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[state = newState]);

                if (!selectedPreset.StatePreset.ContainsKey(newState)) return;

                if (state == ButtonAnimationState.Clicked && Flipbook.FlipbookAnimationList.Exists((x) => x.AnimationInstance == selectedPreset.StatePreset[ButtonAnimationState.Normal]))
                {
                    if (Flipbook.FlipbookAnimationList.Count == 1)
                        return;
                    else
                        state = ButtonAnimationState.Normal;
                }

                if (newState == ButtonAnimationState.Normal && Flipbook.FlipbookAnimationList.Exists((x) => x.AnimationInstance == selectedPreset.StatePreset[ButtonAnimationState.Normal]))
                {
                    return;
                }

                if (state == ButtonAnimationState.Hoover && newState == ButtonAnimationState.Normal)
                {
                    Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[state = newState], true);
                    return;
                }

                if (state == ButtonAnimationState.Activated) return;

                if (state == ButtonAnimationState.Clicked) return;

                if (newState == ButtonAnimationState.Clicked)
                {
                    Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[ButtonAnimationState.Clicked], true);
                    Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[ButtonAnimationState.Normal]);
                    state = ButtonAnimationState.Clicked;

                    return;
                }

                Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[state = newState]);
            }
            else
            {
                Flipbook.AppendAnimationIntoCycle(selectedPreset.StatePreset[state = newState], true);
            }
        }

        public void CheckStateChange()
        {
            if (Flipbook.FlipbookAnimationList.Count > 0)
            {
                if (Flipbook.FlipbookAnimationList[0].AnimationInstance == selectedPreset.StatePreset[ButtonAnimationState.Disabled])
                {
                    OnDisabled?.Invoke(this);
                    OnDisabled = null;
                }
            }
        }

        public void Update()
        {
            if (!ShouldUpdate) return;

            CheckStateChange();

            if (state == ButtonAnimationState.Disabled) return;

            if (collisionBox.Intersects(Cursor.Instance.CurrentFlipbook.Position))
            {
                if (InputHandler.IsBeingHeldUp(MKeys.Left))
                {
                    ChangeButtonState(ButtonAnimationState.Hoover);
                }
                else if (InputHandler.IsBeingPressed(MKeys.Left))
                {
                    isBeingPressed = true;
                }
                else if (InputHandler.IsBeingReleased(MKeys.Left))
                {
                    if (!isBeingPressed) return;

                    ChangeButtonState(ButtonAnimationState.Clicked);
                    OnClick(this);
                }
            }
            else
            {
                if (InputHandler.IsBeingReleased(MKeys.Left))
                {
                    isBeingPressed = false;
                }

                ChangeButtonState(ButtonAnimationState.Normal);
            }
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            Flipbook.Draw(GameTime, SpriteBatch);
            if (animatedButtonText != null) animatedButtonText.Draw(SpriteBatch);
        }
    }
}

