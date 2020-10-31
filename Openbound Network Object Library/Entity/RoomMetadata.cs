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

using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using System.Collections.Generic;
using System.Linq;
using OpenBound_Network_Object_Library.Models;

namespace OpenBound_Network_Object_Library.Entity
{
    public enum GameMode
    {
        Any = -1, //Filtering Options
        Solo = 0,
        Score = 1,
        Tag = 2,
        Jewel = 3,
    }

    public enum MatchSuddenDeathType
    {
        NoDeath = 0,
        BigBombDeath = 1,
        DoubleDeath = 2,
        SSDeath = 3,
    }

    public enum TurnsToSuddenDeath
    {
        Turn40 = 40,
        Turn56 = 56,
        Turn72 = 72,
    }

    public enum SlotModeType
    {
        Basic, Attack
    }

    public enum RoomSize
    {
        OneVsOne = 2,
        TwoVsTwo = 4,
        ThreeVsThree = 6,
        FourVsFour = 8,
    }

    public class RoomMetadata
    {
        //Match Settings
        [JsonProperty("GM")] public GameMode GameMode;
        [JsonProperty("TS")] public TurnsToSuddenDeath TurnsToSuddenDeath;
        [JsonProperty("MS")] public MatchSuddenDeathType MatchSuddenDeathType;
        [JsonProperty("SI")] public RoomSize Size;
        [JsonProperty("SM")] public SlotModeType SlotModeType;
        [JsonProperty("MA")] public Map Map;
        [JsonProperty("LS")] public Dictionary<int, int[]> LoadingScreenSpawnPosition;
        [JsonProperty("SP")] public Dictionary<int, int[]> SpawnPositions;
        [JsonProperty("LR")] public List<int[]> LoadingScreenRemaningSpawnPosition;
        [JsonProperty("VT")] public PlayerTeam? VictoriousTeam;

        //Room Information
        public int ID;
        private string name;
        [JsonProperty("NA")] public string Name
        {
            get => name;
            set
            {
                if (value?.Length > 16) name = value.Substring(0, 16);
                else name = value;
            }
        }

        private string password;
        [JsonProperty("PA")] public string Password
        {
            get => password;
            set
            {
                if (value?.Length > 16) password = value.Substring(0, 16);
                else password = value;
            }
        }
        public bool IsPlaying;

        [JsonIgnore] public List<Player> OriginalTeamA, OriginalTeamB;
        [JsonIgnore] public int OriginalTeamSize;

        [JsonIgnore] private readonly object playerListMutex = new object();

        [JsonProperty("TA")] public List<Player> TeamA;
        [JsonProperty("TB")] public List<Player> TeamB;

        [JsonIgnore] public List<Player> TeamASafe { get { lock (playerListMutex) { return TeamA.ToList(); } } }
        [JsonIgnore] public List<Player> TeamBSafe { get { lock (playerListMutex) { return TeamB.ToList(); } } }
        [JsonIgnore] public List<Player> PlayerList { get { lock (playerListMutex) { return TeamA.Union(TeamB).ToList(); } } }

        [JsonProperty("RO")] public Player RoomOwner;

        //Filtering Options
        [JsonProperty("PN")] public int PageNumber;

        public RoomMetadata(GameMode gameMode,
            TurnsToSuddenDeath turnsToSuddenDeath, MatchSuddenDeathType matchSuddenDeathType,
            RoomSize roomSize, Player roomOwner, int roomNumber, string roomName, string roomPassword)
        {
            GameMode = gameMode;
            TurnsToSuddenDeath = turnsToSuddenDeath;
            MatchSuddenDeathType = matchSuddenDeathType;

            Map = Map.GetMap(GameMapType.A, GameMap.Random);

            Size = roomSize;

            RoomOwner = roomOwner;

            ID = roomNumber;
            Name = roomName;
            Password = roomPassword;

            TeamA = new List<Player>() { roomOwner };
            TeamB = new List<Player>();

            IsPlaying = false;
        }

        public void AddA(Player player)
        {
            lock (playerListMutex)
            {
                TeamA.Add(player);
                player.PlayerTeam = PlayerTeam.Red;
            }
        }

        public void AddB(Player player)
        {
            lock (playerListMutex)
            {
                TeamB.Add(player);
                player.PlayerTeam = PlayerTeam.Blue;
            }
        }

        public void RemovePlayer(Player player)
        {
            lock (playerListMutex)
            {
                TeamA.RemoveAll((x) => x.ID == player.ID);
                TeamB.RemoveAll((x) => x.ID == player.ID);
            }
        }

        [JsonIgnore] public int NumberOfPlayers { get { lock (playerListMutex) { return TeamA.Count + TeamB.Count; } } }
        [JsonIgnore] public bool HasPassword { get { return !(Password == ""); } }
        [JsonIgnore] public bool IsFull { get { return NumberOfPlayers == (int)Size; } }

        public void StartMatch()
        {
            VictoriousTeam = null;
            IsPlaying = true;

            List<Player> pList = PlayerList;

            //Saving Player
            OriginalTeamA = TeamA.ToList();
            OriginalTeamB = TeamB.ToList();

            OriginalTeamSize = OriginalTeamA.Count() + OriginalTeamB.Count();

            //Map
            if (Map.GameMap == GameMap.Random)
                Map = Map.GetRandomMap(Map.GameMapType);

            //Mobiles
            foreach (Player p in pList)
            {
                if (p.PrimaryMobile == MobileType.Random)
                {
                    List<MobileType> existingMobileList = NetworkObjectParameters.ImplementedMobileList.Except(new List<MobileType>() { MobileType.Random }).ToList();
                    p.PrimaryMobile = existingMobileList[NetworkObjectParameters.Random.Next(0, existingMobileList.Count - 1)];
                }
            }

            //Spawn Coordinates
            LoadingScreenSpawnPosition = new Dictionary<int, int[]>();
            SpawnPositions = new Dictionary<int, int[]>();

            pList.OrderBy((x) => NetworkObjectParameters.Random.NextDouble()).ToList();

            List<int[]> spawnCoordinateList = Map.SpawnPoints.ToList();

            PlayerList.ForEach((x) =>
            {
                int randomizedIndex = NetworkObjectParameters.Random.Next(0, spawnCoordinateList.Count());

                SpawnPositions.Add(x.ID, spawnCoordinateList[randomizedIndex]);
                spawnCoordinateList.RemoveAt(randomizedIndex);
            });
        }
    }
}
