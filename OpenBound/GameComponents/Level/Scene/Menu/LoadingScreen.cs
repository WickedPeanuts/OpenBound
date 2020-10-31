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
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Interactive.GameRoom;
using OpenBound.GameComponents.Interface.Interactive.LoadingScreen;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    public class LoadingScreen : GameScene
    {
        MatchConfigurationGrid matchConfigurationGrid;
        PlayerLoadingButtonList playerButtonList;
        List<Sprite> spriteList;
        Minimap minimap;

        public LoadingScreen()
        {
            spriteList = new List<Sprite>();
            //Background
            Background = new Sprite(@"Interface/InGame/Scene/LoadingScreen/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.LoadingScreenBackground,
                shouldCopyAsset: false);

            playerButtonList = new PlayerLoadingButtonList();

            //playerButtonList.UserLoadingButton.Mobile.Scale = new Vector2(1,1) * 70;

            minimap = new Minimap(GameInformation.Instance.RoomMetadata, Parameter.ScreenCenter + new Vector2(0, 212));

            matchConfigurationGrid = new MatchConfigurationGrid(Parameter.ScreenCenter - new Vector2(-1, 115));

            ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] += UpdateRoomMetadataAsyncCallback;
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomRefreshLoadingPercentage, UpdateLoadingPercentageAsyncCallback);
            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerRoomStartInGameScene, StartGameAsyncCallback);
        }

        #region Networking
        private void UpdateRoomMetadataAsyncCallback(object answer)
        {
            lock (playerButtonList) playerButtonList.UpdatePlayerButtonList();
        }

        private void UpdateLoadingPercentageAsyncCallback(object answer)
        {
            KeyValuePair<int, int> loadingPercentage = (KeyValuePair<int, int>)answer;

            lock (playerButtonList)
            {
                playerButtonList.UpdateLoadingPercentages(loadingPercentage);
            }
        }

        private void StartGameAsyncCallback(object answer)
        {
            ServerInformationBroker.Instance.ActionCallbackDictionary[NetworkObjectParameters.GameServerRoomRefreshMetadata] -= UpdateRoomMetadataAsyncCallback;
            SceneHandler.Instance.RequestSceneChange(SceneType.InGame, TransitionEffectType.RotatingRectangles, transitionWaitTime: 3);
        }
        #endregion

        //meow
        float accumulator = 0;
        float factor = 1.2f;
        bool hasStarted = false;
        int lastSend = 0;

        //Remove param and adjust it properly
        public void UpdateLoadingPercentage(int accumulator)
        {
            if (accumulator > 100 || accumulator == lastSend) return;

            lock (playerButtonList)
            {
                if (playerButtonList.UserLoadingButton == null)
                    return;

                playerButtonList.UserLoadingButton.UpdatePercentage(accumulator);
            }

            lastSend = accumulator;

            //if (accumulator % 5 != 0)
            ServerInformationHandler.UpdateLoadingScreenPercentage(accumulator);

            if (accumulator == 100)
            {
                ServerInformationHandler.ClientReadyToStartGame();
                hasStarted = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            lock (playerButtonList) playerButtonList.Update();

            accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds * factor * 10;

            if (!hasStarted)
                UpdateLoadingPercentage(Math.Min((int)accumulator, 100));

            minimap.Update(gameTime);
        }

        public override void Draw(GameTime GameTime)
        {
            base.Draw(GameTime);

            spriteList.ForEach((x) => x.Draw(GameTime, spriteBatch));
            minimap.Draw(GameTime, spriteBatch);
            lock (playerButtonList) playerButtonList.Draw(GameTime, spriteBatch);
            matchConfigurationGrid.Draw(GameTime, spriteBatch);

            //ServerInformationHandler.ClientReadyToStartGame();
        }
    }
}
