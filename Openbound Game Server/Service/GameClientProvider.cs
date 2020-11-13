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
using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Database.Context;
using OpenBound_Network_Object_Library.Database.Controller;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using OpenBound_Network_Object_Library.TCP.Entity;

namespace OpenBound_Game_Server.Service
{
    class GameClientProvider
    {
        #region Connection
        public static bool GameServerPlayerAccessRequest(string param, Dictionary<int, object> paramDictionary, ExtendedConcurrentQueue<byte[]> provider)
        {
            try
            {
                Player player = ObjectWrapper.Deserialize<Player>(param);
                lock (NetworkObjectParameters.GameServerInformation)
                {
                    if (NetworkObjectParameters.GameServerInformation.ConnectedClients + 1 > NetworkObjectParameters.GameServerInformation.ConnectedClientCapacity)
                        return false;

                    lock (GameServerObjects.Instance.PlayerHashtable)
                    {
                        if (GameServerObjects.Instance.PlayerHashtable.ContainsKey(player.ID))
                            return false;
                        else
                        {
                            //Retrieve Player from database since player's connection request cant be trusted
                            //Remember that I cant trust player ID either

                            using (OpenBoundDatabaseContext context = new OpenBoundDatabaseContext())
                            {
                                player = context.Players
                                    .Include(p => p.Guild)
                                    .Where((x) => x.ID == player.ID)
                                    .FirstOrDefault();

                                player.Password = null;
                            }

                            if (player == null)
                                throw new Exception();

                            PlayerSession pS = new PlayerSession() { Player = player, ProviderQueue = provider };
                            paramDictionary.Add(NetworkObjectParameters.PlayerSession, pS);
                            GameServerObjects.Instance.PlayerHashtable.Add(player.ID, pS);
                            NetworkObjectParameters.GameServerInformation.ConnectedClients++;
                            //GameServerObjects.lobbyServerCSP.RequestQueue.Enqueue(NetworkObjectParameters.GameServerRegisterRequest, NetworkObjectParameters.GameServerInformation);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return false;
        }
        #endregion

        #region Server Information Request
        public static bool GameServerSearchPlayer(string param)
        {
            try
            {
                Player player = ObjectWrapper.Deserialize<Player>(param);
                lock (GameServerObjects.Instance.PlayerHashtable)
                {
                    return GameServerObjects.Instance.PlayerHashtable.ContainsKey(player.ID);
                }
            }
            catch (Exception) { }

            return false;
        }
        #endregion

        #region Game List / Requests 
        public static RoomMetadata GameServerRoomListCreateRoom(string param, PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = ObjectWrapper.Deserialize<RoomMetadata>(param);

                lock (GameServerObjects.Instance.RoomMetadataSortedList)
                {
                    //Look for the first available room id;
                    room.ID = 1;
                    while (GameServerObjects.Instance.RoomMetadataSortedList.ContainsKey(room.ID)) room.ID++;

                    //Remove the player that was already addded in the serialization proccess
                    room.RemovePlayer(playerSession.Player);
                    room.AddA(playerSession.Player);

                    //Create room and chat
                    GameServerObjects.Instance.CreateRoom(room);
                }

                playerSession.Player.PlayerNavigation = PlayerNavigation.InGameRoom;
                playerSession.Player.PlayerRoomStatus = PlayerRoomStatus.Master;
                playerSession.RoomMetadata = room;
                playerSession.RoomMetadata.RoomOwner = playerSession.Player;

                Console.WriteLine($"- {room.RoomOwner.Nickname} created a room ({room.ID} - {room.Name})");

                return room;
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }

            return null;
        }

        public static void GameServerRoomListRequestList(string param, ExtendedConcurrentQueue<byte[]> provider)
        {
            try
            {
                RoomMetadata filter = ObjectWrapper.Deserialize<RoomMetadata>(param);

                lock (GameServerObjects.Instance.RoomMetadataSortedList)
                {
                    var query = GameServerObjects.Instance.RoomMetadataSortedList.AsEnumerable();

                    if (filter.GameMode != GameMode.Any)
                        query = query.Where((x) => x.Value.GameMode == filter.GameMode);

                    if (!filter.IsPlaying)
                        query = query.Where((x) => !x.Value.IsPlaying);

                    //The last filter should be by page
                    query = query.Skip(9 * filter.PageNumber).Take(10);

                    query.ToList().ForEach((room) =>
                    {
                        provider.Enqueue(NetworkObjectParameters.GameServerRoomListRequestList, room.Value);
                    });
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        public static RoomMetadata GameServerRoomListRoomEnter(string param, PlayerSession playerSession)
        {
            try
            {
                RoomMetadata filter = ObjectWrapper.Deserialize<RoomMetadata>(param);

                lock (GameServerObjects.Instance.RoomMetadataSortedList)
                {
                    RoomMetadata room = GameServerObjects.Instance.RoomMetadataSortedList[filter.ID];

                    lock (room)
                    {
                        List<Player> roomUnion = room.PlayerList;

                        //if the room is full, refuse returning null
                        if (room.IsPlaying || room.NumberOfPlayers == (int)room.Size) return null;

                        //insert the player on the lowest numbered team
                        if (room.TeamASafe.Count() <= room.TeamBSafe.Count())
                            room.AddA(playerSession.Player);
                        else
                            room.AddB(playerSession.Player);

                        playerSession.Player.PlayerLoadingStatus = PlayerRoomStatus.NotReady;
                        playerSession.Player.PlayerRoomStatus = PlayerRoomStatus.NotReady;
                        playerSession.Player.PlayerNavigation = PlayerNavigation.InGameRoom;

                        playerSession.RoomMetadata = room;

                        //send an update for each member of the match with the current metadata
                        BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, roomUnion);
                    }

                    Console.WriteLine($" - {playerSession.Player.Nickname} joined the room ({room.ID} - {room.Name})");

                    return room;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }

            //case things goes bad, return null to the request user
            return null;
        }
        #endregion

        #region Game Room / Requests
        public static bool GameServerRoomLeaveRoom(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;
                Player player = playerSession.Player;

                lock (GameServerObjects.Instance.RoomMetadataSortedList)
                {
                    //remove the player from each possible team
                    lock (room)
                    {
                        room.RemovePlayer(player);

                        playerSession.RoomMetadata = null;
                        playerSession.Player.PlayerNavigation = PlayerNavigation.InGameMenus;

                        Console.WriteLine($" - {player.Nickname} left the room ({room.ID} - {room.Name})");

                        if (room.NumberOfPlayers == 0)
                        {
                            GameServerObjects.Instance.RoomMetadataSortedList.Remove(room.ID);
                            Console.WriteLine($" - A room was destroyed ({room.ID} - {room.Name})");
                            return true;
                        }

                        List<Player> roomUnion = room.PlayerList;

                        if (player.PlayerRoomStatus == PlayerRoomStatus.Master)
                        {
                            Player newMaster = roomUnion.First();

                            newMaster.PlayerRoomStatus = PlayerRoomStatus.Master;
                            room.RoomOwner = newMaster;
                        }

                        //Connect to room chat
                        GameServerChatLeave(playerSession);

                        //send an update for each member of the match with the current metadata
                        BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, roomUnion);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }

            return false;
        }

        public static void GameServerRoomChangePrimaryMobile(string param, PlayerSession playerSession)
        {
            try
            {
                MobileType mobileType = ObjectWrapper.Deserialize<MobileType>(param);

                RoomMetadata room = playerSession.RoomMetadata;

                lock (room)
                {
                    playerSession.Player.PrimaryMobile = mobileType;
                    BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, room.PlayerList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerRoomReadyRoom(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;
                Player player = playerSession.Player;

                lock (room)
                {
                    List<Player> roomUnion = room.PlayerList;

                    if (player.ID == room.RoomOwner.ID)
                    {
                        int readyPlayers = roomUnion.Where((x) => x.PlayerRoomStatus == PlayerRoomStatus.Ready).Count();

                        #if DEBUG
                        if (/*readyPlayers + 1 == room.NumberOfPlayers &&
                            room.TeamA.Count() == room.TeamB.Count()*/true)
                        #else
                        if (readyPlayers + 1 == room.NumberOfPlayers &&
                            room.TeamA.Count() == room.TeamB.Count())
                        #endif
                        {
                            Console.WriteLine($" - Room ({room.ID} - {room.Name}) has started!");

                            //Sets state of each player to be used in the "Loading Complete" method, also resets the room status to "not ready" in case the game goes back to the room screen.
                            roomUnion.ForEach((x) =>
                            {
                                x.PlayerLoadingStatus = PlayerRoomStatus.NotReady;
                                x.PlayerRoomStatus = PlayerRoomStatus.NotReady;
                                x.PlayerNavigation = PlayerNavigation.InLoadingScreen;
                            });

                            //Resets the RoomOwner status to master.
                            room.RoomOwner.PlayerRoomStatus = PlayerRoomStatus.Master;

                            //Prepare match
                            room.StartMatch();

                            BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, roomUnion);
                            BroadcastToPlayer(NetworkObjectParameters.GameServerRoomReadyRoom, null, roomUnion);
                        }
                    }
                    else
                    {
                        player.PlayerRoomStatus = (player.PlayerRoomStatus == PlayerRoomStatus.NotReady) ?
                            PlayerRoomStatus.Ready : PlayerRoomStatus.NotReady;
                    }

                    Console.WriteLine($" - {player.Nickname} on ({room.ID} - {room.Name}) is {player.PlayerRoomStatus}");

                    //send an update for each member of the match with the current metadata
                    BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, roomUnion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        internal static void GameServerRoomChangeTeam(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;
                Player player = playerSession.Player;

                lock (room)
                {
                    if (room.TeamA.Contains(player) && room.TeamB.Count < 4)
                    {
                        room.RemovePlayer(player);
                        room.AddB(player);
                    }
                    else if (room.TeamB.Contains(player) && room.TeamA.Count < 4)
                    {
                        room.RemovePlayer(player);
                        room.AddA(player);
                    }

                    //send an update for each member of the match with the current metadata
                    BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, room.PlayerList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        internal static void GameServerRoomChangeMap(string param, PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;
                Player player = playerSession.Player;

                lock (room)
                {
                    if (room.RoomOwner != player) return;

                    int mapIndex = ObjectWrapper.Deserialize<int>(param);

                    if (mapIndex == NetworkObjectParameters.ChangeMapLeft) room.Map = Map.GetPreviousMap(room.Map);
                    else if (mapIndex == NetworkObjectParameters.ChangeMapRight) room.Map = Map.GetNextMap(room.Map);
                    else room.Map = Map.GetMap(mapIndex);

                    //send an update for each member of the match with the current metadata
                    BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshMetadata, room, room.PlayerList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerRoomRefreshLoadingPercentage(string param, PlayerSession playerSession)
        {
            try
            {
                int loadingPercentage = int.Parse(param);

                Console.WriteLine($"{playerSession.Player.ID} - {playerSession.Player.Nickname} is {loadingPercentage}%");

                BroadcastToPlayer(NetworkObjectParameters.GameServerRoomRefreshLoadingPercentage, new KeyValuePair<int, int>(playerSession.Player.ID, loadingPercentage), playerSession.RoomMetadata.PlayerList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerRoomStartInGameScene(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;

                playerSession.Player.PlayerLoadingStatus = PlayerRoomStatus.Ready;

                CheckStartInGameSceneCondition(room);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerRoomRequestDisconnect(PlayerSession playerSession)
        {
            RoomMetadata room = playerSession.RoomMetadata;

            lock (room)
                room.RemovePlayer(playerSession.Player);

            CheckStartInGameSceneCondition(room);
        }

        public static void CheckStartInGameSceneCondition(RoomMetadata matchMetadata)
        {
            lock (matchMetadata)
            {
                List<Player> roomUnion = matchMetadata.PlayerList;

                if (roomUnion.Any((x) => x.PlayerLoadingStatus != PlayerRoomStatus.Ready)) return;

                BroadcastToPlayer(NetworkObjectParameters.GameServerRoomStartInGameScene, null, roomUnion);
            }
        }
#endregion

        #region InGame / Requests
        public static void GameServerInGameStartMatch(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;

                lock (room)
                {
                    playerSession.Player.PlayerLoadingStatus = PlayerRoomStatus.NotReady;
                    playerSession.Player.PlayerNavigation = PlayerNavigation.InGame;

                    List<Player> roomUnion = room.PlayerList;
                    if (roomUnion.Any((x) => x.PlayerNavigation != PlayerNavigation.InGame)) return;

                    MatchManager mm = new MatchManager(room);

                    RegisterIntoPlayerSession(roomUnion, (playerE) => { playerE.MatchManager = mm; });

                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameStartMatch, mm.SyncMobileList, roomUnion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerInGameRefreshSyncMobile(string param, PlayerSession playerSession)
        {
            try
            {
                SyncMobile filter = ObjectWrapper.Deserialize<SyncMobile>(param);

                MatchManager mm = playerSession.MatchManager;
                Player player = playerSession.Player;

                lock (mm)
                {
                    SyncMobile sm = mm.SyncMobileList.ToList().Find((x) => x.Owner.ID == player.ID);
                    sm.Update(filter);
                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRefreshSyncMobile, sm, mm.MatchUnion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static MatchManager GameServerInGameRequestNextPlayerTurn(PlayerSession playerSession)
        {
            try
            {
                RoomMetadata room = playerSession.RoomMetadata;
                MatchManager mm = playerSession.MatchManager;

                lock (mm)
                {
                    mm.TurnOwner = mm.CurrentTurnOwner;

                    if (room.VictoriousTeam == null)
                        return mm;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }

            return null;
        }

        public static void GameServerInGameRequestShot(string param, PlayerSession playerSession)
        {
            try
            {
                SyncMobile filter = ObjectWrapper.Deserialize<SyncMobile>(param);
                MatchManager mm = playerSession.MatchManager;

                lock (mm)
                {
                    mm.ComputePlayerAction(filter);
                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestShot, filter, mm.MatchUnion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerInGameRequestLoseTurn(string param, PlayerSession playerSession)
        {
            try
            {
                SyncMobile filter = ObjectWrapper.Deserialize<SyncMobile>(param);
                RoomMetadata room = playerSession.RoomMetadata;
                MatchManager mm = playerSession.MatchManager;

                lock (mm)
                {
                    mm.ComputePlayerTurnSkip(filter);
                    mm.TurnOwner = mm.CurrentTurnOwner;
                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestNextPlayerTurn, room.VictoriousTeam == null ? mm : null, mm.MatchUnion);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerInGameRequestItemUsage(string param, PlayerSession playerSession)
        {
            try
            {
                SyncMobile filter = ObjectWrapper.Deserialize<SyncMobile>(param);
                MatchManager mm = playerSession.MatchManager;

                lock (mm)
                {
                    mm.ComputePlayerItem(filter);    
                }

                BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestItemUsage, filter, mm.MatchUnion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        #warning TODO: Simplify
        public static void GameServerInGameRequestDamage(string param, PlayerSession playerSession)
        {
            try
            {
                ProjectileDamage filter = ObjectWrapper.Deserialize<ProjectileDamage>(param);
                MatchManager mm = playerSession.MatchManager;

                List<Player> pList;
                List<SyncMobile> alivePlayerList;
                List<SyncMobile> syncMobileList;

                Player shootingPlayer;
                PlayerPerformanceMetadata ppm;

                int enemyDamage = 0;
                int allyDamage = 0;
                float travelledTime = 0;
                int angle = 0;
                HashSet<WeatherType> interactedWeatherSet = new HashSet<WeatherType>();
                // If a shot goes forward and the wind pushes it back.
                // Example: if mobile is facing left, the angle is bigger than 90 and the projectile has exploded anywhere on the right of the mobile
                bool boomerShot = false;
                // When a projectile explode on the opposite side the mobile is facing
                bool backwardShot = false;

                uint enemyKillCount = 0;
                uint allyKillCount = 0;

                int initialGoldAcquired;
                int totalGoldAcquired;
                int totalExpAcquired = 0;

                int previousTotalDamageDealt;
                int currentDamage = 0;

                List<List<CustomMessage>> llCM = new List<List<CustomMessage>>();

                lock (mm) {
                    pList = mm.MatchUnion;
                    syncMobileList = mm.SyncMobileList;
                    alivePlayerList = mm.SyncMobileList.Where((x) => x.IsAlive).ToList();
                    shootingPlayer = pList.Find((x) => x.ID == filter.OwnerID);
                    ppm = mm.MatchMetadata.PlayerPerformanceDictionary[shootingPlayer.ID];
                    totalGoldAcquired = initialGoldAcquired = ppm.GoldAmount;
                    currentDamage = ppm.TotalEnemyDamageDealt;
                    previousTotalDamageDealt = ppm.TotalEnemyDamageDealt;
                }

                if (shootingPlayer.ID != playerSession.Player.ID || filter.ProjectileDamageInformationList.Count == 0)
                    return;

                foreach (ProjectileDamageInformation pdi in filter.ProjectileDamageInformationList)
                {
                    Player target = pList.Find((x) => x.ID == pdi.TargetID);

                    if (target.PlayerTeam == shootingPlayer.PlayerTeam)
                    {
                        allyDamage += pdi.TargetDamage;

                        if (pdi.WasTargetKilled)
                        {
                            // Ally Kill
                            allyKillCount++;
                            totalGoldAcquired += NetworkObjectParameters.GoldAllyKill;

                            if (pdi.TargetID == shootingPlayer.ID)
                                llCM.Add(Message.CreateIGGDDSuicideMessage(shootingPlayer));
                            else
                                llCM.Add(Message.CreateIGGDDecreaseAllyKillMessage(shootingPlayer));

                            totalExpAcquired += NetworkObjectParameters.ExperienceAllyKill;

                        }
                    }
                    else
                    {
                        enemyDamage += pdi.TargetDamage;

                        if (pdi.WasTargetKilled)
                        {
                            // Enemy Kill
                            enemyKillCount++;
                            totalGoldAcquired += NetworkObjectParameters.GoldEnemyKill;

                            if (syncMobileList.Find((x) => x.Owner.ID == pdi.TargetID).CausaMortis == CausaMortis.NoHealth)
                                llCM.Add(Message.CreateIGGBEnemyKillMessage(shootingPlayer));
                            else
                                llCM.Add(Message.CreateIGGBBungeeKillMessage(shootingPlayer));

                            totalExpAcquired += NetworkObjectParameters.ExperienceEnemyKill;
                        }

                        boomerShot |=
                            (pdi.ShooterInitialFacingPosition == Facing.Left && pdi.FinalPosition[0] - pdi.StartingPosition[0] < 0 && pdi.ShotAngle > 90) ||
                            (pdi.ShooterInitialFacingPosition == Facing.Right && pdi.FinalPosition[0] - pdi.StartingPosition[0] > 0 && pdi.ShotAngle < 90);

                        backwardShot |=
                            (pdi.ShooterInitialFacingPosition == Facing.Left && pdi.FinalPosition[0] - pdi.StartingPosition[0] > 0) ||
                            (pdi.ShooterInitialFacingPosition == Facing.Right && pdi.FinalPosition[0] - pdi.StartingPosition[0] < 0);
                    }

                    foreach (WeatherType wt in pdi.InteractedWeatherSet)
                        interactedWeatherSet.Add(wt);

                    travelledTime = Math.Max(travelledTime, pdi.TotalTravelledTime);
                    angle = pdi.ShotAngle;
                }

                if (allyDamage > 30)
                {
                    // Ally Damage
                    totalGoldAcquired += NetworkObjectParameters.GoldFriendlyFire;
                    llCM.Add(Message.CreateIGGDFriendlyFireMessage(shootingPlayer));
                }

                if (enemyDamage != 0 || enemyKillCount > 0)
                {
                    //90º = 0, 89º = 1 (and so on), 91º = 359
                    if (angle >= 70 && angle <= 120)
                    {
                        if (travelledTime > 4)
                        {
                            // Ultra High Angle
                            totalGoldAcquired += NetworkObjectParameters.GoldUHighAngle;
                            llCM.Add(Message.CreateIGGBUHighAngleMessage(shootingPlayer));
                        }
                        else if (travelledTime > 2.5f)
                        {
                            // High Angle
                            totalGoldAcquired += NetworkObjectParameters.GoldHighAngle;
                            llCM.Add(Message.CreateIGGBHighAngleMessage(shootingPlayer));
                        }
                    }

                    if (interactedWeatherSet.Contains(WeatherType.Tornado))
                    {
                        // Tornado
                        totalGoldAcquired += NetworkObjectParameters.GoldWeatherShot;
                        llCM.Add(Message.CreateIGGBTornadoShotMessage(shootingPlayer));
                    }

                    if (interactedWeatherSet.Contains(WeatherType.Mirror))
                    {
                        // Tornado
                        totalGoldAcquired += NetworkObjectParameters.GoldWeatherShot;
                        llCM.Add(Message.CreateIGGBMirrorShotMessage(shootingPlayer));
                    }

                    if (boomerShot)
                    {
                        // Boomer shot
                        totalGoldAcquired += NetworkObjectParameters.GoldBoomerShot;
                        llCM.Add(Message.CreateIGGBGoldBoomerShotMessage(shootingPlayer));
                    }

                    if (backwardShot)
                    {
                        // Backshot
                        totalGoldAcquired += NetworkObjectParameters.GoldBackShot;
                        llCM.Add(Message.CreateIGGBBackShotMessage(shootingPlayer));
                    }

                    if (enemyDamage > 500)
                    {
                        // Excellent shot
                        totalGoldAcquired += NetworkObjectParameters.GoldExcellentShot;
                        llCM.Add(Message.CreateIGGBDExcellentShotMessage(shootingPlayer));
                    }
                    else if (enemyDamage > 250)
                    {
                        // Good Shot
                        totalGoldAcquired += NetworkObjectParameters.GoldGoodShot;
                        llCM.Add(Message.CreateIGGBDGoodShotMessage(shootingPlayer));
                    }
                    else if (enemyDamage > 150)
                    {
                        // Simple Shot
                        totalGoldAcquired += NetworkObjectParameters.GoldSimpleShot;
                        llCM.Add(Message.CreateIGGBDSimpleShotBonusMessage(shootingPlayer));
                    }

                    if (enemyDamage > 0)
                    {
                        int newDamage = currentDamage + enemyDamage;

                        if (currentDamage < 1000 && newDamage >= 1000)
                        {
                            // 1000dmg
                            totalGoldAcquired += NetworkObjectParameters.Gold1000Damage;
                            llCM.Add(Message.CreateIGGB1000DamageMessage(shootingPlayer));
                        }
                        else if (currentDamage < 1000 && newDamage >= 1000)
                        {
                            // 2000dmg
                            totalGoldAcquired += NetworkObjectParameters.Gold2000Damage;
                            totalExpAcquired += NetworkObjectParameters.Experience2000Damage;
                            llCM.Add(Message.CreateIGGB2000DamageMessage(shootingPlayer));
                        }
                        else if (currentDamage < 3000 && newDamage >= 3000)
                        {
                            // 3000dmg
                            totalGoldAcquired += NetworkObjectParameters.Gold3000Damage;
                            totalExpAcquired += NetworkObjectParameters.Experience3000Damage;
                            llCM.Add(Message.CreateIGGB3000DamageMessage(shootingPlayer));
                        }
                    }

                    if (enemyKillCount > 1)
                    {
                        // Multiple Kill
                        totalGoldAcquired += NetworkObjectParameters.GoldMultipleKill;
                        llCM.Add(Message.CreateIGGBMultipleKillMessage(shootingPlayer));
                    }
                }

                //Prestigy bonus
                if (initialGoldAcquired != totalGoldAcquired && playerSession.Player.Popularity != 0)
                    llCM.Add(Message.CreateIGPopularityMessage(shootingPlayer,
                        (int)((totalGoldAcquired - initialGoldAcquired) * (playerSession.Player.Popularity / 100f))));

                //Broadcast info about player's shot
                foreach (List<CustomMessage> lCM in llCM)
                {
                    BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendSystemMessage, lCM, pList);
                }

                //Victory-related player rewards
                int victoryGold = 0;
                int victoryExp = 0;

                int defeatGold = 0;
                int defeatExp = 0;

                PlayerTeam? victoriousTeam = null;
                if (enemyKillCount >= 1 || allyKillCount >= 1)
                {
                    //Win by K.O. always check the requesting player's team first (in case of draw)
                    if (!alivePlayerList.Exists((x) => x.Owner.PlayerTeam != playerSession.Player.PlayerTeam))
                        victoriousTeam = playerSession.Player.PlayerTeam;
                    else
                        victoriousTeam = (playerSession.Player.PlayerTeam == PlayerTeam.Blue ? PlayerTeam.Red : PlayerTeam.Blue);

                    if (victoriousTeam != null)
                    {
                        foreach (Player p in pList)
                        {
                            if (pList.Count <= (int)RoomSize.OneVsOne) {
                                victoryGold = NetworkObjectParameters.ExperienceWin1v1Match;
                                defeatGold = NetworkObjectParameters.ExperienceLose1v1Match;
                                victoryExp = NetworkObjectParameters.ExperienceWin1v1Match;
                                defeatExp = NetworkObjectParameters.ExperienceLose1v1Match;
                                BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendSystemMessage, (p.PlayerTeam == victoriousTeam) ? Message.CreateIGGBWin1v1MatchMessage(p) : Message.CreateIGGBLose1v1MatchMessage(p), p);
                            } else if (pList.Count <= (int)RoomSize.TwoVsTwo) {
                                victoryGold = NetworkObjectParameters.ExperienceWin2v2Match;
                                defeatGold = NetworkObjectParameters.ExperienceLose2v2Match;
                                victoryExp = NetworkObjectParameters.ExperienceWin2v2Match;
                                defeatExp = NetworkObjectParameters.ExperienceLose2v2Match;
                                BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendSystemMessage, (p.PlayerTeam == victoriousTeam) ? Message.CreateIGGBWin2v2MatchMessage(p) : Message.CreateIGGBLose2v2MatchMessage(p), p);
                            } else if (pList.Count <= (int)RoomSize.ThreeVsThree) {
                                victoryGold = NetworkObjectParameters.ExperienceWin3v3Match;
                                defeatGold = NetworkObjectParameters.ExperienceLose3v3Match;
                                victoryExp = NetworkObjectParameters.ExperienceWin3v3Match;
                                defeatExp = NetworkObjectParameters.ExperienceLose3v3Match;
                                BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendSystemMessage, (p.PlayerTeam == victoriousTeam) ? Message.CreateIGGBWin3v3MatchMessage(p) : Message.CreateIGGBLose3v3MatchMessage(p), p);
                            } else if (pList.Count <= (int)RoomSize.FourVsFour) {
                                victoryGold = NetworkObjectParameters.ExperienceWin4v4Match;
                                defeatGold = NetworkObjectParameters.ExperienceLose4v4Match;
                                victoryExp = NetworkObjectParameters.ExperienceWin4v4Match;
                                defeatExp = NetworkObjectParameters.ExperienceLose4v4Match;
                                BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendSystemMessage, (p.PlayerTeam == victoriousTeam) ? Message.CreateIGGBWin4v4MatchMessage(p) : Message.CreateIGGBLose4v4MatchMessage(p), p);
                            }
                        }
                    }
                }

                //Update player's gold amount
                if (victoriousTeam == null)
                {
                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestDamage, (int)(totalGoldAcquired * (1 + playerSession.Player.Popularity / 100f)), playerSession.Player);
                }

                //Compute Status
                lock (mm)
                {
                    ppm.AllyKillCount += allyKillCount;
                    ppm.EnemyKillCount += enemyKillCount;
                    ppm.TotalEnemyDamageDealt += enemyDamage;
                    ppm.TotalAllyDamageDealt += allyDamage;
                    if (allyDamage > 0) ppm.FriendlyFireCounter++;
                    if (enemyDamage > 0)
                    {
                        ppm.DirectHitCounter++;
                        ppm.HighAngleShotCounter += (angle >= 70 && angle <= 110 && travelledTime > 2.5) ? 1u : 0u;
                    }
                    ppm.GoldAmount = (int)(totalGoldAcquired * (1 + playerSession.Player.Popularity / 100f));
                    ppm.ExpAmount += totalExpAcquired;
                    ppm.ShotCounter++;

                    if (victoriousTeam != null)
                    {
                        PlayerController pc = new PlayerController();

                        foreach (Player p in pList)
                        {
                            PlayerPerformanceMetadata ppMetadata = mm.MatchMetadata.PlayerPerformanceDictionary[p.ID];

                            if (p.PlayerTeam == victoriousTeam)
                            {
                                ppMetadata.GoldAmount += victoryGold;
                                ppMetadata.ExpAmount += victoryExp;
                            }
                            else
                            {
                                ppMetadata.GoldAmount += defeatGold;
                                ppMetadata.ExpAmount += defeatExp;
                            }

                            //Commit alterations
                            pc.UpdatePlayerPerformanceMetadata(p.ID, ppMetadata);

                            //Send New Gold Balance to players
                            BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestDamage, ppMetadata.GoldAmount, p);
                        }
                    }
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex.Message}");
            }
        }

        public static void GameServerInGameRequestDeath(string param, PlayerSession playerSession)
        {
            try
            {
                SyncMobile filter = ObjectWrapper.Deserialize<SyncMobile>(param);
                MatchManager mm = playerSession.MatchManager;

                lock (mm)
                {
                    SyncMobile serverSideMobile = mm.SyncMobileList.Find((x) => x.Owner.ID == filter.Owner.ID);

                    if (serverSideMobile == null || !serverSideMobile.IsAlive) return;

                    serverSideMobile.Update(filter);
                    serverSideMobile.IsAlive = false;

                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestDeath, serverSideMobile, mm.MatchUnion);

                    //Sends death message
                    BroadcastMessage(Message.CreateDeathMessage(filter.Owner), mm.MatchUnion);

                    CheckWinConditions(mm, playerSession);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerInGameRequestDeath {ex.Message}");
            }
        }

        public static void GameServerInGameRequestDisconnect(PlayerSession playerSession)
        {
            try
            {
                MatchManager mm = playerSession.MatchManager;
                if (playerSession.MatchManager == null) return;

                lock (mm)
                {
                    SyncMobile sm = mm.SyncMobileList.Find((x) => x.Owner.ID == playerSession.Player.ID);

                    if (sm == null) return;

                    sm.IsAlive = false;

                    mm.SyncMobileList.Remove(sm);

                    BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestDisconnect, sm, mm.MatchUnion);
                    CheckWinConditions(mm, playerSession);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerInGameRequestDisconnect {ex.Message}");
            }
        }

        public static void CheckWinConditions(MatchManager matchManager, PlayerSession playerSession)
        {
            List<SyncMobile> aliveMobList = matchManager.SyncMobileList.Where((x) => x.IsAlive).ToList();

            PlayerTeam? victoriousTeam = null;

            //Win by K.O. always check the requesting player's team first (in case of draw)
            if (!aliveMobList.Exists((x) => x.Owner.PlayerTeam != playerSession.Player.PlayerTeam))
                victoriousTeam = playerSession.Player.PlayerTeam;
            else if (!aliveMobList.Exists((x) => x.Owner.PlayerTeam == playerSession.Player.PlayerTeam))
                victoriousTeam = (playerSession.Player.PlayerTeam == PlayerTeam.Blue ? PlayerTeam.Red : PlayerTeam.Blue);

            if (victoriousTeam == null) return;

            RoomMetadata room = playerSession.RoomMetadata;

            lock (room)
            {
                room.VictoriousTeam = victoriousTeam;
                room.IsPlaying = false;
                room.PlayerList.ForEach((x) => x.PlayerNavigation = PlayerNavigation.InGameRoom);
                /*save data on database*/
            }

            BroadcastToPlayer(NetworkObjectParameters.GameServerInGameRequestGameEnd, victoriousTeam, matchManager.MatchUnion);
        }
        #endregion

        #region Messaging / Room List Chat Requests
        public static bool GameServerChatEnterRequest(string param, PlayerSession playerSession)
        {
            try
            {
                //Parse it back to string in order to remove string formatation
                param = ObjectWrapper.Deserialize<string>(param);
                GameServerChatEnter(param, playerSession);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerChatEnterRequest {ex.Message}");
            }

             return false;
        }

        public static void GameServerChatEnter(string param, PlayerSession playerSession)
        {
            try
            {
                if (string.IsNullOrEmpty(param) || param == playerSession.CurrentConnectedChat) return;

                //Parse the player selected channel to find out its index
                (char, int) tuple = playerSession.GetCurrentConnectChatAsTuple(param);

                //If is a random connection, find which is the best suitable channel
                if (tuple.Item2 == 0) {
                    lock (GameServerObjects.Instance.ChatDictionary[tuple.Item1])
                    {
                        //Is attempting to connect on any game list channel, give the player the first possible channel
                        if (tuple.Item1 == NetworkObjectParameters.GameServerChatGameListIdentifier)
                        {
                            //find the first non-full channel IF the player hasn't selected a specific channel
                            tuple.Item2 = GameServerObjects.Instance.ChatDictionary[tuple.Item1].Keys
                                .First((x) => GameServerObjects.Instance.ChatDictionary[tuple.Item1][x].Count < NetworkObjectParameters.GameServerInformation.MaximumClientsPerChatChannel);

                            //Sends the player the newfound chat
                            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatJoinChannel, tuple.Item2);

                            //Connecting player receives a connecting message
                            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatEnter, playerSession.Player);

                            //The user receives every connected player
                            foreach(Player p in GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2])
                                playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatEnter, p);

                            //Everyone else receives this user metadata
                            BroadcastToPlayer(NetworkObjectParameters.GameServerChatEnter, playerSession.Player,
                                GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2]);

                            //Connect to room
                            GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2].Add(playerSession.Player);
                        }
                    }
                }
                else
                {
                    //Is attempting to connect on a specific room/channel
                    lock (GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2])
                    {
                        //Connects if there are free slots left on the channel.
                        //If the user is attempting to connect on a Room it should always return true
                        if (GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2].Count < NetworkObjectParameters.GameServerInformation.MaximumClientsPerChatChannel)
                        {
                            //Sends the player the newfound chat
                            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatJoinChannel, tuple.Item2);

                            //Connecting player receives a connecting message
                            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatEnter, playerSession.Player);

                            //The user receives every connected player
                            foreach (Player p in GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2])
                                playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatEnter, p);

                            //Everyone else receives this user metadata
                            BroadcastToPlayer(NetworkObjectParameters.GameServerChatEnter, playerSession.Player,
                                GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2]);

                            //Connect to room
                            GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2].Add(playerSession.Player);
                        }
                        else
                        {
                            //Error, the channel is full
                            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatJoinChannel, 0);
                            return;
                        }
                    }
                }

                //Leave any prior room
                GameServerChatLeave(playerSession);

                //Sends welcome message to player
                if (tuple.Item1 == NetworkObjectParameters.GameServerChatGameListIdentifier)
                {
                    BroadcastMessage(Message.CreateChannelWelcomeMessage(tuple.Item2), playerSession);
                }
                else
                {
                    BroadcastMessage(Message.CreateRoomWelcomeMessage(playerSession.RoomMetadata.Name), playerSession);
                }

                //Updates the current connected channel id
                playerSession.CurrentConnectedChat = tuple.Item1 + tuple.Item2.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerChatRoomListEnter {ex.Message}");
            }
        }

        public static void GameServerChatLeave(PlayerSession playerSession)
        {
            try
            {
                if (!playerSession.IsChatConnected) return;

                //Parse the current player channel
                (char, int) tuple = playerSession.GetCurrentConnectChatAsTuple();

                lock (GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2])
                {
                    //if player is connected to any channel, disconnect from the selected channel
                    if (!string.IsNullOrEmpty(playerSession.CurrentConnectedChat))
                    {
                        GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2].Remove(playerSession.Player);
                        BroadcastToPlayer(NetworkObjectParameters.GameServerChatLeave, playerSession.Player, GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2]);
                    }
                }

                //Updates the current connected channel id
                playerSession.CurrentConnectedChat = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerChatRoomListLeave {ex.Message}");
            }
        }

        public static void GameServerChatRoomSendMessage(string param, PlayerSession playerSession)
        {
            try
            {
                if (!playerSession.IsChatConnected) return;

                PlayerMessage pm = ObjectWrapper.Deserialize<PlayerMessage>(param);

                //Parse the player selected channel to find out its index
                (char, int) tuple = playerSession.GetCurrentConnectChatAsTuple();

                IEnumerable<Player> pList;

                lock (GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2])
                {
                    if (pm.PlayerTeam == null)
                        pList = GameServerObjects.Instance.ChatDictionary[tuple.Item1][tuple.Item2];
                    else if (pm.PlayerTeam == PlayerTeam.Blue)
                        pList = playerSession.RoomMetadata.TeamB;
                    else
                        pList = playerSession.RoomMetadata.TeamA;

                    BroadcastToPlayer(NetworkObjectParameters.GameServerChatSendPlayerMessage, pm, pList);
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: When GameServerChatRoomSendMessage {ex.Message}");
            }
        }
        #endregion

        #region Helping Functions
        private static void RegisterIntoPlayerSession(List<Player> playerList, Action<PlayerSession> action)
        {
            lock (GameServerObjects.Instance.PlayerHashtable)
            {
                playerList.ForEach((user) =>
                {
                    PlayerSession tmpSession = (PlayerSession)GameServerObjects.Instance.PlayerHashtable[user.ID];
                    action(tmpSession);
                });
            }
        }

        private static void BroadcastToPlayer(int service, object message, Player player)
        {
            lock (GameServerObjects.Instance.PlayerHashtable)
            {
                if (!GameServerObjects.Instance.PlayerHashtable.ContainsKey(player.ID)) return;
                ((PlayerSession)GameServerObjects.Instance.PlayerHashtable[player.ID]).ProviderQueue.Enqueue(service, message);
            }
        }

        private static void BroadcastToPlayer(int service, object message, IEnumerable<Player> players)
        {
            lock (GameServerObjects.Instance.PlayerHashtable)
            {
                foreach (Player p in players)
                {
                    if (!GameServerObjects.Instance.PlayerHashtable.ContainsKey(p.ID)) continue;
                    ((PlayerSession)GameServerObjects.Instance.PlayerHashtable[p.ID]).ProviderQueue.Enqueue(service, message);
                }
            }
        }

        private static void BroadcastMessage(object message, IEnumerable<Player> players)
        {
            lock (GameServerObjects.Instance.PlayerHashtable)
            {
                foreach (Player p in players)
                {
                    if (!GameServerObjects.Instance.PlayerHashtable.ContainsKey(p.ID)) continue;
                    ((PlayerSession)GameServerObjects.Instance.PlayerHashtable[p.ID]).ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatSendSystemMessage, message);
                }
            }
        }

        private static void BroadcastMessage<T>(List<T> message, PlayerSession playerSession)
        {
            playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatSendSystemMessage, message);
        }

        private static void BroadcastMessage<T>(List<List<T>> messageList, PlayerSession playerSession)
        {
            foreach(List<T> message in messageList)
                playerSession.ProviderQueue.Enqueue(NetworkObjectParameters.GameServerChatSendSystemMessage, message);
        }
#endregion

        #region Avatar Shop / Transactions
        public static AvatarMetadata GameServerAvatarShopBuyAvatar(string param, PlayerSession playerSession, PaymentMethod paymentMethod)
        {
            AvatarMetadata avatar = ObjectWrapper.Deserialize<AvatarMetadata>(param);
            bool success = new PlayerController().PurchaseAvatar(playerSession.Player, avatar, paymentMethod);

            if (success)
                return avatar;

            return null;
        }

        public static void GameServerAvatarShopUpdatePlayerMetadata(string param, PlayerSession playerSession)
        {
            Player player = ObjectWrapper.Deserialize<Player>(param);
            PlayerController pc = new PlayerController();

            //Validate attributes
            
            // If player has placed more points than he should OR
            if (player.GetCurrentAttributePoints() < player.Attribute.Sum() ||
                // If player has placed more points in a specific category than the maximum amount OR
                player.Attribute.ToList().Exists((x) => x > NetworkObjectParameters.PlayerAttributeMaximumPerCategory) ||
                // Checks if all equipped avatars are owned 
                !pc.CheckPlayerAvatarPossessions(player)
                )
                return;

            //Update DB
            pc.UpdatePlayerMetadata(player);

            //Update current session avatars
            playerSession.Player.Attribute = player.Attribute;
            playerSession.Player.Avatar = player.Avatar;
        }
        #endregion

        #region DEBUG
        private static void DebugMethod(PlayerSession playerSession)
        {
            if (!playerSession.RoomMetadata.PlayerList.Contains(playerSession.RoomMetadata.RoomOwner))
                Console.WriteLine("\n\n NOT FOUND HIM! OWNER \n\n");

            if (!playerSession.RoomMetadata.PlayerList.Contains(playerSession.Player))
                Console.WriteLine("\n\n NOT FOUND HIM! PLAYER \n\n");
        }
        #endregion

    }
}
