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
using OpenBound_Network_Object_Library.Security;
using System;
using System.Threading;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound_Lobby_Server.Service
{
    public class PlayerHandler
    {
        public static Player GenerateUIDForPlayerLogin(string param)
        {
            try
            {
                Player player = ObjectWrapper.Deserialize<Player>(param);

                lock (LobbyServerObjects.RequestedLoginPlayers)
                {
                    Player previousRequestedPlayer = LobbyServerObjects.RequestedLoginPlayers.Find((x) => x.ID == player.ID);
                    if (previousRequestedPlayer == null)
                    {
                        //If there is no player registered on the login server, register a new one with a new security token
                        LobbyServerObjects.RequestedLoginPlayers.Add(player);
                        previousRequestedPlayer = player;
                    }

                    //If there is already a player but somehow he couldn't get in, re-roll another token for him
                    previousRequestedPlayer.SecurityToken = SecurityTokenHandler.GenerateSecurityToken();

                    Console.WriteLine($"- Security Token Created for {previousRequestedPlayer.Nickname} - {previousRequestedPlayer.SecurityToken.Token} - {previousRequestedPlayer.SecurityToken.DateTime.TimeOfDay.ToString().Split('.')[0]} - {previousRequestedPlayer.SecurityToken.UnifiedSecurityToken}");

                    //Timebomb thread to remove the player on the list after 2 minutes in order to optimize the searches
                    new Thread(() => DeleteRequestedLoginPlayer(previousRequestedPlayer)).Start();

                    return previousRequestedPlayer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool LobbyLoginRequest(string param)
        {
            try
            {
                Player player = ObjectWrapper.Deserialize<Player>(param);

                lock (LobbyServerObjects.RequestedLoginPlayers)
                {
                    if (!LobbyServerObjects.RequestedLoginPlayers
                        .Exists((x) => x.ID == player.ID && x.SecurityToken == player.SecurityToken))
                    { return false; }

                    LobbyServerObjects.RequestedLoginPlayers.RemoveAll((x) => x.ID == player.ID);
                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }

        public static void DeleteRequestedLoginPlayer(Player Player)
        {
            Thread.Sleep(1000 * 60 * NetworkObjectParameters.SecurityTokenExpirationInMinutes);

            if (!SecurityTokenHandler.IsTokenDateValid(Player.SecurityToken))
            {
                lock (LobbyServerObjects.RequestedLoginPlayers)
                {
                    LobbyServerObjects.RequestedLoginPlayers.Remove(Player);
                }
            }
        }
    }
}
