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

using OpenBound.Common;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using OpenBound.GameComponents.MobileAction;

namespace OpenBound.ServerCommunication.Service
{
    class ServerInformationHandler
    {
        //Lobby Server Handlers
        public static void RequestServerList()
        {
            ServerInformationBroker.Instance.LobbyServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.LobbyServerServerListRequest,
                null);
        }

        #region Server List Handlers
        public static void ServerListHandle(string request)
        {
            try
            {
                GameServerInformation si = ObjectWrapper.Deserialize<GameServerInformation>(request);

                lock (GameInformation.Instance.ServerList)
                {
                    if (GameInformation.Instance.ServerList == null)
                        GameInformation.Instance.ServerList = new List<GameServerInformation>();
                    GameInformation.Instance.ServerList.Add(si);
                }
            }
            catch (Exception) { }
        }
        #endregion

        //Game Server Handlers
        #region Connection
        public static void ConnectToGameServer(GameServerInformation serverInformation)
        {
            ServerInformationBroker.Instance.ConnectToGameServer(serverInformation);

            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerPlayerAccessRequest,
                GameInformation.Instance.PlayerInformation);
        }
        #endregion

        #region GameList
        //Room
        public static void CreateRoom(RoomMetadata roomMetadata)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomListCreateRoom,
                roomMetadata
                );
        }

        public static void GameServerRequestRoomList(RoomMetadata roomMetadata)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomListRequestList,
                roomMetadata
                );
        }

        public static void ConnectToRoom(RoomMetadata roomMetadata)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomListRoomEnter,
                roomMetadata
                );
        }

        public static void SendGameListMessage(PlayerMessage message)
        {
            if (ObjectValidator.ValidateAndModify(message))
            {
#if !DEBUGSCENE
                ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                    NetworkObjectParameters.GameServerChatSendPlayerMessage,
                    message);
#endif
            }
        }

        public static void SendChatConnectionRequest(string channelID)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerChatEnter,
                channelID);
        }

        public static void SendChatDisconnectionRequest()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerChatLeave, null);
        }
        #endregion

        #region GameRoom
        public static void ChangePrimaryMobile(MobileType mobileType)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomChangePrimaryMobile,
                mobileType
                );
        }

        public static void ChangeTeam()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomChangeTeam,
                null
                );
        }

        public static void LeaveRoom()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomLeaveRoom,
                null
                );
        }

        public static void ReadyRoom()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomReadyRoom,
                null
                );
        }

        public static void ChangeMap(int mapIndex)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomChangeMap,
                mapIndex
                );
        }
        #endregion

        #region LoadingScreen
        public static void UpdateLoadingScreenPercentage(int loadingPercentage)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomRefreshLoadingPercentage,
                loadingPercentage
                );
        }

        public static void ClientReadyToStartGame()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerRoomStartInGameScene,
                null
                );
        }
        #endregion

        #region InGame
        public static void RequestNextPlayerTurn(ProjectileDamage projectileDamageDealt)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerInGameRequestNextPlayerTurn,
                null);

            if (projectileDamageDealt != null)
                ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                    NetworkObjectParameters.GameServerInGameRequestDamage,
                    projectileDamageDealt);
        }

        public static void StartMatch()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
               NetworkObjectParameters.GameServerInGameStartMatch,
               null);
        }

        public static void SynchronizeMobileStatus(SyncMobile syncMobile)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerInGameRefreshSyncMobile,
                syncMobile);
        }

        public static void SynchronizeItemUsage(SyncMobile syncMobile)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerInGameRequestItemUsage,
                syncMobile);
        }

        public static void RequestShot(SyncMobile syncMobile)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
               NetworkObjectParameters.GameServerInGameRequestShot,
               syncMobile);
        }

        public static void RequestDeath(SyncMobile syncMobile)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerInGameRequestDeath,
                syncMobile);
        }
        #endregion

        public static void AvatarShopBuyAvatarGold(AvatarMetadata avatarMetadata)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerAvatarShopBuyAvatarGold,
                avatarMetadata);
        }

        public static void RequestLoseTurn(SyncMobile syncMobile)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerInGameRequestLoseTurn, syncMobile);
        }

        public static void AvatarShopBuyAvatarCash(AvatarMetadata avatarMetadata)
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerAvatarShopBuyAvatarCash,
                avatarMetadata);
        }

        public static void AvatarShopUpdatePlayerData()
        {
            ServerInformationBroker.Instance.GameServerServiceProvider.RequestQueue.Enqueue(
                NetworkObjectParameters.GameServerAvatarShopUpdatePlayerData,
                GameInformation.Instance.PlayerInformation);
        }
    }
}
