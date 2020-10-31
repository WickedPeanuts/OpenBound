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
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Level.Scene;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OpenBound.GameComponents.Interface
{
    public enum FontType
    {
        GameListPlayerCounter,

        GameRoomPingCounterRed,
        GameRoomPingCounterWhite,

        //Angle
        HUDBlueCurrentAngle,
        HUDBluePreviousAngle,

        //Floating Text
        HUDBlueDamage,

        //Turn/Delay Board
        HUDBlueRedTeamTurnCounter,
        HUDBlueBlueTeamTurnCounter,
        HUDBlueDelay,

        //Life/Gold board
        HUDBlueGold,
        HUDBlueScoreboard,
        HUDBlueCrosshairTrueAngle,
        HUDBlueCrosshairFalseAngle,
        HUDBlueWindCompass,

        //Status Bar
        HUDBlueStatusBar,
        
        //Thor
        HUDBlueThorLevelIndicator,
        HUDBlueThorExperienceIndicator,

        //Time Counter
        HUDTurnTimer,

        //Avatar Shop
        AvatarShopStatusCounter,
    }

    public enum TextAnchor
    {
        Left,
        Middle,
        Right,
    }

    public enum Number
    {
        N0 = 0, N1 = 1, N2 = 2, N3 = 3, N4 = 4, N5 = 5,
        N6 = 6, N7 = 7, N8 = 8, N9 = 9, NM = 10
    }

    //This library is used on fonts made by sprite, the same as the ones used on life counter, dmg counter
    public abstract class SpriteNumericField
    {
        private static readonly Dictionary<FontType, string> spritePath = new Dictionary<FontType, string>()
        {
            { FontType.GameListPlayerCounter, "GameList/PlayerCounter" },

            { FontType.GameRoomPingCounterRed, "Interface/Spritefont/GameRoom/Counter" },
            { FontType.GameRoomPingCounterWhite, "Interface/Spritefont/GameRoom/Counter" },

            { FontType.HUDBlueCurrentAngle, "HUD/Blue/CurrentAngle" },
            { FontType.HUDBluePreviousAngle, "HUD/Blue/PreviousAngle" },
            { FontType.HUDBlueDamage, "HUD/Blue/Damage" },

            { FontType.HUDBlueRedTeamTurnCounter, "Interface/Spritefont/HUD/Blue/RedTeamTurnCounter" },
            { FontType.HUDBlueBlueTeamTurnCounter, "Interface/Spritefont/HUD/Blue/BlueTeamTurnCounter" },
            { FontType.HUDBlueDelay, "HUD/Blue/Delay" },
            { FontType.HUDBlueCrosshairTrueAngle, "Interface/Spritefont/HUD/Blue/CrosshairAngle" },
            { FontType.HUDBlueCrosshairFalseAngle, "Interface/Spritefont/HUD/Blue/CrosshairAngle" },

            { FontType.HUDBlueGold, "HUD/Blue/Gold" },
            { FontType.HUDBlueScoreboard, "HUD/Blue/Scoreboard" },

            { FontType.HUDBlueWindCompass, "Interface/Spritefont/HUD/Blue/WindCompass" },

            { FontType.HUDBlueStatusBar, "Interface/Spritefont/HUD/Blue/StatusBarCounter" },

            { FontType.HUDBlueThorLevelIndicator, "Interface/Spritefont/HUD/Blue/ThorLevel" },
            { FontType.HUDBlueThorExperienceIndicator, "Interface/Spritefont/HUD/Blue/Defense" },

            //Avatar Shop
            { FontType.AvatarShopStatusCounter, "Interface/Spritefont/AvatarShop/AvatarStatusCounter" },

            //Time Counter
            { FontType.HUDTurnTimer, "Interface/Spritefont/HUD/Blue/TurnTimer" },
        };

        private static readonly Dictionary<FontType, Dictionary<Number, Rectangle>> numericTextFieldStatePresets
            = new Dictionary<FontType, Dictionary<Number, Rectangle>>()
            {
                #region GameList
                {
                    FontType.GameListPlayerCounter, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 10, 13) },
                        { Number.N1, new Rectangle(1  * 10, 0, 10, 13) },
                        { Number.N2, new Rectangle(2  * 10, 0, 10, 13) },
                        { Number.N3, new Rectangle(3  * 10, 0, 10, 13) },
                        { Number.N4, new Rectangle(4  * 10, 0, 10, 13) },
                        { Number.N5, new Rectangle(5  * 10, 0, 10, 13) },
                        { Number.N6, new Rectangle(6  * 10, 0, 10, 13) },
                        { Number.N7, new Rectangle(7  * 10, 0, 10, 13) },
                        { Number.N8, new Rectangle(8  * 10, 0, 10, 13) },
                        { Number.N9, new Rectangle(9  * 10, 0, 10, 13) },
                    }
                },
                #endregion
                #region GameRoom
                {
                    FontType.GameRoomPingCounterRed, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0     , 0, 8, 9) },
                        { Number.N1, new Rectangle(1  * 8, 0, 8, 9) },
                        { Number.N2, new Rectangle(2  * 8, 0, 8, 9) },
                        { Number.N3, new Rectangle(3  * 8, 0, 8, 9) },
                        { Number.N4, new Rectangle(4  * 8, 0, 8, 9) },
                        { Number.N5, new Rectangle(5  * 8, 0, 8, 9) },
                        { Number.N6, new Rectangle(6  * 8, 0, 8, 9) },
                        { Number.N7, new Rectangle(7  * 8, 0, 8, 9) },
                        { Number.N8, new Rectangle(8  * 8, 0, 8, 9) },
                        { Number.N9, new Rectangle(9  * 8, 0, 8, 9) },
                    }
                },
                {
                    FontType.GameRoomPingCounterWhite, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(10 * 8, 0, 8, 9) },
                        { Number.N1, new Rectangle(11 * 8, 0, 8, 9) },
                        { Number.N2, new Rectangle(12 * 8, 0, 8, 9) },
                        { Number.N3, new Rectangle(13 * 8, 0, 8, 9) },
                        { Number.N4, new Rectangle(14 * 8, 0, 8, 9) },
                        { Number.N5, new Rectangle(15 * 8, 0, 8, 9) },
                        { Number.N6, new Rectangle(16 * 8, 0, 8, 9) },
                        { Number.N7, new Rectangle(17 * 8, 0, 8, 9) },
                        { Number.N8, new Rectangle(18 * 8, 0, 8, 9) },
                        { Number.N9, new Rectangle(19 * 8, 0, 8, 9) },
                    }
                },
                #endregion
                #region HUDBlue
                {
                    FontType.HUDBlueCurrentAngle, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 15, 15) },
                        { Number.N1, new Rectangle(1  * 15, 0, 15, 15) },
                        { Number.N2, new Rectangle(2  * 15, 0, 15, 15) },
                        { Number.N3, new Rectangle(3  * 15, 0, 15, 15) },
                        { Number.N4, new Rectangle(4  * 15, 0, 15, 15) },
                        { Number.N5, new Rectangle(5  * 15, 0, 15, 15) },
                        { Number.N6, new Rectangle(6  * 15, 0, 15, 15) },
                        { Number.N7, new Rectangle(7  * 15, 0, 15, 15) },
                        { Number.N8, new Rectangle(8  * 15, 0, 15, 15) },
                        { Number.N9, new Rectangle(9  * 15, 0, 15, 15) },
                        { Number.NM, new Rectangle(10 * 15, 0, 15, 15) },
                    }
                },
                {
                    FontType.HUDBluePreviousAngle, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 10, 8) },
                        { Number.N1, new Rectangle(1  * 10, 0, 10, 8) },
                        { Number.N2, new Rectangle(2  * 10, 0, 10, 8) },
                        { Number.N3, new Rectangle(3  * 10, 0, 10, 8) },
                        { Number.N4, new Rectangle(4  * 10, 0, 10, 8) },
                        { Number.N5, new Rectangle(5  * 10, 0, 10, 8) },
                        { Number.N6, new Rectangle(6  * 10, 0, 10, 8) },
                        { Number.N7, new Rectangle(7  * 10, 0, 10, 8) },
                        { Number.N8, new Rectangle(8  * 10, 0, 10, 8) },
                        { Number.N9, new Rectangle(9  * 10, 0, 10, 8) },
                        { Number.NM, new Rectangle(10 * 10, 0, 10, 8) },
                    }
                },
                {
                    FontType.HUDBlueDamage, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(1  * 14, 0, 14, 19) },
                        { Number.N1, new Rectangle(2  * 14, 0, 14, 19) },
                        { Number.N2, new Rectangle(3  * 14, 0, 14, 19) },
                        { Number.N3, new Rectangle(4  * 14, 0, 14, 19) },
                        { Number.N4, new Rectangle(5  * 14, 0, 14, 19) },
                        { Number.N5, new Rectangle(6  * 14, 0, 14, 19) },
                        { Number.N6, new Rectangle(7  * 14, 0, 14, 19) },
                        { Number.N7, new Rectangle(8  * 14, 0, 14, 19) },
                        { Number.N8, new Rectangle(9  * 14, 0, 14, 19) },
                        { Number.N9, new Rectangle(10 * 14, 0, 14, 19) },
                        { Number.NM, new Rectangle(0      , 0, 14, 19) },
                    }
                },
                {
                    FontType.HUDBlueDelay, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0     , 0, 8, 9) },
                        { Number.N1, new Rectangle(1  * 8, 0, 8, 9) },
                        { Number.N2, new Rectangle(2  * 8, 0, 8, 9) },
                        { Number.N3, new Rectangle(3  * 8, 0, 8, 9) },
                        { Number.N4, new Rectangle(4  * 8, 0, 8, 9) },
                        { Number.N5, new Rectangle(5  * 8, 0, 8, 9) },
                        { Number.N6, new Rectangle(6  * 8, 0, 8, 9) },
                        { Number.N7, new Rectangle(7  * 8, 0, 8, 9) },
                        { Number.N8, new Rectangle(8  * 8, 0, 8, 9) },
                        { Number.N9, new Rectangle(9  * 8, 0, 8, 9) },
                        { Number.NM, new Rectangle(10 * 8, 0, 8, 9) },
                    }
                },
                {
                    FontType.HUDBlueRedTeamTurnCounter, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 13, 17) },
                        { Number.N1, new Rectangle(1  * 13, 0, 13, 17) },
                        { Number.N2, new Rectangle(2  * 13, 0, 13, 17) },
                        { Number.N3, new Rectangle(3  * 13, 0, 13, 17) },
                        { Number.N4, new Rectangle(4  * 13, 0, 13, 17) },
                        { Number.N5, new Rectangle(5  * 13, 0, 13, 17) },
                        { Number.N6, new Rectangle(6  * 13, 0, 13, 17) },
                        { Number.N7, new Rectangle(7  * 13, 0, 13, 17) },
                    }
                },
                {
                    FontType.HUDBlueBlueTeamTurnCounter, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 13, 17) },
                        { Number.N1, new Rectangle(1  * 13, 0, 13, 17) },
                        { Number.N2, new Rectangle(2  * 13, 0, 13, 17) },
                        { Number.N3, new Rectangle(3  * 13, 0, 13, 17) },
                        { Number.N4, new Rectangle(4  * 13, 0, 13, 17) },
                        { Number.N5, new Rectangle(5  * 13, 0, 13, 17) },
                        { Number.N6, new Rectangle(6  * 13, 0, 13, 17) },
                        { Number.N7, new Rectangle(7  * 13, 0, 13, 17) },
                    }
                },
                {
                    FontType.HUDBlueGold, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0     , 0, 15, 15) },
                        { Number.N1, new Rectangle(1  * 15, 0, 15, 15) },
                        { Number.N2, new Rectangle(2  * 15, 0, 15, 15) },
                        { Number.N3, new Rectangle(3  * 15, 0, 15, 15) },
                        { Number.N4, new Rectangle(4  * 15, 0, 15, 15) },
                        { Number.N5, new Rectangle(5  * 15, 0, 15, 15) },
                        { Number.N6, new Rectangle(6  * 15, 0, 15, 15) },
                        { Number.N7, new Rectangle(7  * 15, 0, 15, 15) },
                        { Number.N8, new Rectangle(8  * 15, 0, 15, 15) },
                        { Number.N9, new Rectangle(9  * 15, 0, 15, 15) },
                        { Number.NM, new Rectangle(10 * 15, 0, 15, 15) },
                    }
                },
                {
                    FontType.HUDBlueScoreboard, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 19, 20) },
                        { Number.N1, new Rectangle(1  * 19, 0, 19, 20) },
                        { Number.N2, new Rectangle(2  * 19, 0, 19, 20) },
                        { Number.N3, new Rectangle(3  * 19, 0, 19, 20) },
                        { Number.N4, new Rectangle(4  * 19, 0, 19, 20) },
                        { Number.N5, new Rectangle(5  * 19, 0, 19, 20) },
                        { Number.N6, new Rectangle(6  * 19, 0, 19, 20) },
                        { Number.N7, new Rectangle(7  * 19, 0, 19, 20) },
                        { Number.N8, new Rectangle(8  * 19, 0, 19, 20) },
                        { Number.N9, new Rectangle(9  * 19, 0, 19, 20) },
                    }
                },
                {
                    FontType.HUDBlueCrosshairTrueAngle, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 12, 10, 12) },
                        { Number.N1, new Rectangle(1  * 10, 12, 10, 12) },
                        { Number.N2, new Rectangle(2  * 10, 12, 10, 12) },
                        { Number.N3, new Rectangle(3  * 10, 12, 10, 12) },
                        { Number.N4, new Rectangle(4  * 10, 12, 10, 12) },
                        { Number.N5, new Rectangle(5  * 10, 12, 10, 12) },
                        { Number.N6, new Rectangle(6  * 10, 12, 10, 12) },
                        { Number.N7, new Rectangle(7  * 10, 12, 10, 12) },
                        { Number.N8, new Rectangle(8  * 10, 12, 10, 12) },
                        { Number.N9, new Rectangle(9  * 10, 12, 10, 12) },
                        { Number.NM, new Rectangle(10 * 10, 12, 10, 12) },
                    }
                },
                {
                    FontType.HUDBlueCrosshairFalseAngle, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 10, 12) },
                        { Number.N1, new Rectangle(1  * 10, 0, 10, 12) },
                        { Number.N2, new Rectangle(2  * 10, 0, 10, 12) },
                        { Number.N3, new Rectangle(3  * 10, 0, 10, 12) },
                        { Number.N4, new Rectangle(4  * 10, 0, 10, 12) },
                        { Number.N5, new Rectangle(5  * 10, 0, 10, 12) },
                        { Number.N6, new Rectangle(6  * 10, 0, 10, 12) },
                        { Number.N7, new Rectangle(7  * 10, 0, 10, 12) },
                        { Number.N8, new Rectangle(8  * 10, 0, 10, 12) },
                        { Number.N9, new Rectangle(9  * 10, 0, 10, 12) },
                        { Number.NM, new Rectangle(10 * 10, 0, 10, 12) },
                    }
                },
                {
                    FontType.HUDBlueWindCompass, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0, 13, 19) },
                        { Number.N1, new Rectangle(1  * 13, 0, 13, 19) },
                        { Number.N2, new Rectangle(2  * 13, 0, 13, 19) },
                        { Number.N3, new Rectangle(3  * 13, 0, 13, 19) },
                        { Number.N4, new Rectangle(4  * 13, 0, 13, 19) },
                        { Number.N5, new Rectangle(5  * 13, 0, 13, 19) },
                        { Number.N6, new Rectangle(6  * 13, 0, 13, 19) },
                        { Number.N7, new Rectangle(7  * 13, 0, 13, 19) },
                        { Number.N8, new Rectangle(8  * 13, 0, 13, 19) },
                        { Number.N9, new Rectangle(9  * 13, 0, 13, 19) },
                    }
                },
                {
                    FontType.HUDBlueThorLevelIndicator, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N1, new Rectangle(0  * 9, 0, 9, 12) },
                        { Number.N2, new Rectangle(1  * 9, 0, 9, 12) },
                        { Number.N3, new Rectangle(2  * 9, 0, 9, 12) },
                        { Number.N4, new Rectangle(3  * 9, 0, 9, 12) },
                        { Number.N5, new Rectangle(4  * 9, 0, 9, 12) },
                        { Number.N6, new Rectangle(5  * 9, 0, 9, 12) },
                    }
                },
                {
                    FontType.HUDBlueThorExperienceIndicator, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0     , 0, 8, 9) },
                        { Number.N1, new Rectangle(1  * 8, 0, 8, 9) },
                        { Number.N2, new Rectangle(2  * 8, 0, 8, 9) },
                        { Number.N3, new Rectangle(3  * 8, 0, 8, 9) },
                        { Number.N4, new Rectangle(4  * 8, 0, 8, 9) },
                        { Number.N5, new Rectangle(5  * 8, 0, 8, 9) },
                        { Number.N6, new Rectangle(6  * 8, 0, 8, 9) },
                        { Number.N7, new Rectangle(7  * 8, 0, 8, 9) },
                        { Number.N8, new Rectangle(8  * 8, 0, 8, 9) },
                        { Number.N9, new Rectangle(9  * 8, 0, 8, 9) },
                        { Number.NM, new Rectangle(10 * 8, 0, 8, 9) },
                    }
                },
                {
                    FontType.HUDTurnTimer, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0      , 0 * 82, 66, 82) },
                        { Number.N1, new Rectangle(1  * 66, 0 * 82, 66, 82) },
                        { Number.N2, new Rectangle(2  * 66, 0 * 82, 66, 82) },
                        { Number.N3, new Rectangle(3  * 66, 0 * 82, 66, 82) },
                        { Number.N4, new Rectangle(4  * 66, 0 * 82, 66, 82) },
                        { Number.N5, new Rectangle(0  * 66, 1 * 82, 66, 82) },
                        { Number.N6, new Rectangle(1  * 66, 1 * 82, 66, 82) },
                        { Number.N7, new Rectangle(2  * 66, 1 * 82, 66, 82) },
                        { Number.N8, new Rectangle(3  * 66, 1 * 82, 66, 82) },
                        { Number.N9, new Rectangle(4  * 66, 1 * 82, 66, 82) },
                    }
                },
                {
                    FontType.HUDBlueStatusBar, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle( 0      , 0, 10, 12) },
                        { Number.N1, new Rectangle( 1  * 10, 0, 10, 12) },
                        { Number.N2, new Rectangle( 2  * 10, 0, 10, 12) },
                        { Number.N3, new Rectangle( 3  * 10, 0, 10, 12) },
                        { Number.N4, new Rectangle( 4  * 10, 0, 10, 12) },
                        { Number.N5, new Rectangle( 5  * 10, 0, 10, 12) },
                        { Number.N6, new Rectangle( 6  * 10, 0, 10, 12) },
                        { Number.N7, new Rectangle( 7  * 10, 0, 10, 12) },
                        { Number.N8, new Rectangle( 8  * 10, 0, 10, 12) },
                        { Number.N9, new Rectangle( 9  * 10, 0, 10, 12) },
                        { Number.NM, new Rectangle(10  * 10, 0, 10, 12) },
                    }
                },
                #endregion
                #region AvatarShop
                {
                    FontType.AvatarShopStatusCounter, new Dictionary<Number, Rectangle>()
                    {
                        { Number.N0, new Rectangle(0  * 8, 0, 8, 13) },
                        { Number.N1, new Rectangle(1  * 8, 0, 8, 13) },
                        { Number.N2, new Rectangle(2  * 8, 0, 8, 13) },
                        { Number.N3, new Rectangle(3  * 8, 0, 8, 13) },
                        { Number.N4, new Rectangle(4  * 8, 0, 8, 13) },
                        { Number.N5, new Rectangle(5  * 8, 0, 8, 13) },
                        { Number.N6, new Rectangle(6  * 8, 0, 8, 13) },
                        { Number.N7, new Rectangle(7  * 8, 0, 8, 13) },
                        { Number.N8, new Rectangle(8  * 8, 0, 8, 13) },
                        { Number.N9, new Rectangle(9  * 8, 0, 8, 13) },
                    }
                },
                #endregion
            };

        public float CurrentValue { get; protected set; }

        private List<Sprite> spriteList;
        private List<Sprite> unusedSpriteList;

        private readonly FontType fontType;
        private readonly TextAnchor textAnchor;
        private readonly int numericFieldLenght;

        private bool attachToCamera;
        private string stringConversionMask;

        private Vector2 position;
        public Vector2 Position { get => position; set => position = value.ToIntegerDomain(); }
        private Vector2 positionOffset;
        public Vector2 PositionOffset { get => positionOffset; set => positionOffset = value.ToIntegerDomain(); }

        public Vector2 SpriteSize => new Vector2(spriteList[0].SourceRectangle.Width, spriteList[0].SourceRectangle.Height);
        public Vector2 MeasureSize
        {
            get
            {
                Vector2 size = SpriteSize;
                return new Vector2((size.X - 2) * spriteList.Count, size.Y);
            }
        }

        public Vector2 MeasureCompleteSize
        {
            get
            {
                Vector2 size = SpriteSize;
                return new Vector2((size.X - 2) * numericFieldLenght, size.Y);
            }
        }

#if DEBUG
        //Debug
        DebugCrosshair debugCrosshair;
#endif

        //Updating Value
        public SpriteNumericField(FontType FontType, int NumericFieldLength, float LayerDepth, Vector2 Position = default,
            Vector2 PositionOffset = default, TextAnchor TextAnchor = TextAnchor.Left, int StartingValue = 0, bool AttachToCamera = true,
            bool forceRenderingAllNumbers = false)
        {
            CurrentValue = StartingValue;
            fontType = FontType;
            this.Position = Position;
            this.PositionOffset = PositionOffset;
            numericFieldLenght = NumericFieldLength;
            textAnchor = TextAnchor;
            attachToCamera = AttachToCamera;

            if (forceRenderingAllNumbers)
            {
                stringConversionMask = "";
                for (int i = 0; i < numericFieldLenght; i++)
                    stringConversionMask += "0";
            } else
            {
                stringConversionMask = "0";
            }

            spriteList = new List<Sprite>();
            unusedSpriteList = new List<Sprite>();

            for (int i = 0; i < NumericFieldLength; i++)
            {
                Sprite s = new Sprite("Interface/Spritefont/" + spritePath[FontType],
                    layerDepth: LayerDepth);
                s.Pivot = Vector2.Zero;
                spriteList.Add(s);
            }

            ReassignTextValue();

#if DEBUG
            debugCrosshair = new DebugCrosshair(Color.Green);
            DebugHandler.Instance.Add(debugCrosshair);
#endif
        }

        public void HideElement()
        {
            spriteList.Union(unusedSpriteList).ToList().ForEach((x) =>
            {
                x.HideElement();
            });
        }

        public void ShowElement()
        {
            spriteList.Union(unusedSpriteList).ToList().ForEach((x) =>
            {
                x.ShowElement();
            });
        }

        public void SetTransparency(float alpha)
        {
            foreach(Sprite s in spriteList)
            {
                s.SetTransparency(alpha);
            }
        }

        public virtual void Update(GameTime GameTime)
        {
            UpdatePosition();
            UpdateDigitsPosition();

#if DEBUG
            debugCrosshair.Update(Position);
#endif
        }

        protected void UpdatePosition()
        {
            if (!attachToCamera) return;
            Position = PositionOffset - GameScene.Camera.CameraOffset;
        }

        protected void UpdateNeededSpriteList(int NeededSprites)
        {
            while (spriteList.Count > NeededSprites)
            {
                Sprite obj = spriteList[0];
                spriteList.RemoveAt(0);
                unusedSpriteList.Add(obj);
            }

            while (spriteList.Count < NeededSprites)
            {
                Sprite obj = unusedSpriteList[0];
                unusedSpriteList.RemoveAt(0);
                spriteList.Add(obj);
            }
        }

        protected void ReassignTextValue()
        {
            string numberText = CurrentValue.ToString(stringConversionMask);

            if (numberText.Length > numericFieldLenght)
                return;

            UpdateNeededSpriteList(numberText.Length);

            //Add sign, if necessary
            int i = 0;
            if (CurrentValue < 0)
            {
                i = 1;
                spriteList[0].SourceRectangle = numericTextFieldStatePresets[fontType][Number.NM];
            }

            //Loop Through the string
            for (; i < numberText.Length; i++)
                spriteList[i].SourceRectangle = numericTextFieldStatePresets[fontType][(Number)(numberText[i] - '0')];
        }

        protected void UpdateDigitsPosition()
        {
            int i = 0;
            // Here I add up two (2) to the position because I gave 2 extra transparent
            // pixels when creating the spriteText spritesheet
            if (textAnchor == TextAnchor.Left)
            {
                spriteList.ForEach((x) =>
                {
                    x.Position = new Vector2((int)(Position.X + x.SourceRectangle.Width * i - (2 * i)), (int)Position.Y);
                    i++;
                });
            }
            else if (textAnchor == TextAnchor.Right)
            {
                Vector2 tmpStartingPosition = new Vector2((int)(Position.X - MeasureSize.X), (int)Position.Y).ToIntegerDomain();
                spriteList.ForEach((x) =>
                {
                    x.Position = tmpStartingPosition + new Vector2((x.SourceRectangle.Width * i - (2 * i)), 0);
                    i++;
                });
            }
            else if (textAnchor == TextAnchor.Middle)
            {
                Vector2 tmpStartingPosition = new Vector2((int)(Position.X - MeasureSize.X / 2), (int)Position.Y);
                spriteList.ForEach((x) =>
                {
                    x.Position = tmpStartingPosition + new Vector2((x.SourceRectangle.Width * i - (2 * i)), 0);
                    i++;
                });
            }
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            spriteList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
        }
    }

    public class NumericSpriteFont : SpriteNumericField
    {
        public NumericSpriteFont(FontType FontType, int NumericFieldLength, float LayerDepth, Vector2 Position = default,
            Vector2 PositionOffset = default, TextAnchor textAnchor = TextAnchor.Left, int StartingValue = 0, bool attachToCamera = true,
            bool forceRendingAllNumbers = false)
            : base(FontType, NumericFieldLength, LayerDepth, Position, PositionOffset, textAnchor, StartingValue, attachToCamera, forceRenderingAllNumbers: forceRendingAllNumbers)
        { }

        public void UpdateValue(int Value)
        {
            CurrentValue = Value;
        }

        public override void Update(GameTime GameTime = null)
        {
            ReassignTextValue();
            base.Update(GameTime);
        }
    }

    public class CurrencySpriteFont : SpriteNumericField
    {
        public int FinalValue { get; private set; }
        private float incrementFactor;
        private float elapsedTime;

        public CurrencySpriteFont(FontType fontType, int numericFieldLength, float layerDepth, Vector2 position = default,
            Vector2 positionOffset = default, TextAnchor textAnchor = TextAnchor.Left, int startingValue = 0, bool attachToCamera = true)
            : base(fontType, numericFieldLength, layerDepth, position, positionOffset, textAnchor, startingValue, attachToCamera)
        {
            FinalValue = startingValue;
            elapsedTime = 0;
        }

        public void AddValue(int Value)
        {
            FinalValue += Value;
            incrementFactor = (FinalValue - CurrentValue) / Parameter.InterfaceNumericTextFieldFactorNumber;
        }

        public override void Update(GameTime GameTime)
        {
            if (Math.Abs(FinalValue) != Math.Abs(CurrentValue))
            {
                elapsedTime += (float)GameTime.ElapsedGameTime.TotalSeconds;

                if (elapsedTime >= Parameter.InterfaceNumericTextFieldUpdateRate)
                {
                    elapsedTime = 0;

                    CurrentValue += incrementFactor;
                    
                    if (Math.Abs(CurrentValue) > Math.Abs(FinalValue)) CurrentValue = FinalValue;

                    ReassignTextValue();
                }
            }

            base.Update(GameTime);
        }
    }
}
