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
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public enum ButtonType
    {
        //Misc
        Dummy,

        //In Game
        BlueShotS1, BlueShotS2, BlueShotSS,

        BlueTag,

        BlueShotMode,

        BlueOptions,
        BlueSkipTurn,

        MessageFilter,

        //Server List
        ServerListButton,
        ChannelListButton1, ChannelListButton2, ChannelListButton3, ChannelListButton4,
        ChannelListButton5, ChannelListButton6, ChannelListButton7, ChannelListButton8,

        //GameList
        RoomButton,

        //GameRoom
        PlayerButton,
        RenameButton,
        ChangeMapLeftArrow, ChangeMapRightArrow,

        //Avatar Shop
        AvatarButton, AvatarTabIndex,

        //Scroll Bar
        ScrollBarLeft, ScrollBarRight, ScrollBarUp, ScrollBarDown,

        //Popup
        CreateGameSolo, CreateGameTag, CreateGameJewel, CreateGameScore,
        CreateGame1v1, CreateGame2v2, CreateGame3v3, CreateGame4v4,
        Accept, Cancel,

        //Popup - Select Mobile
        SelectMobileAduka, SelectMobileArmor, SelectMobileASate, SelectMobileBigfoot,
        SelectMobileBoomer, SelectMobileDragon, SelectMobileFrank, SelectMobileGrub,
        SelectMobileIce, SelectMobileJD, SelectMobileJFrog, SelectMobileKalsiddon,
        SelectMobileKnight, SelectMobileLightning, SelectMobileMage, SelectMobileMaya,
        SelectMobileNak, SelectMobilePhoenix, SelectMobileRandom, SelectMobileRaonLauncher,
        SelectMobileTiburon, SelectMobileTrico, SelectMobileTurtle, SelectMobileBlueWhale,
        SelectMobileWolf,

        //Popup - Buy Avatar
        AvatarBuyCash, AvatarBuyGold,

        //Popup - Select Item
        
        //Red - 2 slots
        SelectItemDual, SelectItemDualPlus, SelectItemThunder,
        //Red - 1 Slot
        SelectItemBlood, SelectItemBungeShot, SelectItemPowerUp,

        //Green - 2 Slots
        SelectItemEnergyUp2,
        //Green - 1 Slot
        SelectItemEnergyUp1,

        //Blue - 1 Slot
        SelectItemChangeWind,

        //Purple - 2 Slots
        SelectItemTeamTeleport, SelectItemTeleport,
    }

    public enum ButtonAnimationState
    {
        Normal = 0,
        Hoover = 1,
        Clicked = 2,
        Disabled = 3,
        Activated = 4,
    }

    public struct ButtonPreset
    {
        public string SpritePath;
        public Dictionary<ButtonAnimationState, Rectangle> StatePreset;
    }

    public class Button
    {
        protected static Dictionary<ButtonType, ButtonPreset> ButtonPresets
            = new Dictionary<ButtonType, ButtonPreset>()
            {
                #region InGame
                {
                    ButtonType.BlueShotS1,
                    new ButtonPreset() {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/ShotSelector",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,    0, 37, 37) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37,   0, 37, 37) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37*2, 0, 37, 37) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37*3, 0, 37, 37) },
                            { ButtonAnimationState.Activated, new Rectangle(37*4, 0, 37, 37) },
                        }
                    }
                },
                {
                    ButtonType.BlueShotS2,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/ShotSelector",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37*5, 0, 37, 37) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37*6, 0, 37, 37) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37*7, 0, 37, 37) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37*8, 0, 37, 37) },
                            { ButtonAnimationState.Activated, new Rectangle(37*9, 0, 37, 37) },
                        }
                    }
                },
                {
                    ButtonType.BlueShotSS,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/ShotSelector",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37*10, 0, 37, 37) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37*11, 0, 37, 37) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37*12, 0, 37, 37) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37*13, 0, 37, 37) },
                            { ButtonAnimationState.Activated, new Rectangle(37*14, 0, 37, 37) },
                        }
                    }
                },
                {
                    ButtonType.BlueTag,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/Tag",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,    0, 41, 38) },
                            { ButtonAnimationState.Hoover,    new Rectangle(41,   0, 41, 38) },
                            { ButtonAnimationState.Clicked,   new Rectangle(41*2, 0, 41, 38) },
                            { ButtonAnimationState.Disabled,  new Rectangle(41*3, 0, 41, 38) },
                        }
                    }
                },
                {
                    ButtonType.BlueShotMode,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/ShotMode",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,    0, 34, 27) },
                            { ButtonAnimationState.Hoover,    new Rectangle(34,   0, 34, 27) },
                            { ButtonAnimationState.Clicked,   new Rectangle(34*2, 0, 34, 27) },
                            { ButtonAnimationState.Disabled,  new Rectangle(34*3, 0, 34, 27) },
                        }
                    }
                },
                {
                    ButtonType.BlueOptions,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/Options",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,    0, 60, 23) },
                            { ButtonAnimationState.Hoover,    new Rectangle(60,   0, 60, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(60*2, 0, 60, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(60*3, 0, 60, 23) },
                        }
                    }
                },
                {
                    ButtonType.BlueSkipTurn,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/SkipTurn",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,    0, 60, 23) },
                            { ButtonAnimationState.Hoover,    new Rectangle(60,   0, 60, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(60*2, 0, 60, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(60*3, 0, 60, 23) },
                        }
                    }
                },
                {
                    ButtonType.MessageFilter,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/HUD/Blue/MessageFilter",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(26 * 0, 0, 26, 16) },
                            { ButtonAnimationState.Hoover,    new Rectangle(26 * 1, 0, 26, 16) },
                            { ButtonAnimationState.Clicked,   new Rectangle(26 * 2, 0, 26, 16) },
                            { ButtonAnimationState.Disabled,  new Rectangle(26 * 0, 0, 26, 16) },
                        }
                    }
                },
                #endregion
                #region Server List
                {
                    ButtonType.ServerListButton,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/ServerList/Server",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,     0, 326, 81) },
                            { ButtonAnimationState.Hoover,    new Rectangle(326,   0, 326, 81) },
                            { ButtonAnimationState.Clicked,   new Rectangle(326*2, 0, 326, 81) },
                            { ButtonAnimationState.Disabled,  new Rectangle(326*3, 0, 326, 81) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton1,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel1",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(11 * 0, 0, 11, 15) },
                            { ButtonAnimationState.Hoover,    new Rectangle(11 * 1, 0, 11, 15) },
                            { ButtonAnimationState.Clicked,   new Rectangle(11 * 2, 0, 11, 15) },
                            { ButtonAnimationState.Disabled,  new Rectangle(11 * 3, 0, 11, 15) },
                            { ButtonAnimationState.Activated, new Rectangle(11 * 4, 0, 11, 15) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton2,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel2",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(17 * 0, 0, 17, 15) },
                            { ButtonAnimationState.Hoover,    new Rectangle(17 * 1, 0, 17, 15) },
                            { ButtonAnimationState.Clicked,   new Rectangle(17 * 2, 0, 17, 15) },
                            { ButtonAnimationState.Disabled,  new Rectangle(17 * 3, 0, 17, 15) },
                            { ButtonAnimationState.Activated, new Rectangle(17 * 4, 0, 17, 15) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton3,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel3",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(13 * 0, 0, 13, 16) },
                            { ButtonAnimationState.Hoover,    new Rectangle(13 * 1, 0, 13, 16) },
                            { ButtonAnimationState.Clicked,   new Rectangle(13 * 2, 0, 13, 16) },
                            { ButtonAnimationState.Disabled,  new Rectangle(13 * 3, 0, 13, 16) },
                            { ButtonAnimationState.Activated, new Rectangle(13 * 4, 0, 13, 16) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton4,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel4",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(14 * 0, 0, 14, 15) },
                            { ButtonAnimationState.Hoover,    new Rectangle(14 * 1, 0, 14, 15) },
                            { ButtonAnimationState.Clicked,   new Rectangle(14 * 2, 0, 14, 15) },
                            { ButtonAnimationState.Disabled,  new Rectangle(14 * 3, 0, 14, 15) },
                            { ButtonAnimationState.Activated, new Rectangle(14 * 4, 0, 14, 15) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton5,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel5",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(18 * 0, 0, 18, 17) },
                            { ButtonAnimationState.Hoover,    new Rectangle(18 * 1, 0, 18, 17) },
                            { ButtonAnimationState.Clicked,   new Rectangle(18 * 2, 0, 18, 17) },
                            { ButtonAnimationState.Disabled,  new Rectangle(18 * 3, 0, 18, 17) },
                            { ButtonAnimationState.Activated, new Rectangle(18 * 4, 0, 18, 17) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton6,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel6",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(16 * 0, 0, 16, 17) },
                            { ButtonAnimationState.Hoover,    new Rectangle(16 * 1, 0, 16, 17) },
                            { ButtonAnimationState.Clicked,   new Rectangle(16 * 2, 0, 16, 17) },
                            { ButtonAnimationState.Disabled,  new Rectangle(16 * 3, 0, 16, 17) },
                            { ButtonAnimationState.Activated, new Rectangle(16 * 4, 0, 16, 17) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton7,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel7",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(15 * 0, 0, 15, 15) },
                            { ButtonAnimationState.Hoover,    new Rectangle(15 * 1, 0, 15, 15) },
                            { ButtonAnimationState.Clicked,   new Rectangle(15 * 2, 0, 15, 15) },
                            { ButtonAnimationState.Disabled,  new Rectangle(15 * 3, 0, 15, 15) },
                            { ButtonAnimationState.Activated, new Rectangle(15 * 4, 0, 15, 15) },
                        }
                    }
                },
                {
                    ButtonType.ChannelListButton8,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameList/Channel8",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(15 * 0, 0, 15, 18) },
                            { ButtonAnimationState.Hoover,    new Rectangle(15 * 1, 0, 15, 18) },
                            { ButtonAnimationState.Clicked,   new Rectangle(15 * 2, 0, 15, 18) },
                            { ButtonAnimationState.Disabled,  new Rectangle(15 * 3, 0, 15, 18) },
                            { ButtonAnimationState.Activated, new Rectangle(15 * 4, 0, 15, 18) },
                        }
                    }
                },
                #endregion
                #region Room
                {
                    ButtonType.PlayerButton,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameRoom/Player/ButtonPlaceholder",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(0,     0, 200, 70) },
                        }
                    }
                },
                {
                    ButtonType.RenameButton,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameRoom/RenameRoom",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(17*0, 0, 17, 17) },
                            { ButtonAnimationState.Hoover,    new Rectangle(17*1, 0, 17, 17) },
                            { ButtonAnimationState.Clicked,   new Rectangle(17*2, 0, 17, 17) },
                            { ButtonAnimationState.Disabled,  new Rectangle(17*3, 0, 17, 17) },
                        }
                    }
                },
                {
                    ButtonType.ChangeMapLeftArrow,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameRoom/LeftMap",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(18*0, 0, 18, 18) },
                            { ButtonAnimationState.Hoover,    new Rectangle(18*1, 0, 18, 18) },
                            { ButtonAnimationState.Clicked,   new Rectangle(18*2, 0, 18, 18) },
                            { ButtonAnimationState.Disabled,  new Rectangle(18*3, 0, 18, 18) },
                        }
                    }
                },
                {
                    ButtonType.ChangeMapRightArrow,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/GameRoom/RightMap",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(18*0, 0, 18, 18) },
                            { ButtonAnimationState.Hoover,    new Rectangle(18*1, 0, 18, 18) },
                            { ButtonAnimationState.Clicked,   new Rectangle(18*2, 0, 18, 18) },
                            { ButtonAnimationState.Disabled,  new Rectangle(18*3, 0, 18, 18) },
                        }
                    }
                },
                #endregion
                #region AvatarShop
                {
                    ButtonType.AvatarButton,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/AvatarShop/AvatarButton/Background",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(105 * 0, 0, 105, 83) },
                            { ButtonAnimationState.Hoover,    new Rectangle(105 * 1, 0, 105, 83) },
                            { ButtonAnimationState.Clicked,   new Rectangle(105 * 2, 0, 105, 83) },
                            { ButtonAnimationState.Activated, new Rectangle(105 * 2, 0, 105, 83) },
                        }
                    }
                },
                {
                    ButtonType.AvatarTabIndex,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/AvatarShop/TabIndex",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Disabled,   new Rectangle(82 * 0, 0, 82, 30) },
                            { ButtonAnimationState.Normal, new Rectangle(82 * 1, 0, 82, 30) },
                        }
                    }
                },
                #endregion
                #region Scroll Bar
                {
                    ButtonType.ScrollBarDown,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/TextBox/Scroll",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(23 * 0, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Activated, new Rectangle(23 * 0, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(23 * 0, 23 * 1, 23, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(23 * 0, 23 * 1, 23, 23) },
                        }
                    }
                },
                {
                    ButtonType.ScrollBarUp,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/TextBox/Scroll",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(23 * 1, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Activated, new Rectangle(23 * 1, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(23 * 1, 23 * 1, 23, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(23 * 1, 23 * 1, 23, 23) },
                        }
                    }
                },
                {
                    ButtonType.ScrollBarLeft,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/TextBox/Scroll",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(23 * 2, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Activated, new Rectangle(23 * 2, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(23 * 2, 23 * 1, 23, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(23 * 2, 23 * 1, 23, 23) },
                        }
                    }
                },
                {
                    ButtonType.ScrollBarRight,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/TextBox/Scroll",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(23 * 3, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Activated, new Rectangle(23 * 3, 23 * 0, 23, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(23 * 3, 23 * 1, 23, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(23 * 3, 23 * 1, 23, 23) },
                        }
                    }
                },
                #endregion
                #region Popup
                {
                    ButtonType.Accept,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/Accept",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(20*0, 0, 20, 25) },
                            { ButtonAnimationState.Hoover,    new Rectangle(20*1, 0, 20, 25) },
                            { ButtonAnimationState.Clicked,   new Rectangle(20*2, 0, 20, 25) },
                            { ButtonAnimationState.Disabled,  new Rectangle(20*3, 0, 20, 25) },
                        }
                    }
                },
                {
                    ButtonType.Cancel,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/Cancel",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(23*0, 0, 23, 23) },
                            { ButtonAnimationState.Hoover,    new Rectangle(23*1, 0, 23, 23) },
                            { ButtonAnimationState.Clicked,   new Rectangle(23*2, 0, 23, 23) },
                            { ButtonAnimationState.Disabled,  new Rectangle(23*3, 0, 23, 23) },
                        }
                    }
                },
                #endregion
                #region Popup - Create Game
                {
                    ButtonType.CreateGame1v1,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/1v1",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(51*0, 0, 51, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(51*1, 0, 51, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(51*2, 0, 51, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(51*3, 0, 51, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(51*4, 0, 51, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGame2v2,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/2v2",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(51*0, 0, 51, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(51*1, 0, 51, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(51*2, 0, 51, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(51*3, 0, 51, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(51*4, 0, 51, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGame3v3,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/3v3",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(51*0, 0, 51, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(51*1, 0, 51, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(51*2, 0, 51, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(51*3, 0, 51, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(51*4, 0, 51, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGame4v4,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/4v4",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(51*0, 0, 51, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(51*1, 0, 51, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(51*2, 0, 51, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(51*3, 0, 51, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(51*4, 0, 51, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGameSolo,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/Solo",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(32*0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(32*1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(32*2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(32*3, 0, 32, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(32*4, 0, 32, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGameTag,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/Tag",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(32*0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(32*1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(32*2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(32*3, 0, 32, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(32*4, 0, 32, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGameJewel,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/Jewel",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(32*0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(32*1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(32*2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(32*3, 0, 32, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(32*4, 0, 32, 31) },
                        }
                    }
                },
                {
                    ButtonType.CreateGameScore,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/CreateRoom/Score",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(32*0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,    new Rectangle(32*1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,   new Rectangle(32*2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled,  new Rectangle(32*3, 0, 32, 31) },
                            { ButtonAnimationState.Activated, new Rectangle(32*4, 0, 32, 31) },
                        }
                    }
                },
                #endregion
                #region Popup - Select Mobile
                {
                    ButtonType.SelectMobileAduka,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 0, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileArmor,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 0, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileASate,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 0, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileBigfoot,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 0, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 0, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileBoomer,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 1, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileDragon,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 1, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileFrank,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 1, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileGrub,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 1, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 1, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileIce,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 2, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileJD,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 2, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileJFrog,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 2, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileKalsiddon,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 2, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 2, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileKnight,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 3, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileLightning,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 3, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileMage,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 3, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileMaya,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 3, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 3, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileNak,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 4, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobilePhoenix,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 4, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileRandom,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 4, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileRaonLauncher,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 4, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 4, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileTiburon,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 5, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileTrico,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 05, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 06, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 07, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 08, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 09, 62 * 5, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileTurtle,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 10, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 11, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 12, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 13, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 14, 62 * 5, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileBlueWhale,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 15, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 16, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 17, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 18, 62 * 5, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 19, 62 * 5, 67, 62) },
                        }
                    }
                },
                {
                    ButtonType.SelectMobileWolf,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectMobile/Mobile",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(67 * 00, 62 * 6, 67, 62) },
                            { ButtonAnimationState.Hoover,    new Rectangle(67 * 01, 62 * 6, 67, 62) },
                            { ButtonAnimationState.Clicked,   new Rectangle(67 * 02, 62 * 6, 67, 62) },
                            { ButtonAnimationState.Disabled,  new Rectangle(67 * 03, 62 * 6, 67, 62) },
                            { ButtonAnimationState.Activated, new Rectangle(67 * 04, 62 * 6, 67, 62) },
                        }
                    }
                },
                #endregion
                #region Popup - Buy Avatar
                {
                    ButtonType.AvatarBuyCash,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/AvatarShop/Cash",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,   new Rectangle(32 * 0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,   new Rectangle(32 * 1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,  new Rectangle(32 * 2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled, new Rectangle(32 * 3, 0, 32, 31) },
                        }
                    }
                },
                {
                    ButtonType.AvatarBuyGold,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/AvatarShop/Gold",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,   new Rectangle(32 * 0, 0, 32, 31) },
                            { ButtonAnimationState.Hoover,   new Rectangle(32 * 1, 0, 32, 31) },
                            { ButtonAnimationState.Clicked,  new Rectangle(32 * 2, 0, 32, 31) },
                            { ButtonAnimationState.Disabled, new Rectangle(32 * 3, 0, 32, 31) },
                        }
                    }
                },
                #endregion
                #region Popup - Select Item - 2 Slots Items
                {
                    ButtonType.SelectItemDual,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/Dual",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemDualPlus,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/DualPlus",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemThunder,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/Thunder",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemEnergyUp2,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/EnergyUp2",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemTeamTeleport,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/TeamTeleport",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemTeleport,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/Teleport",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(73 * 0, 0, 73, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(73 * 1, 0, 73, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(73 * 2, 0, 73, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(73 * 3, 0, 73, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(73 * 2, 0, 73, 36) },
                        }
                    }
                },
                #endregion
                #region Popup - Select Item - 1 Slot Item
                {
                    ButtonType.SelectItemBlood,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/Blood",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37 * 0, 0, 37, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37 * 1, 0, 37, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37 * 2, 0, 37, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37 * 3, 0, 37, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(37 * 2, 0, 37, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemBungeShot,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/BungeShot",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37 * 0, 0, 37, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37 * 1, 0, 37, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37 * 2, 0, 37, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37 * 3, 0, 37, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(37 * 2, 0, 37, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemPowerUp,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/PowerUp",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37 * 0, 0, 37, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37 * 1, 0, 37, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37 * 2, 0, 37, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37 * 3, 0, 37, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(37 * 2, 0, 37, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemEnergyUp1,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/EnergyUp1",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37 * 0, 0, 37, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37 * 1, 0, 37, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37 * 2, 0, 37, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37 * 3, 0, 37, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(37 * 2, 0, 37, 36) },
                        }
                    }
                },
                {
                    ButtonType.SelectItemChangeWind,
                    new ButtonPreset()
                    {
                        SpritePath = "Interface/StaticButtons/Popup/SelectItem/ChangeWind",
                        StatePreset = new Dictionary<ButtonAnimationState, Rectangle>()
                        {
                            { ButtonAnimationState.Normal,    new Rectangle(37 * 0, 0, 37, 36) },
                            { ButtonAnimationState.Hoover,    new Rectangle(37 * 1, 0, 37, 36) },
                            { ButtonAnimationState.Clicked,   new Rectangle(37 * 2, 0, 37, 36) },
                            { ButtonAnimationState.Disabled,  new Rectangle(37 * 3, 0, 37, 36) },
                            { ButtonAnimationState.Activated, new Rectangle(37 * 2, 0, 37, 36) },
                        }
                    }
                },
                #endregion
            };

        public Sprite ButtonSprite { get; private set; }
        public Vector2 ButtonOffset { get; set; }
        protected ButtonAnimationState buttonAnimationState;
        protected Dictionary<ButtonAnimationState, Rectangle> buttonInstances;

        protected Cursor cursor;
        protected bool isBeingPressed;
        public bool IsActivated { get; set; }
        public bool IsEnabled { get; set; }

        protected bool isClicked;

        public bool ShouldUpdate;

        public Action<object> OnBeingPressed;
        public Action<object> OnBeingDragged;
        public Action<object> OnClick;
        public Action<object> OnBeginHoover;
        public Action<object> OnHoover;
        public Action<object> OnUnhoover;
        public Action<object> OnBeingReleased;

        public object Tag;

        public bool ShouldRender;

#if DEBUG
        public List<DebugCrosshair> DebugCrosshairList;
#endif

        public Button(ButtonType buttonType, float layerDepth, Action<object> onClick, Vector2 buttonOffset = default, ButtonPreset buttonPreset = default)
        {
            if (buttonPreset.Equals(default(ButtonPreset)))
                buttonPreset = ButtonPresets[buttonType];
            buttonInstances = buttonPreset.StatePreset;

            buttonAnimationState = ButtonAnimationState.Disabled;

            ButtonSprite = new Sprite(buttonPreset.SpritePath, layerDepth: layerDepth);
            ButtonSprite.Pivot = new Vector2(buttonInstances.First().Value.Width, buttonInstances.First().Value.Height) / 2;
            ButtonOffset = buttonOffset;
            cursor = Cursor.Instance;
            isBeingPressed = false;

            ChangeButtonState(ButtonAnimationState.Normal);

            OnClick = onClick;
            OnBeginHoover = OnHoover = OnUnhoover = OnBeingReleased = OnBeingPressed = OnBeingDragged = default;

            ShouldUpdate = true;
            IsEnabled = true;
            ShouldRender = true;

#if DEBUG
            DebugCrosshairList = new List<DebugCrosshair>()
            {
                new DebugCrosshair(Color.White),new DebugCrosshair(Color.White),
                new DebugCrosshair(Color.White),new DebugCrosshair(Color.White),
                new DebugCrosshair(Color.Red),
            };

            DebugHandler.Instance.AddRange(DebugCrosshairList);
#endif
        }

        public virtual void Disable()
        {
            IsActivated = false;
            IsEnabled = false;
            ChangeButtonState(ButtonAnimationState.Disabled, true);
        }

        public virtual void Enable()
        {
            IsEnabled = true;
            ChangeButtonState(ButtonAnimationState.Normal, true);
        }

        public void ChangeButtonState(ButtonAnimationState NewState, bool Force = false)
        {
            if (buttonAnimationState == NewState) return;

            if (!buttonInstances.ContainsKey(NewState)) return;

            if (!Force)
            {
                if ((buttonAnimationState == ButtonAnimationState.Activated) && (NewState == ButtonAnimationState.Normal || NewState == ButtonAnimationState.Hoover)) return;
            }

            ButtonSprite.SourceRectangle = buttonInstances[buttonAnimationState = NewState];
        }

        protected virtual Rectangle CalculateCollisionRectangle()
        {
            return new Rectangle(
                (int)ButtonSprite.Position.X - (int)(ButtonSprite.Pivot.X * ButtonSprite.Scale.X),
                (int)ButtonSprite.Position.Y - (int)(ButtonSprite.Pivot.Y * ButtonSprite.Scale.Y),
                (int)(ButtonSprite.SourceRectangle.Width  * ButtonSprite.Scale.X),
                (int)(ButtonSprite.SourceRectangle.Height * ButtonSprite.Scale.Y));
        }

        public virtual void UpdateAttatchedPosition()
        {
            ButtonSprite.Position = ButtonOffset - GameScene.Camera.CameraOffset;
        }

        public virtual void Update()
        {
            if (!ShouldUpdate || !ShouldRender) return;

            UpdateAttatchedPosition();

            if (buttonAnimationState == ButtonAnimationState.Disabled) return;

            Rectangle collisionRectangle = CalculateCollisionRectangle();

            if (collisionRectangle.Intersects(cursor.CurrentFlipbook.Position))
            {
                if (InputHandler.IsBeingPressed(MKeys.Left))
                {
                    OnBeingPressed?.Invoke(this);
                    isBeingPressed = true;
                }
                else if (InputHandler.IsBeingHeldDown(MKeys.Left))
                {
                    if (isBeingPressed && buttonAnimationState != ButtonAnimationState.Disabled)
                    {
                        ChangeButtonState(ButtonAnimationState.Clicked);
                    }
                }
                else if (InputHandler.IsBeingReleased(MKeys.Left))
                {
                    if (isBeingPressed)
                        OnBeingReleased?.Invoke(this);
                    else
                        return;

                    isBeingPressed = false;
                    IsActivated = !IsActivated;

                    ChangeButtonState(IsActivated ? ButtonAnimationState.Activated : ButtonAnimationState.Normal);

                    OnClick?.Invoke(this);
                }
                else if (buttonAnimationState != ButtonAnimationState.Hoover)
                {
                    ChangeButtonState(ButtonAnimationState.Hoover);
                    OnBeginHoover?.Invoke(this);
                }
                else
                {
                    OnHoover?.Invoke(this);
                }
            }
            else
            {
                if (InputHandler.IsBeingReleased(MKeys.Left))
                {
                    if (isBeingPressed)
                        OnBeingReleased?.Invoke(this);

                    isBeingPressed = false;
                }

                if (buttonInstances.ContainsKey(ButtonAnimationState.Activated))
                    ChangeButtonState(IsActivated ? ButtonAnimationState.Activated : ButtonAnimationState.Normal);
                else
                    ChangeButtonState(ButtonAnimationState.Normal);
            }

            if (isBeingPressed)
            {
                OnBeingDragged?.Invoke(this);
            }

#if DEBUG
            DebugCrosshairList[0].Update(new Vector2(collisionRectangle.X, collisionRectangle.Y));
            DebugCrosshairList[1].Update(new Vector2(collisionRectangle.X, collisionRectangle.Y + collisionRectangle.Height));
            DebugCrosshairList[2].Update(new Vector2(collisionRectangle.X + collisionRectangle.Width, collisionRectangle.Y + collisionRectangle.Height));
            DebugCrosshairList[3].Update(new Vector2(collisionRectangle.X + collisionRectangle.Width, collisionRectangle.Y));
            DebugCrosshairList[4].Update(ButtonSprite.Position);
#endif
        }

        public virtual void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            if (!ShouldRender) return;

            ButtonSprite.Draw(GameTime, SpriteBatch);
        }
    }
}
