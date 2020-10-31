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
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Interface.Interactive.GameRoom
{
    class MatchConfigurationGrid
    {
        protected static Dictionary<GameMode, Rectangle> GameModePreset
            = new Dictionary<GameMode, Rectangle>()
            {
                { GameMode.Solo,  new Rectangle(242 * 0, 22 * 0, 242, 22) },
                { GameMode.Score, new Rectangle(242 * 1, 22 * 0, 242, 22) },
                { GameMode.Tag,   new Rectangle(242 * 2, 22 * 0, 242, 22) },
                { GameMode.Jewel, new Rectangle(242 * 3, 22 * 0, 242, 22) },
            };

        protected static Dictionary<RoomSize, Rectangle> RoomSizePreset
            = new Dictionary<RoomSize, Rectangle>()
            {
                { RoomSize.OneVsOne,     new Rectangle(242 * 0, 22 * 1, 242, 22) },
                { RoomSize.TwoVsTwo,     new Rectangle(242 * 1, 22 * 1, 242, 22) },
                { RoomSize.ThreeVsThree, new Rectangle(242 * 2, 22 * 1, 242, 22) },
                { RoomSize.FourVsFour,   new Rectangle(242 * 3, 22 * 1, 242, 22) },
            };

        protected static Dictionary<GameMapType, Rectangle> GameMapTypePreset
            = new Dictionary<GameMapType, Rectangle>()
            {
                { GameMapType.A, new Rectangle(242 * 0, 22 * 2, 242, 22) },
                { GameMapType.B, new Rectangle(242 * 1, 22 * 2, 242, 22) },
                { GameMapType.C, new Rectangle(242 * 2, 22 * 2, 242, 22) },
                { GameMapType.D, new Rectangle(242 * 3, 22 * 2, 242, 22) },
            };

        protected static Dictionary<SlotModeType, Rectangle> SlotModeTypePreset
            = new Dictionary<SlotModeType, Rectangle>()
            {
                { SlotModeType.Basic,  new Rectangle(242 * 0, 22 * 3, 242, 22) },
                { SlotModeType.Attack, new Rectangle(242 * 1, 22 * 3, 242, 22) },
            };

        protected static Dictionary<MatchSuddenDeathType, Rectangle> MatchSuddenDeathPreset
            = new Dictionary<MatchSuddenDeathType, Rectangle>()
            {
                { MatchSuddenDeathType.NoDeath,      new Rectangle(242 * 2, 22 * 3, 242, 22) },
                { MatchSuddenDeathType.BigBombDeath, new Rectangle(242 * 3, 22 * 3, 242, 22) },
                { MatchSuddenDeathType.DoubleDeath,  new Rectangle(242 * 0, 22 * 4, 242, 22) },
                { MatchSuddenDeathType.SSDeath,      new Rectangle(242 * 1, 22 * 4, 242, 22) },
            };

        protected static Dictionary<TurnsToSuddenDeath, Rectangle> TurnsToSuddenDeathPreset
            = new Dictionary<TurnsToSuddenDeath, Rectangle>()
            {
                { TurnsToSuddenDeath.Turn40, new Rectangle(242 * 2, 22 * 4, 242, 22) },
                { TurnsToSuddenDeath.Turn56, new Rectangle(242 * 3, 22 * 4, 242, 22) },
                { TurnsToSuddenDeath.Turn72, new Rectangle(242 * 0, 22 * 5, 242, 22) },
            };

        List<Sprite> spriteList;
        List<Sprite> interrogationSpriteList;
        List<NumericSpriteFont> numericSpriteFontList;
        List<SpriteText> spriteTextList;

        Sprite mapThumbnail;
        Vector2 basePosition;

        public MatchConfigurationGrid(Vector2 Position)
        {
            basePosition = Position;

            spriteList = new List<Sprite>();
            interrogationSpriteList = new List<Sprite>();

            numericSpriteFontList = new List<NumericSpriteFont>();
            spriteTextList = new List<SpriteText>();

            RoomMetadata roomMetadata = GameInformation.Instance.RoomMetadata;

            Vector2 sprSize = new Vector2(242, 22);

            Sprite sp;

            //Room configuration description
            for (int i = 0; i < 6; i++)
            {
                sp = new Sprite(
                    "Interface/InGame/Scene/GameRoom/Information",
                    position: Position + new Vector2(0, (sprSize.Y - 2) * i),
                    layerDepth: DepthParameter.InterfaceButton)
                { Pivot = sprSize / 2f };

                spriteList.Add(sp);
            }

            //Map Weather
            for (int i = 0; i < roomMetadata.Map.WeatherPreset.Length; i++)
            {
                Vector2 lineOffset;

                if (i < 8)
                    lineOffset = Position - new Vector2(90, 131) + new Vector2(26, 0) * i;
                else
                    lineOffset = numericSpriteFontList[i - 5].Position + new Vector2(0, 20);

                numericSpriteFontList.Add(
                    new NumericSpriteFont(FontType.GameRoomPingCounterWhite, 1,
                    DepthParameter.InterfaceButton,
                    Position: lineOffset,
                    attachToCamera: false));

                interrogationSpriteList.Add(new Sprite("Interface/Spritefont/GameRoom/Interrogation",
                    position: lineOffset + new Vector2(4, 4),
                    layerDepth: DepthParameter.InterfaceButton));
            }

            //Texts
            spriteTextList.Add(new SpriteText(
                FontTextType.Consolas11, "",
                Color.White, Alignment.Center,
                DepthParameter.InterfaceButtonText,
                Position - new Vector2(0, 45), Color.Black));

            spriteTextList.Add(new SpriteText(
                FontTextType.Consolas11, "",
                Color.White, Alignment.Left,
                DepthParameter.InterfaceButtonText,
                Position - new Vector2(85, 158), Color.Black));

            UpdateStatusMenu();
        }

        public void UpdateStatusMenu()
        {
            RoomMetadata roomMetadata = GameInformation.Instance.RoomMetadata;

            spriteList[0].SourceRectangle = GameModePreset[roomMetadata.GameMode];
            spriteList[1].SourceRectangle = RoomSizePreset[roomMetadata.Size];
            spriteList[2].SourceRectangle = GameMapTypePreset[roomMetadata.Map.GameMapType];
            spriteList[3].SourceRectangle = SlotModeTypePreset[roomMetadata.SlotModeType];
            spriteList[4].SourceRectangle = MatchSuddenDeathPreset[roomMetadata.MatchSuddenDeathType];
            spriteList[5].SourceRectangle = TurnsToSuddenDeathPreset[roomMetadata.TurnsToSuddenDeath];

            //Minimap icon
            GameMap map = GameInformation.Instance.RoomMetadata.Map.GameMap;
            GameMapType type = GameInformation.Instance.RoomMetadata.Map.GameMapType;

            mapThumbnail = new Sprite($"Graphics/Maps/{map}/GameRoomThumb{type}", layerDepth: DepthParameter.WeatherEffect);
            mapThumbnail.Position = basePosition - new Vector2(1, 78f);

            //Maps Weather
            //if (matchMetadata.Map.GameMap == GameMap.Random)
            //but since I dont know the right information about all the maps
            //i'll keep it simple
            if (roomMetadata.Map.Force == -1)
            {
                numericSpriteFontList.ForEach((x) => x.HideElement());
                interrogationSpriteList.ForEach((x) => x.ShowElement());
            }
            else
            {
                numericSpriteFontList.ForEach((x) => x.ShowElement());
                interrogationSpriteList.ForEach((x) => x.HideElement());
            }

            for (int i = 0; i < roomMetadata.Map.WeatherPreset.Length; i++)
            {
                numericSpriteFontList[i].UpdateValue(roomMetadata.Map.WeatherPreset[i]);
                numericSpriteFontList[i].Update();
            }

            //MapName
            spriteTextList[0].Text = roomMetadata.Map.Name;
            spriteTextList[1].Text = $"{roomMetadata.ID} {roomMetadata.Name}";
            numericSpriteFontList.ForEach((x) => x.Update());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            numericSpriteFontList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            interrogationSpriteList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
            mapThumbnail.Draw(null, spriteBatch);
        }
    }
}
