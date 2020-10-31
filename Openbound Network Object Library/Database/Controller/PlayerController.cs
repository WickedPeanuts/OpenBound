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

using OpenBound_Network_Object_Library.Database.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OpenBound_Network_Object_Library.ValidationModel;
using CryptSharp;
using OpenBound_Network_Object_Library.Models;
using CryptSharp.Core;
using Microsoft.EntityFrameworkCore;
using OpenBound_Network_Object_Library.Entity;

namespace OpenBound_Network_Object_Library.Database.Controller
{
    public class PlayerController
    {

        public Player LoginPlayer(Player filter) {
            try
            {
                filter.Nickname = filter.Nickname.ToLower();

                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    Player tmpPlayer = odc.Players.Include((p) => p.Guild)
                        .FirstOrDefault((x) => x.Nickname.ToLower() == filter.Nickname);

                    if (tmpPlayer == null || !Crypter.CheckPassword(filter.Password, tmpPlayer.Password)) return null;

                    tmpPlayer.NullifySensitiveData();

                    if (tmpPlayer.Guild != null)
                        tmpPlayer.Guild.GuildMembers = null;

                    return tmpPlayer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return null;
        } 

        public Dictionary<AvatarCategory, HashSet<int>> RetrievePlayerAvatarList(Player player)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    Player tmpPlayer = odc.Players
                        .Include(x => x.PlayerAvatarMetadataList)
                        .FirstOrDefault((x) => x.ID == player.ID);

                    tmpPlayer.LoadOwnedAvatarDictionary();

                    return tmpPlayer.OwnedAvatar;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Register a new player (account) and if the registration was successful.
        /// </summary>
        public Player RegisterPlayer(PlayerDTO playerDTO)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    Player newPlayer = new Player()
                    {
                        Nickname = playerDTO.Nickname,
                        Password = Crypter.Blowfish.Crypt(playerDTO.Password),
                        Gender = playerDTO.Gender,
                        Email = playerDTO.Email,
                    };

                    //Check if there is another player with the same nickname/email
                    List<Player> repeatedCredentialPlayerList = odc.Players
                        .Where(x => x.Nickname.ToLower() == playerDTO.Nickname.ToLower() ||
                        x.Email.ToLower() == playerDTO.Email.ToLower()).ToList();

                    //There is another player using the requested nickname/password
                    if (repeatedCredentialPlayerList.Count > 0)
                    {
                        // Preparing a custom message to SignUpForm. Check
                        // OpenBound_Game_Launcher.Launcher.Connection.LauncherRequestManager.Register for more insight

                        if (repeatedCredentialPlayerList.Exists((x) => x.Nickname == newPlayer.Nickname))
                        {
                            //Null nicknames are going to be interpreted as "Nickname is already in use"
                            newPlayer.Nickname = null;
                        }

                        if (repeatedCredentialPlayerList.Exists((x) => x.Password == newPlayer.Password))
                        {
                            //Null passwords are going to be interpreted as "Password is already in use"
                            newPlayer.Password = null;
                        }
                    }
                    else
                    {
                        //Adds a new player player
                        using (var transaction = odc.Database.BeginTransaction())
                        {
                            Player player = odc.Players.Add(newPlayer).Entity;

                            List<PlayerAvatarMetadata> basicAvatarList = new List<PlayerAvatarMetadata>()
                            {
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Hat,     Gender = player.Gender  }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Body,    Gender = player.Gender  }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Extra,   Gender = Gender.Unissex }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Flag,    Gender = Gender.Unissex }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Goggles, Gender = Gender.Unissex }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Misc,    Gender = Gender.Unissex }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.Pet,     Gender = Gender.Unissex }, PaymentMethod.Free),
                                new PlayerAvatarMetadata(player, new AvatarMetadata() { ID = 0, AvatarCategory = AvatarCategory.ExItem,  Gender = Gender.Unissex }, PaymentMethod.Free)
                            };

                            basicAvatarList.ForEach((x) => player.PlayerAvatarMetadataList.Add(x));

                            odc.Players.Add(player);
                            transaction.Commit();
                        }

                        odc.SaveChanges();
                    }

                    return newPlayer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return null;
        }


        public bool PurchaseAvatar(Player player, AvatarMetadata avatarMetadata, PaymentMethod paymentMethod)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    AvatarMetadata databaseAvatar = odc.AvatarMetadata
                        .FirstOrDefault((x) => x.ID == avatarMetadata.ID && x.AvatarCategory == avatarMetadata.AvatarCategory);//.Include();

                    if ((paymentMethod == PaymentMethod.Gold && player.Gold >= databaseAvatar.GoldPrice && databaseAvatar.GoldPrice > 0) ||
                        (paymentMethod == PaymentMethod.Cash && player.Cash >= databaseAvatar.CashPrice && databaseAvatar.CashPrice > 0) ||
                        (paymentMethod == PaymentMethod.Cash && player.Cash >= databaseAvatar.CashPrice && databaseAvatar.CashPrice > 0))
                    {
                        using (var dbContextTransaction = odc.Database.BeginTransaction())
                        {
                            Player p = odc.Players
                                .First((x) => x.ID == player.ID);

                            odc.PlayerAvatarMetadata.Add(new PlayerAvatarMetadata(p, databaseAvatar, paymentMethod));

                            if (paymentMethod == PaymentMethod.Gold)
                                p.Gold -= databaseAvatar.GoldPrice;
                            else if (paymentMethod == PaymentMethod.Cash)
                                p.Cash -= databaseAvatar.CashPrice;
                            else if (paymentMethod == PaymentMethod.Token)
                                p.Token -= databaseAvatar.TokenPrice;

                            odc.SaveChanges();

                            dbContextTransaction.Commit();
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Returns true if all player's equipped avatar exists in player's owned avatar list,
        /// otherwise returns false
        /// </summary>
        public bool CheckPlayerAvatarPossessions(Player player)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    //Searches for the 8 equipped avatars in players db avatar list.
                    //If the number of matchs is equal to the number of equipped avatars
                    //All avatars are owned, therefore returns true
                    return odc.Players
                      .Include(q => q.PlayerAvatarMetadataList)
                      .ThenInclude(o => o.AvatarMetadata)
                      .First((x) => x.ID == player.ID)
                      .PlayerAvatarMetadataList
                      .Where(am =>
                          (am.AvatarMetadata.ID == player.EquippedAvatarHat     && am.AvatarMetadata.AvatarCategory == AvatarCategory.Hat)     ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarBody    && am.AvatarMetadata.AvatarCategory == AvatarCategory.Body)    ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarGoggles && am.AvatarMetadata.AvatarCategory == AvatarCategory.Goggles) ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarFlag    && am.AvatarMetadata.AvatarCategory == AvatarCategory.Flag)    ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarExItem  && am.AvatarMetadata.AvatarCategory == AvatarCategory.ExItem)  ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarPet     && am.AvatarMetadata.AvatarCategory == AvatarCategory.Pet)     ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarMisc    && am.AvatarMetadata.AvatarCategory == AvatarCategory.Misc)    ||
                          (am.AvatarMetadata.ID == player.EquippedAvatarExtra   && am.AvatarMetadata.AvatarCategory == AvatarCategory.Extra))
                      .Count() >= player.Avatar.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Updates the player equiped avatar list and the selected attributes
        /// </summary>
        /// <param name="player"></param>
        public void UpdatePlayerMetadata(Player player)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    Player p = odc.Players.First((x) => x.ID == player.ID);
                    p.Avatar = player.Avatar;
                    p.Attribute = player.Attribute;
                    odc.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }
        }

        public void UpdatePlayerPerformanceMetadata(int playerID, PlayerPerformanceMetadata playerPerformanceMetadata)
        {
            try
            {
                using (OpenBoundDatabaseContext odc = new OpenBoundDatabaseContext())
                {
                    Player p = odc.Players.First((x) => x.ID == playerID);
                    p.DirectHit += playerPerformanceMetadata.DirectHitCounter;
                    p.ShotCounter += playerPerformanceMetadata.ShotCounter;
                    p.FriendlyFire += playerPerformanceMetadata.FriendlyFireCounter;
                    p.HighAngleShots += playerPerformanceMetadata.HighAngleShotCounter;
                    p.EnemyKill += playerPerformanceMetadata.EnemyKillCount;
                    p.AllyKill += playerPerformanceMetadata.AllyKillCount;
                    p.Gold += playerPerformanceMetadata.GoldAmount;
                    p.Experience += playerPerformanceMetadata.ExpAmount;
                    odc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
            }
        }
    }
}
