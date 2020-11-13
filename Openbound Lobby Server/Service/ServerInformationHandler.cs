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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Extension;
using OpenBound_Network_Object_Library.TCP.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound_Lobby_Server.Service
{
    class ServerInformationHandler
    {
        public static bool RefreshGameServerStatusRequest(string param, Dictionary<int, object> paramDictionary)
        {
            try
            {
                GameServerInformation information = ObjectWrapper.Deserialize<GameServerInformation>(param);

                if (!NetworkObjectParameters.GameServerRequestIPWhitelist.Contains(information.ServerPublicAddress))
                {
                    Console.WriteLine(
                        $"- {information.ServerConsoleName} is attempting to register but it is not on the whitelist.\n" +
                        $"Whitelisted servers: {string.Join(", ", NetworkObjectParameters.GameServerRequestIPWhitelist)}");

                    return false;
                }

                lock (LobbyServerObjects.ServerInformationList)
                {
                    GameServerInformation si = LobbyServerObjects.ServerInformationList.Find((x) => x.ServerID == information.ServerID);

                    if (si == null)
                    {
                        LobbyServerObjects.ServerInformationList.Add(information);
                    }
                    else
                    {
                        LobbyServerObjects.ServerInformationList.Remove(si);
                        LobbyServerObjects.ServerInformationList.Add(information);
                    }

                    information.IsOnline = true;

                    LobbyServerObjects.ServerInformationList = LobbyServerObjects.ServerInformationList.OrderBy((x) => x.ServerID).ToList();

                    paramDictionary.Add(NetworkObjectParameters.GameServerRegisterRequest, information);
                }

                Console.WriteLine($"- {information.ServerName}:{information.ServerPublicAddress}:{information.ServerPort} refreshed its status. ");

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GameServerListRequest(ExtendedConcurrentQueue<byte[]> provider)
        {
            try
            {
                lock (LobbyServerObjects.ServerInformationList)
                {
                    foreach (GameServerInformation si in LobbyServerObjects.ServerInformationList)
                    {
                        provider.Enqueue(NetworkObjectParameters.LobbyServerServerListRequest, si);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
