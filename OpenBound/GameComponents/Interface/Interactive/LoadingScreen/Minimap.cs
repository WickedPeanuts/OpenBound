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
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;

namespace OpenBound.GameComponents.Interface.Interactive.LoadingScreen
{
    public class Minimap
    {
        //
        Sprite mapBG, mapFG;
        List<Sprite> spawnPointList;

        SpriteText mapName, mapHelper;
        List<SpriteText> playerNameList;

        //Map Repositioning Variables
        Vector2 basePosition;
        Vector2 fgOffset;
        List<Vector2> spawnPointsOffset;
        float totalButtonPushTime;

        Vector2 tacticalMapDimensions;
        Vector2 miVect;

        public Minimap(RoomMetadata roomMetadata, Vector2 position)
        {
            spawnPointList = new List<Sprite>();
            playerNameList = new List<SpriteText>();

            //Map
            Map map = GameInformation.Instance.RoomMetadata.Map;
            GameMap gameMap = GameInformation.Instance.RoomMetadata.Map.GameMap;
            GameMapType type = GameInformation.Instance.RoomMetadata.Map.GameMapType;

            Vector2 frameSize = new Vector2(750, 159); //Size of the small minimap frame on the background image

            mapBG = new Sprite($"Graphics/Maps/{gameMap}/Background", layerDepth: DepthParameter.Background);
            mapBG.Position = position;

            float maxScale = Math.Max(frameSize.X / mapBG.SpriteWidth, frameSize.Y / mapBG.SpriteHeight);
            mapBG.Scale = new Vector2(maxScale, maxScale);

            //Find the very first pixel on the top of the central axis of sprite and the first on the bottom
            //In order to help centralizing (if possible) the sprite
            mapFG = new Sprite($@"Graphics/Maps/{gameMap}/Foreground{type}", layerDepth: DepthParameter.LoadingScreenMinimapForeground);
            maxScale = Math.Max(frameSize.X / mapFG.SpriteWidth, frameSize.Y / mapFG.SpriteHeight);
            mapFG.Scale = new Vector2(maxScale, maxScale);
            basePosition = mapFG.Position = position;

            //Spawn Points
            spawnPointsOffset = new List<Vector2>();
            foreach (KeyValuePair<int, int[]> playerPos in roomMetadata.SpawnPositions.ToList())
            {
                Player player = roomMetadata.TeamASafe.Find((y) => y.ID == playerPos.Key);

                int box = 1;
                Color nameColor = Parameter.TextColorTeamRed;

                if (player == null)
                {
                    player = roomMetadata.TeamBSafe.Find((y) => y.ID == playerPos.Key);

                    box = 2;
                    nameColor = Parameter.TextColorTeamBlue;
                }

                Vector2 boxPosition = mapFG.Position + (new Vector2(playerPos.Value[0], playerPos.Value[1]) - mapFG.Pivot) * mapFG.Scale;

                Sprite boxSprite = new Sprite("Interface/InGame/Scene/LoadingScreen/PlayerMapMarker",
                    position: boxPosition, layerDepth: DepthParameter.LoadingScreenMinimapSpawnPointBox,
                    sourceRectangle: new Rectangle(14 * box, 0, 14, 14))
                {
                    Pivot = new Vector2(7, 7)
                };

                spawnPointList.Add(boxSprite);
                spawnPointsOffset.Add(boxPosition - mapFG.Position);

                playerNameList.Add(new SpriteText(
                        FontTextType.NotoSans10Family, player.Nickname,
                        nameColor, Alignment.Center,
                        layerDepth: DepthParameter.LoadingScreenMinimapSpawnPointText, boxPosition - new Vector2(0, 20), Color.Black));
            }

            //Filter remaining spawn points and add them to remainingSpawnPoints, then add the white square sprite
            List<int[]> remainingSpawnPositions = new List<int[]>();
            foreach (int[] spawnPoints in map.SpawnPoints)
            {
                if (roomMetadata.SpawnPositions.Values.ToList().Find((x) => x[0] == spawnPoints[0] && x[1] == spawnPoints[1]) == null)
                {
                    remainingSpawnPositions.Add(spawnPoints);
                }
            }
            
            foreach(int[] spawnPosition in remainingSpawnPositions)
            {
                Vector2 boxOffset = (new Vector2(spawnPosition[0], spawnPosition[1]) - mapFG.Pivot) * mapFG.Scale;
                Sprite boxSprite = new Sprite("Interface/InGame/Scene/LoadingScreen/PlayerMapMarker",
                   position: mapFG.Position + boxOffset,
                   layerDepth: DepthParameter.LoadingScreenMinimapSpawnPointBox,
                   sourceRectangle: new Rectangle(0, 0, 14, 14))
                {
                    Pivot = new Vector2(7, 7)
                };

                spawnPointList.Add(boxSprite);
                spawnPointsOffset.Add(boxOffset);
            };

            mapName = new SpriteText(FontTextType.Consolas11, roomMetadata.Map.Name,
                Color.White, Alignment.Left, DepthParameter.InterfaceButton,
                position + new Vector2(-365, -72), Color.Black);

            mapHelper = new SpriteText(FontTextType.Consolas11, Language.LoadingScreenMinimapCommand, Color.White, Alignment.Right, DepthParameter.InterfaceButton, position + new Vector2(365, 67), Color.Black);

            UpdateElementPositions(Vector2.Zero);
            totalButtonPushTime = 0;

            //TacticalMap
            tacticalMapDimensions = new Vector2(750, 159) / 2f;
            miVect = mapFG.Pivot * mapFG.Scale;

            Vector2 playerPosition = roomMetadata.SpawnPositions[GameInformation.Instance.PlayerInformation.ID].ToVector2() * mapFG.Scale;

            UpdateElementPositions((miVect - playerPosition) * Vector2.UnitY);
        }

        public void UpdateElementPositions(Vector2 offset)
        {
            fgOffset += offset;

            fgOffset = new Vector2(fgOffset.X,
                MathHelper.Clamp(fgOffset.Y, -miVect.Y + tacticalMapDimensions.Y + 10, miVect.Y - tacticalMapDimensions.Y - 10));

            //Repositioning Elements
            mapFG.Position = basePosition + fgOffset;
            for (int i = 0; i < spawnPointList.Count; i++)
            {
                Vector2 newPos = basePosition + fgOffset + spawnPointsOffset[i];
                if (i < playerNameList.Count)
                {
                    spawnPointList[i].Position = new Vector2(newPos.X, MathHelper.Clamp(newPos.Y, basePosition.Y - 55, basePosition.Y + 78));
                    playerNameList[i].Position = spawnPointList[i].Position - new Vector2(0, 20);
                }
                else
                {
                    spawnPointList[i].Position = new Vector2(newPos.X, MathHelper.Clamp(newPos.Y, basePosition.Y - 70, basePosition.Y + 78));
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Vector2 offset = Vector2.UnitY;

            //Calculate offset
            if (InputHandler.IsBeingHeldDown(Keys.Up))
                offset *= Parameter.InterfaceMinimapScrollSpeed;
            else if (InputHandler.IsBeingHeldDown(Keys.Down))
                offset *= -Parameter.InterfaceMinimapScrollSpeed;
            else
                totalButtonPushTime = 0;

            if (offset == Vector2.UnitY) return;

            totalButtonPushTime = Math.Min(0.5f, totalButtonPushTime + (float)gameTime.ElapsedGameTime.TotalSeconds);

            offset *= totalButtonPushTime;

            UpdateElementPositions(offset);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spawnPointList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            playerNameList.ForEach((x) => x.Draw(spriteBatch));
            mapBG.Draw(null, spriteBatch);
            mapFG.Draw(null, spriteBatch);
            mapHelper.Draw(spriteBatch);
            mapName.Draw(spriteBatch);
        }
    }
}
