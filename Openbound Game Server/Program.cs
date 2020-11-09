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

using OpenBound_Game_Server.Common;
using OpenBound_Game_Server.Server;
using OpenBound_Game_Server.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.FileManagement;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.TCP.ServiceProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.TCP.Entity;
using OpenBound_Network_Object_Library.FileManager;

namespace OpenBound_Game_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.GameServer);
            ConfigFileManager.LoadConfigFile(RequesterApplication.GameServer);

            Console.WriteLine("OpenBound Game Server");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine($"Game Server ({NetworkObjectParameters.GameServerInformation.ServerConsoleName}) has started and is listening for Clients.");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("This server is meant to handle all the current rooms and matches, it's max capacity defines how many players can access it.");
            Console.WriteLine("This server must register all matchs and results on the database");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Server Responsabilities:");
            Console.WriteLine("Lobby Server - Register and update its current status to LobbyServer.");
            Console.WriteLine("Game Client  - Synchronize its objects to all connected clients");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Operation Log:");
            Console.WriteLine("----------------------\n");

            GameServerObjects.serverServiceProvider = new ServerServiceProvider(
                NetworkObjectParameters.GameServerInformation.ServerPort,
                NetworkObjectParameters.GameServerBufferSize,
                GameServiceHUB,
                onDisconnect: OnDisconnect);

            GameServerObjects.serverServiceProvider.StartOperation();

            GameServerObjects.lobbyServerCSP = new ClientServiceProvider(
                NetworkObjectParameters.LobbyServerInformation.ServerLocalAddress,
                NetworkObjectParameters.LobbyServerInformation.ServerPort,
                NetworkObjectParameters.LobbyServerBufferSize,
                LobbyServiceConsumerAction);

            GameServerObjects.lobbyServerCSP.StartOperation();

            GameServerObjects.lobbyServerCSP.RequestQueue.Enqueue(NetworkObjectParameters.GameServerRegisterRequest, NetworkObjectParameters.GameServerInformation);
        }

        public static void LobbyServiceConsumerAction(ClientServiceProvider csp, string[] response)
        {
            int service = int.Parse(response[0]);

            switch (service)
            {
                case NetworkObjectParameters.GameServerRegisterRequest:
                    LobbyServerHandler.GameServerRegister(response[1]);
                    break;
            }
        }

        public static void GameServiceHUB(ExtendedConcurrentQueue<byte[]> provider, string[] request, Dictionary<int, object> paramDictionary)
        {
            int service;
            object answer = null;

            service = (request != null) ? int.Parse(request[0])
                : NetworkObjectParameters.ServerProcessingError;

            PlayerSession playerSession = null;
            if (paramDictionary.ContainsKey(NetworkObjectParameters.PlayerSession))
                playerSession = (PlayerSession)paramDictionary[NetworkObjectParameters.PlayerSession];

            switch (service)
            {
                // Server Information
                case NetworkObjectParameters.GameServerMetadataRequest:
                    answer = NetworkObjectParameters.GameServerInformation;
                    break;

                // Connection
                case NetworkObjectParameters.GameServerPlayerAccessRequest:
                    answer = GameClientProvider.GameServerPlayerAccessRequest(request[1], paramDictionary, provider);
                    break;

                // Server Information Request
                case NetworkObjectParameters.GameServerSearchPlayer:
                    GameClientProvider.GameServerSearchPlayer(request[1]);
                    break;

                // Game List / Requests
                case NetworkObjectParameters.GameServerRoomListCreateRoom:
                    answer = GameClientProvider.GameServerRoomListCreateRoom(request[1], playerSession);
                    break;
                case NetworkObjectParameters.GameServerRoomListRequestList:
                    GameClientProvider.GameServerRoomListRequestList(request[1], provider);
                    return;
                case NetworkObjectParameters.GameServerRoomListRoomEnter:
                    answer = GameClientProvider.GameServerRoomListRoomEnter(request[1], playerSession);
                    break;

                // Game Room / Requests
                case NetworkObjectParameters.GameServerRoomLeaveRoom:
                    answer = GameClientProvider.GameServerRoomLeaveRoom(playerSession);
                    break;
                case NetworkObjectParameters.GameServerRoomReadyRoom:
                    GameClientProvider.GameServerRoomReadyRoom(playerSession);
                    return;
                case NetworkObjectParameters.GameServerRoomChangePrimaryMobile:
                    GameClientProvider.GameServerRoomChangePrimaryMobile(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerRoomChangeTeam:
                    GameClientProvider.GameServerRoomChangeTeam(playerSession);
                    return;
                case NetworkObjectParameters.GameServerRoomChangeMap:
                    GameClientProvider.GameServerRoomChangeMap(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerRoomRefreshLoadingPercentage:
                    GameClientProvider.GameServerRoomRefreshLoadingPercentage(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerRoomStartInGameScene:
                    GameClientProvider.GameServerRoomStartInGameScene(playerSession);
                    return;

                // InGame / Requests
                case NetworkObjectParameters.GameServerInGameStartMatch:
                    GameClientProvider.GameServerInGameStartMatch(playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRefreshSyncMobile:
                    GameClientProvider.GameServerInGameRefreshSyncMobile(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRequestNextPlayerTurn:
                    answer = GameClientProvider.GameServerInGameRequestNextPlayerTurn(playerSession);
                    break;
                case NetworkObjectParameters.GameServerInGameRequestShot:
                    GameClientProvider.GameServerInGameRequestShot(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRequestDeath:
                    GameClientProvider.GameServerInGameRequestDeath(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRequestItemUsage:
                    GameClientProvider.GameServerInGameRequestItemUsage(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRequestDamage:
                    GameClientProvider.GameServerInGameRequestDamage(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerInGameRequestLoseTurn:
                    GameClientProvider.GameServerInGameRequestLoseTurn(request[1], playerSession);
                    return;

                // Messaging / Room List Chat Requests
                case NetworkObjectParameters.GameServerChatEnter:
                    GameClientProvider.GameServerChatEnterRequest(request[1], playerSession);
                    return;
                case NetworkObjectParameters.GameServerChatLeave:
                    GameClientProvider.GameServerChatLeave(playerSession);
                    return;
                case NetworkObjectParameters.GameServerChatSendPlayerMessage:
                    GameClientProvider.GameServerChatRoomSendMessage(request[1], playerSession);
                    return;

                // Avatar Shop
                case NetworkObjectParameters.GameServerAvatarShopBuyAvatarGold:
                    answer = GameClientProvider.GameServerAvatarShopBuyAvatar(request[1], playerSession, PaymentMethod.Gold);
                    break;
                case NetworkObjectParameters.GameServerAvatarShopBuyAvatarCash:
                    answer = GameClientProvider.GameServerAvatarShopBuyAvatar(request[1], playerSession, PaymentMethod.Cash);
                    break;
                case NetworkObjectParameters.GameServerAvatarShopUpdatePlayerData:
                    GameClientProvider.GameServerAvatarShopUpdatePlayerMetadata(request[1], playerSession);
                    return;
            }

            provider.Enqueue(service, answer);
        }

        public static void OnDisconnect(Dictionary<int, object> paramDictionary)
        {
            PlayerSession playerSession = null;
            if (paramDictionary.ContainsKey(NetworkObjectParameters.PlayerSession))
                playerSession = (PlayerSession)paramDictionary[NetworkObjectParameters.PlayerSession];

            if (playerSession == null) return;

            lock (NetworkObjectParameters.GameServerInformation)
                NetworkObjectParameters.GameServerInformation.ConnectedClients = Math.Max(0, NetworkObjectParameters.GameServerInformation.ConnectedClients - 1);

            //Remove player from game files
            if (playerSession.Player != null)
            {
                lock (GameServerObjects.Instance.PlayerHashtable)
                {
                    if (GameServerObjects.Instance.PlayerHashtable.ContainsKey(playerSession.Player.ID))
                        GameServerObjects.Instance.PlayerHashtable.Remove(playerSession.Player.ID);

                    Console.WriteLine($"- {playerSession.Player.Nickname} is now disconnected from the game server.");
                }
            }

            //Disconnects from any connected chat
            GameClientProvider.GameServerChatLeave(playerSession);

            //Remove player from loading screen & start match (if necessary)
            if (playerSession.Player.PlayerNavigation == PlayerNavigation.InLoadingScreen)
                GameClientProvider.GameServerRoomRequestDisconnect(playerSession);

            //Remove player from match
            if (playerSession.Player.PlayerNavigation == PlayerNavigation.InGame)
                GameClientProvider.GameServerInGameRequestDisconnect(playerSession);

            //Remove player from room
            if (playerSession.RoomMetadata != null)
                GameClientProvider.GameServerRoomLeaveRoom(playerSession);
        }
    }
}
