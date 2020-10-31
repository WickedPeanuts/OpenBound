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
using OpenBound.GameComponents.Audio;
using OpenBound.GameComponents.Interface.Builder;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.ServerSelection;
using OpenBound.ServerCommunication;
using OpenBound.ServerCommunication.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Extension;
using System.Collections.Generic;

namespace OpenBound.GameComponents.Level.Scene.Menu
{
    class ServerSelection : GameScene
    {
        public List<ServerButton> serverButtonList;

        AnimatedButton exitDoor;

        public ServerSelection()
        {
            ServerInformationHandler.RequestServerList();

            Background = new Sprite(@"Interface/InGame/Scene/ServerList/Background",
                position: Parameter.ScreenCenter,
                layerDepth: DepthParameter.Background,
                shouldCopyAsset: false);

            //Must change exit door position on gamelist as well
            exitDoor = AnimatedButtonBuilder.BuildButton(
                AnimatedButtonType.ExitDoor,
                Parameter.ScreenCenter + new Vector2(-330, 265),
                (sender) =>
                {
                    SceneHandler.Instance.CloseGame();
                });

            serverButtonList = new List<ServerButton>();

            ServerInformationBroker.Instance.ActionCallbackDictionary.AddOrReplace(NetworkObjectParameters.GameServerPlayerAccessRequest, ChangeScene);
        }

        public override void OnSceneIsActive()
        {
            base.OnSceneIsActive();

            AudioHandler.ChangeSong(SongParameter.ServerAndRoomSelection);
        }

        public void UpdateServerButtonList()
        {
            lock (GameInformation.Instance.ServerList)
            {
                foreach (GameServerInformation si in GameInformation.Instance.ServerList)
                {
                    int h = serverButtonList.Count / 2;
                    int w = serverButtonList.Count % 2 == 0 ? -1 : 1;

                    Vector2 buttonCenter = new Vector2(175 * w, 87 * h - 230);

                    serverButtonList.Add(
                        new ServerButton(si, buttonCenter,
                        (sender) =>
                        {
                            serverButtonList.ForEach((x) => x.Disable());
                            GameInformation.Instance.ConnectedServerInformation = si;
                            ServerInformationHandler.ConnectToGameServer(si);
                        }));
                }

                GameInformation.Instance.ServerList.Clear();
            }
        }

        public override void Update(GameTime GameTime)
        {
            base.Update(GameTime);

            exitDoor.Update();
            serverButtonList.ForEach((x) => x.Update());

            UpdateServerButtonList();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            exitDoor.Draw(gameTime, spriteBatch);
            serverButtonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }

        public void ChangeScene(object serverAuthStatus)
        {
            if ((bool)serverAuthStatus)
            {
                serverButtonList.ForEach((x) => x.OnClick = default);
                SceneHandler.Instance.RequestSceneChange(SceneType.GameList, TransitionEffectType.RotatingRectangles);
            }
            else
            {
                serverButtonList.ForEach((x) => x.Enable());
                ServerInformationBroker.Instance.DisconnectFromGameServer();
            }
        }
    }
}
