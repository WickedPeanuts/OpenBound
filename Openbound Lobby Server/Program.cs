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

using OpenBound_Lobby_Server.Common;
using OpenBound_Lobby_Server.Service;
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.FileOutput;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.TCP.ServiceProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.TCP.Entity;

namespace OpenBound_Lobby_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.LobbyServer);
            ConfigFileManager.LoadConfigFile(RequesterApplication.LobbyServer);
            LobbyServerObjects.ServerInformationList = ConfigFileManager.LoadServerlistPlaceholderFile();

            Console.WriteLine("OpenBound Lobby Server");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine($"Lobby Server ({NetworkObjectParameters.LobbyServerInformation.ServerConsoleName}) has started and is listening open for Login Server's requests.");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine($"This server must be to connected to Login Server ({NetworkObjectParameters.LoginServerInformation.ServerConsoleName}) for exchanging UID for each login attempt.");
            Console.WriteLine("Server won't register new login attempts when the LoginServer is offline.");
            Console.WriteLine("This server can be closed and re-opened at any time, but it will drop all players that requests on this sever.");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Server Responsabilities:");
            Console.WriteLine("Login Server - Handshake and grant UID to all players that requests login from Login Server");
            Console.WriteLine("Game Server  - Listen, register, and update all Game Server status");
            Console.WriteLine("Game Client  - Feed information about all registered Game Server");
            Console.WriteLine("----------------------\n");

            Console.WriteLine("----------------------");
            Console.WriteLine("Operation Log:");
            Console.WriteLine("----------------------\n");

            LobbyServerObjects.RequestedLoginPlayers = new List<Player>();
            LobbyServerObjects.ClientServiceProviderList = new List<ClientServiceProvider>();

            ServerServiceProvider serverServiceProvider = new ServerServiceProvider(
                NetworkObjectParameters.LobbyServerInformation.ServerPort,
                NetworkObjectParameters.LoginServerBufferSize,
                LobbyServiceHUB,
                onDisconnect: OnDisconnect);

            serverServiceProvider.StartOperation();
        }

        public static void LobbyServiceHUB(ExtendedConcurrentQueue<byte[]> provider, string[] request, Dictionary<int, object> paramDictionary)
        {
            int service = int.Parse(request[0]);
            object answer = null;

            switch (service)
            {
                case NetworkObjectParameters.LobbyServerPlayerUIDRequest:
                    answer = PlayerHandler.GenerateUIDForPlayerLogin(request[1]);
                    break;
                case NetworkObjectParameters.LobbyServerLoginRequest:
                    answer = PlayerHandler.LobbyLoginRequest(request[1]);
                    break;
                case NetworkObjectParameters.GameServerRegisterRequest:
                    answer = ServerInformationHandler.RefreshGameServerStatusRequest(request[1], paramDictionary);
                    break;
                case NetworkObjectParameters.LobbyServerServerListRequest:
                    ServerInformationHandler.GameServerListRequest(provider);
                    return;
            }

            provider.Enqueue(service, answer);
        }

        public static void OnDisconnect(Dictionary<int, object> paramDictionary)
        {
            if (paramDictionary.ContainsKey(NetworkObjectParameters.GameServerRegisterRequest))
            {
                ((GameServerInformation)paramDictionary[NetworkObjectParameters.GameServerRegisterRequest]).IsOnline = false;
            }
        }
    }
}
